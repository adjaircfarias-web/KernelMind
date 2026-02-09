import { Component, OnInit, OnDestroy, ViewChild, ElementRef, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChatService } from '../../services/chat.service';
import { ApiService } from '../../services/api.service';
import { ChatMessage, Pizza } from '../../models';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="chat-container">
      <!-- Header -->
      <div class="chat-header">
        <div class="header-content">
          <span class="pizza-emoji">🍕</span>
          <div class="header-text">
            <h1>KernelMind</h1>
            <span class="subtitle">Pizzaria Inteligente</span>
          </div>
        </div>
        <div class="header-actions">
          <button class="menu-btn" (click)="toggleMenu()">
            📋 Cardápio
          </button>
        </div>
      </div>

      <!-- Chat Messages -->
      <div class="chat-messages" #messagesContainer>
        <!-- Welcome Message -->
        <div class="message assistant">
          <div class="avatar">🤖</div>
          <div class="message-content">
            <p>Olá! 👋 Sou o assistente virtual da KernelMind.</p>
            <p>Posso ajudá-lo a:</p>
            <ul>
              <li>🍕 Ver o cardápio de pizzas</li>
              <li>🔍 Buscar pizzas por ingredientes</li>
              <li>📦 Fazer pedidos</li>
              <li>💰 Calcular totais com entrega</li>
            </ul>
            <p>Como posso ajudá-lo hoje?</p>
          </div>
        </div>

        <!-- Messages -->
        @for (msg of messages; track msg.timestamp) {
          <div [class]="'message ' + msg.role">
            <div class="avatar">{{ msg.role === 'user' ? '👤' : '🤖' }}</div>
            <div class="message-content">{{ msg.content }}</div>
          </div>
        }

        <!-- Typing Indicator -->
        @if (isTyping) {
          <div class="message assistant typing">
            <div class="avatar">🤖</div>
            <div class="message-content typing-indicator">
              <span></span>
              <span></span>
              <span></span>
            </div>
          </div>
        }
      </div>

      <!-- Input Area -->
      <div class="chat-input">
        <div class="input-container">
          <textarea
            #messageInput
            [(ngModel)]="newMessage"
            (keydown.enter)="sendMessage($any($event))"
            (input)="autoResize($event)"
            placeholder="Digite sua mensagem..."
            rows="1"
            [disabled]="isTyping"
          ></textarea>
          <button 
            class="send-btn" 
            (click)="sendMessage()"
            [disabled]="!newMessage.trim() || isTyping"
          >
            ➤
          </button>
        </div>
      </div>
    </div>

    <!-- Menu Sidebar -->
    @if (showMenu) {
      <div class="menu-overlay" (click)="toggleMenu()">
        <div class="menu-sidebar" (click)="$event.stopPropagation()">
          <div class="menu-header">
            <h2>🍕 Cardápio</h2>
            <button class="close-btn" (click)="toggleMenu()">✕</button>
          </div>
          <div class="menu-content">
            @for (category of categories; track category) {
              <div class="category-section">
                <h3>{{ category }}</h3>
                @for (pizza of getPizzasByCategory(category); track pizza.id) {
                  <div class="pizza-item">
                    <div class="pizza-info">
                      <span class="pizza-name">{{ pizza.name }}</span>
                      <span class="pizza-price">{{ pizza.price | currency:'BRL' }}</span>
                    </div>
                    <p class="pizza-desc">{{ pizza.description }}</p>
                  </div>
                }
              </div>
            }
          </div>
        </div>
      </div>
    }
  `,
  styles: [`
    .chat-container {
      display: flex;
      flex-direction: column;
      height: 100vh;
      max-width: 800px;
      margin: 0 auto;
      background: #fff;
      border-radius: 12px;
      box-shadow: 0 4px 20px rgba(0,0,0,0.1);
      overflow: hidden;
    }

    .chat-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 16px 24px;
      background: linear-gradient(135deg, #d32f2f 0%, #b71c1c 100%);
      color: white;
    }

    .header-content {
      display: flex;
      align-items: center;
      gap: 12px;
    }

    .pizza-emoji {
      font-size: 32px;
    }

    .header-text h1 {
      margin: 0;
      font-size: 24px;
      font-weight: 700;
    }

    .subtitle {
      font-size: 12px;
      opacity: 0.9;
    }

    .menu-btn {
      background: rgba(255,255,255,0.2);
      border: 1px solid rgba(255,255,255,0.3);
      color: white;
      padding: 8px 16px;
      border-radius: 20px;
      cursor: pointer;
      font-size: 14px;
      transition: all 0.2s;
    }

    .menu-btn:hover {
      background: rgba(255,255,255,0.3);
    }

    .chat-messages {
      flex: 1;
      overflow-y: auto;
      padding: 24px;
      display: flex;
      flex-direction: column;
      gap: 16px;
      background: #f5f5f5;
    }

    .message {
      display: flex;
      gap: 12px;
      max-width: 85%;
    }

    .message.user {
      align-self: flex-end;
      flex-direction: row-reverse;
    }

    .avatar {
      width: 40px;
      height: 40px;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 20px;
      flex-shrink: 0;
    }

    .message.assistant .avatar {
      background: #d32f2f;
      color: white;
    }

    .message.user .avatar {
      background: #4caf50;
      color: white;
    }

    .message-content {
      background: white;
      padding: 12px 16px;
      border-radius: 16px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.08);
      line-height: 1.5;
    }

    .message.user .message-content {
      background: #d32f2f;
      color: white;
    }

    .message-content ul {
      margin: 8px 0;
      padding-left: 20px;
    }

    .message-content li {
      margin: 4px 0;
    }

    .typing-indicator {
      display: flex;
      gap: 4px;
      padding: 12px 16px;
    }

    .typing-indicator span {
      width: 8px;
      height: 8px;
      background: #999;
      border-radius: 50%;
      animation: typing 1.4s infinite ease-in-out;
    }

    .typing-indicator span:nth-child(2) {
      animation-delay: 0.2s;
    }

    .typing-indicator span:nth-child(3) {
      animation-delay: 0.4s;
    }

    @keyframes typing {
      0%, 80%, 100% { transform: scale(0.8); opacity: 0.5; }
      40% { transform: scale(1); opacity: 1; }
    }

    .chat-input {
      padding: 16px 24px;
      background: white;
      border-top: 1px solid #eee;
    }

    .input-container {
      display: flex;
      gap: 12px;
      align-items: flex-end;
      background: #f5f5f5;
      border-radius: 24px;
      padding: 8px 8px 8px 20px;
    }

    textarea {
      flex: 1;
      border: none;
      background: transparent;
      resize: none;
      font-size: 16px;
      line-height: 24px;
      max-height: 120px;
      outline: none;
    }

    .send-btn {
      width: 44px;
      height: 44px;
      border-radius: 50%;
      border: none;
      background: #d32f2f;
      color: white;
      font-size: 20px;
      cursor: pointer;
      transition: all 0.2s;
    }

    .send-btn:hover:not(:disabled) {
      background: #b71c1c;
      transform: scale(1.05);
    }

    .send-btn:disabled {
      background: #ccc;
      cursor: not-allowed;
    }

    .menu-overlay {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background: rgba(0,0,0,0.5);
      display: flex;
      justify-content: flex-end;
      z-index: 1000;
    }

    .menu-sidebar {
      width: 400px;
      max-width: 100%;
      background: white;
      height: 100%;
      display: flex;
      flex-direction: column;
    }

    .menu-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 20px;
      border-bottom: 1px solid #eee;
    }

    .menu-header h2 {
      margin: 0;
    }

    .close-btn {
      width: 36px;
      height: 36px;
      border-radius: 50%;
      border: none;
      background: #f5f5f5;
      cursor: pointer;
      font-size: 18px;
    }

    .menu-content {
      flex: 1;
      overflow-y: auto;
      padding: 16px;
    }

    .category-section {
      margin-bottom: 24px;
    }

    .category-section h3 {
      color: #d32f2f;
      border-bottom: 2px solid #d32f2f;
      padding-bottom: 8px;
      margin-bottom: 12px;
    }

    .pizza-item {
      padding: 12px;
      border-radius: 8px;
      margin-bottom: 8px;
      background: #f9f9f9;
    }

    .pizza-info {
      display: flex;
      justify-content: space-between;
      font-weight: 600;
    }

    .pizza-price {
      color: #d32f2f;
    }

    .pizza-desc {
      margin: 8px 0 0;
      font-size: 13px;
      color: #666;
    }
  `]
})
export class ChatComponent implements OnInit, OnDestroy {
  messages: ChatMessage[] = [];
  newMessage = '';
  isTyping = false;
  showMenu = false;
  categories: string[] = [];
  pizzas: Pizza[] = [];
  private messageSubscription?: Subscription;

  @Output() pizzaSelected = new EventEmitter<Pizza>();
  @ViewChild('messagesContainer') private messagesContainer!: ElementRef;
  @ViewChild('messageInput') private messageInput!: ElementRef;

  constructor(
    private chatService: ChatService,
    private apiService: ApiService
  ) {}

  ngOnInit(): void {
    this.messageSubscription = this.chatService.messages$.subscribe(msg => {
      this.messages.push(msg);
      this.scrollToBottom();
    });
    this.loadMenu();
  }

  ngOnDestroy(): void {
    this.messageSubscription?.unsubscribe();
  }

  loadMenu(): void {
    this.apiService.getMenu().subscribe({
      next: (pizzas) => {
        this.pizzas = pizzas;
        this.categories = [...new Set(pizzas.map(p => p.category))];
      }
    });
  }

  getPizzasByCategory(category: string): Pizza[] {
    return this.pizzas.filter(p => p.category === category);
  }

  toggleMenu(): void {
    this.showMenu = !this.showMenu;
  }

  addToOrder(pizza: Pizza, event: Event): void {
    event.stopPropagation();
    this.pizzaSelected.emit(pizza);
  }

  sendMessage(event?: KeyboardEvent): void {
    if (event && !event.shiftKey) {
      event.preventDefault();
    }

    if (!this.newMessage.trim() || this.isTyping) return;

    const content = this.newMessage;
    this.newMessage = '';
    this.autoResize({ target: { style: { height: 'auto' } } } as any);

    this.isTyping = true;

    // Create assistant message placeholder for streaming chunks
    const assistantMessage: ChatMessage = {
      role: 'assistant',
      content: '',
      timestamp: new Date()
    };
    this.messages.push(assistantMessage);

    this.chatService.sendMessage(content).subscribe({
      next: (chunk) => {
        assistantMessage.content += chunk;
        this.isTyping = false;
        this.scrollToBottom();
      },
      error: () => {
        if (!assistantMessage.content) {
          assistantMessage.content = 'Desculpe, ocorreu um erro. Tente novamente.';
        }
        this.isTyping = false;
        this.scrollToBottom();
      },
      complete: () => {
        this.isTyping = false;
        this.scrollToBottom();
      }
    });
  }

  autoResize(event: any): void {
    const element = event.target as HTMLElement;
    element.style.height = 'auto';
    element.style.height = Math.min(element.scrollHeight, 120) + 'px';
  }

  private scrollToBottom(): void {
    setTimeout(() => {
      try {
        this.messagesContainer.nativeElement.scrollTop = this.messagesContainer.nativeElement.scrollHeight;
      } catch {}
    }, 10);
  }
}
