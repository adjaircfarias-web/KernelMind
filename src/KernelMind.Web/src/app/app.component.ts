import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { ChatComponent } from './components/chat/chat.component';
import { MenuComponent } from './components/menu/menu.component';
import { OrderComponent } from './components/order/order.component';
import { Pizza } from './models';
import { OrderStateService } from './services';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, HttpClientModule, ChatComponent, MenuComponent, OrderComponent],
  template: `
    <div class="app-container">
      <nav class="app-nav">
        <div class="nav-brand">
          <span class="brand-emoji">🍕</span>
          <span class="brand-name">KernelMind</span>
        </div>
        <div class="nav-tabs">
          <button
            [class.active]="activeTab === 'chat'"
            (click)="activeTab = 'chat'"
          >
            💬 Chat
          </button>
          <button
            [class.active]="activeTab === 'menu'"
            (click)="activeTab = 'menu'"
          >
            📋 Cardápio
          </button>
          <button
            [class.active]="activeTab === 'order'"
            (click)="activeTab = 'order'"
          >
            🛒 Pedido
            @if (orderItemCount > 0) {
              <span class="badge">{{ orderItemCount }}</span>
            }
          </button>
        </div>
      </nav>

      <main class="app-content">
        @switch (activeTab) {
          @case ('chat') {
            <app-chat (pizzaSelected)="onPizzaSelected($event)"></app-chat>
          }
          @case ('menu') {
            <app-menu (addToCart)="onAddToCart($event)"></app-menu>
          }
          @case ('order') {
            <app-order></app-order>
          }
        }
      </main>

      <footer class="app-footer">
        <p>🍕 KernelMind - Pizzaria Inteligente com IA</p>
      </footer>
    </div>
  `,
  styles: [`
    .app-container {
      min-height: 100vh;
      display: flex;
      flex-direction: column;
      background: linear-gradient(135deg, #f5f5f5 0%, #e0e0e0 100%);
    }

    .app-nav {
      background: linear-gradient(135deg, #d32f2f 0%, #b71c1c 100%);
      color: white;
      padding: 16px 24px;
      display: flex;
      justify-content: space-between;
      align-items: center;
      box-shadow: 0 2px 12px rgba(0,0,0,0.15);
      position: sticky;
      top: 0;
      z-index: 100;
    }

    .nav-brand {
      display: flex;
      align-items: center;
      gap: 12px;
    }

    .brand-emoji {
      font-size: 32px;
    }

    .brand-name {
      font-size: 24px;
      font-weight: 700;
    }

    .nav-tabs {
      display: flex;
      gap: 8px;
    }

    .nav-tabs button {
      background: rgba(255,255,255,0.15);
      border: 1px solid rgba(255,255,255,0.3);
      color: white;
      padding: 12px 24px;
      border-radius: 25px;
      font-size: 16px;
      cursor: pointer;
      transition: all 0.2s;
      display: flex;
      align-items: center;
      gap: 8px;
    }

    .nav-tabs button:hover {
      background: rgba(255,255,255,0.25);
    }

    .nav-tabs button.active {
      background: white;
      color: #d32f2f;
      font-weight: 600;
    }

    .badge {
      background: #ffeb3b;
      color: #333;
      padding: 2px 8px;
      border-radius: 12px;
      font-size: 12px;
      font-weight: 700;
    }

    .app-content {
      flex: 1;
      padding: 20px;
    }

    .app-footer {
      text-align: center;
      padding: 20px;
      color: #999;
      font-size: 14px;
      background: white;
      border-top: 1px solid #eee;
    }

    .app-footer p {
      margin: 0;
    }

    @media (max-width: 768px) {
      .app-nav {
        flex-direction: column;
        gap: 16px;
      }

      .nav-tabs {
        width: 100%;
        justify-content: center;
      }

      .nav-tabs button {
        flex: 1;
        justify-content: center;
        padding: 10px 16px;
        font-size: 14px;
      }
    }
  `]
})
export class AppComponent {
  activeTab: 'chat' | 'menu' | 'order' = 'chat';

  private readonly orderState = inject(OrderStateService);

  get orderItemCount(): number {
    return this.orderState.itemCount;
  }

  onPizzaSelected(pizza: Pizza): void {
    this.orderState.addItem(pizza);
  }

  onAddToCart(pizza: Pizza): void {
    this.orderState.addItem(pizza);
    this.activeTab = 'order';
  }
}
