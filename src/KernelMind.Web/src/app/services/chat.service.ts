import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { ChatMessage, ChatRequest } from '../models';
import { Subject, Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private sessionId: string = '';
  private messageSubject = new Subject<ChatMessage>();
  public messages$ = this.messageSubject.asObservable();

  private apiService = inject(ApiService);

  constructor() {
    this.sessionId = this.generateSessionId();
  }

  private generateSessionId(): string {
    return 'session_' + Date.now().toString(36) + Math.random().toString(36).substr(2, 9);
  }

  getSessionId(): string {
    return this.sessionId;
  }

  /**
   * Sends a message and streams the response using native fetch + ReadableStream.
   * Angular HttpClient does not support true SSE streaming (it buffers the full response).
   */
  sendMessage(content: string): Observable<string> {
    return new Observable(observer => {
      const request: ChatRequest = {
        message: content,
        sessionId: this.sessionId
      };

      // Push user message
      this.messageSubject.next({
        role: 'user',
        content: content,
        timestamp: new Date()
      });

      const url = `${environment.apiUrl}/chat/stream/raw`;
      const abortController = new AbortController();

      fetch(url, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(request),
        signal: abortController.signal
      })
        .then(response => {
          if (!response.ok) {
            throw new Error(`HTTP ${response.status}: ${response.statusText}`);
          }

          const reader = response.body!.getReader();
          const decoder = new TextDecoder();
          let buffer = '';

          const readStream = (): Promise<void> => {
            return reader.read().then(({ done, value }) => {
              if (done) {
                // Process any remaining data in buffer
                if (buffer.trim()) {
                  this.processSSEBuffer(buffer, observer);
                }
                observer.complete();
                return;
              }

              buffer += decoder.decode(value, { stream: true });

              // Process complete SSE events (separated by double newline)
              const events = buffer.split('\n\n');
              // Keep the last part as it may be incomplete
              buffer = events.pop() || '';

              for (const event of events) {
                const trimmed = event.trim();
                if (!trimmed) continue;

                if (trimmed.startsWith('data: ')) {
                  const data = trimmed.slice(6);
                  if (data === '[DONE]' || data === '[CANCELLED]') {
                    observer.complete();
                    return;
                  }
                  if (data.startsWith('ERROR:')) {
                    observer.error(new Error(data));
                    return;
                  }
                  observer.next(data);
                }
              }

              return readStream();
            });
          };

          return readStream();
        })
        .catch(err => {
          if (err.name !== 'AbortError') {
            observer.error(err);
          }
        });

      // Cleanup: abort the fetch on unsubscribe
      return () => {
        abortController.abort();
      };
    });
  }

  private processSSEBuffer(buffer: string, observer: any): void {
    const lines = buffer.split('\n');
    for (const line of lines) {
      const trimmed = line.trim();
      if (trimmed.startsWith('data: ')) {
        const data = trimmed.slice(6);
        if (data !== '[DONE]' && data !== '[CANCELLED]' && !data.startsWith('ERROR:')) {
          observer.next(data);
        }
      }
    }
  }

  sendMessageSync(content: string): void {
    const request: ChatRequest = {
      message: content,
      sessionId: this.sessionId
    };

    this.messageSubject.next({
      role: 'user',
      content: content,
      timestamp: new Date()
    });

    this.apiService.sendMessage(request).subscribe({
      next: (response) => {
        this.messageSubject.next({
          role: 'assistant',
          content: response.content,
          timestamp: new Date(response.timestamp)
        });
      },
      error: () => {
        this.messageSubject.next({
          role: 'assistant',
          content: 'Desculpe, ocorreu um erro. Tente novamente.',
          timestamp: new Date()
        });
      }
    });
  }

  clearMessages(): void {
    this.sessionId = this.generateSessionId();
  }
}
