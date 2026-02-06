import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { ChatMessage, ChatRequest } from '../models';
import { Subject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private sessionId: string = '';
  private messageSubject = new Subject<ChatMessage>();
  public messages$ = this.messageSubject.asObservable();

  constructor(private apiService: ApiService) {
    this.sessionId = this.generateSessionId();
  }

  private generateSessionId(): string {
    return 'session_' + Date.now().toString(36) + Math.random().toString(36).substr(2, 9);
  }

  getSessionId(): string {
    return this.sessionId;
  }

  sendMessage(content: string): Observable<string> {
    return new Observable(observer => {
      const request: ChatRequest = {
        message: content,
        sessionId: this.sessionId
      };

      // Add user message to stream
      this.messageSubject.next({
        role: 'user',
        content: content,
        timestamp: new Date()
      });

      this.apiService.streamMessage(request).subscribe({
        next: (chunk: string) => {
          const lines = chunk.split('\n\n');
          for (const line of lines) {
            if (line.startsWith('data: ')) {
              const data = line.slice(6);
              if (data === '[DONE]') {
                observer.complete();
                return;
              }
              observer.next(data);
            }
          }
        },
        error: (err) => observer.error(err),
        complete: () => observer.complete()
      });
    });
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
      error: (err) => {
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
