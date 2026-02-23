import { Injectable, inject } from '@angular/core';
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
   * Sends a message and returns the complete response.
   * NOTE: Using synchronous endpoint to enable Function Calling with Semantic Kernel.
   * Ollama does not support function calling in streaming mode.
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

      // Use synchronous endpoint to enable function calling
      this.apiService.sendMessage(request).subscribe({
        next: (response) => {
          // Emit the complete response as a single chunk
          observer.next(response.content);
          observer.complete();
        },
        error: (err) => {
          observer.error(err);
        }
      });

      // No cleanup needed for HTTP request
      return () => {};
    });
  }

  /**
   * Sends a message and streams the response using native fetch + ReadableStream.
   * DEPRECATED: Use sendMessage() instead. Streaming does not support function calling with Ollama.
   */
  sendMessageStream(content: string): Observable<string> {
    console.warn('sendMessageStream is deprecated. Use sendMessage() for full function calling support.');
    return this.sendMessage(content);
  }

  clearMessages(): void {
    this.sessionId = this.generateSessionId();
  }
}
