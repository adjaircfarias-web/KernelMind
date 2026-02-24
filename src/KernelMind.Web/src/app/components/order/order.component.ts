import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { OrderStateService } from '../../services';
import { Order, OrderItem, OrderStatus, Pizza } from '../../models';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-order',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="order-container">
      <div class="order-header">
        <h2>📦 Meu Pedido</h2>
        @if (orderNumber) {
          <span class="order-number">#{{ orderNumber }}</span>
        }
      </div>

      @if (items.length === 0) {
        <div class="empty-cart">
          <span class="empty-icon">🛒</span>
          <p>Seu carrinho está vazio</p>
          <p class="hint">Adicione pizzas do cardápio para fazer um pedido!</p>
        </div>
      } @else {
        <div class="order-items">
          @for (item of items; track item.id; let i = $index) {
            <div class="order-item">
              <div class="item-info">
                <span class="item-name">{{ item.pizzaName }}</span>
                <span class="item-quantity">x{{ item.quantity }}</span>
              </div>
              <div class="item-price">{{ item.subtotal | currency:'BRL' }}</div>
              <button class="remove-btn" (click)="removeItem(i)">✕</button>
            </div>
          }
        </div>

        <div class="order-summary">
          <div class="summary-row">
            <span>Subtotal:</span>
            <span>{{ subtotal | currency:'BRL' }}</span>
          </div>
          <div class="summary-row">
            <span>Entrega:</span>
            <span>{{ deliveryFee | currency:'BRL' }}</span>
          </div>
          <div class="summary-row total">
            <span>Total:</span>
            <span>{{ total | currency:'BRL' }}</span>
          </div>
        </div>

        <div class="order-form">
          <div class="form-group">
            <label>Nome:</label>
            <input
              type="text"
              [(ngModel)]="customerName"
              placeholder="Seu nome"
              class="form-input"
            />
          </div>
          <div class="form-group">
            <label>Telefone:</label>
            <input
              type="tel"
              [(ngModel)]="phone"
              placeholder="(00) 00000-0000"
              class="form-input"
            />
          </div>
          <div class="form-group">
            <label>Endereço de entrega:</label>
            <textarea
              [(ngModel)]="address"
              placeholder="Rua, número, bairro, cidade..."
              class="form-input"
              rows="3"
            ></textarea>
          </div>
          <div class="form-group">
            <label>Observações:</label>
            <textarea
              [(ngModel)]="notes"
              placeholder="Alguma observação especial?"
              class="form-input"
              rows="2"
            ></textarea>
          </div>
        </div>

        <div class="order-actions">
          <button class="clear-btn" (click)="clearOrder()">
            Limpar Carrinho
          </button>
          <button
            class="confirm-btn"
            (click)="confirmOrder()"
            [disabled]="!canConfirm()"
          >
            Confirmar Pedido
          </button>
        </div>

        @if (orderStatus) {
          <div class="order-status" [class]="'status-' + orderStatus.toLowerCase()">
            <span class="status-icon">{{ getStatusIcon() }}</span>
            <span class="status-text">{{ getStatusText() }}</span>
          </div>
        }
      }
    </div>
  `,
  styles: [`
    .order-container {
      max-width: 500px;
      margin: 0 auto;
      padding: 20px;
      background: white;
      border-radius: 12px;
      box-shadow: 0 2px 12px rgba(0,0,0,0.08);
    }

    .order-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 24px;
      padding-bottom: 16px;
      border-bottom: 2px solid #f0f0f0;
    }

    .order-header h2 {
      margin: 0;
      color: #333;
      font-size: 24px;
    }

    .order-number {
      background: #d32f2f;
      color: white;
      padding: 8px 16px;
      border-radius: 20px;
      font-weight: 600;
    }

    .empty-cart {
      text-align: center;
      padding: 60px 20px;
      color: #999;
    }

    .empty-icon {
      font-size: 64px;
      display: block;
      margin-bottom: 16px;
    }

    .empty-cart p {
      margin: 8px 0;
      font-size: 18px;
    }

    .hint {
      font-size: 14px !important;
      color: #bbb !important;
    }

    .order-items {
      margin-bottom: 20px;
    }

    .order-item {
      display: flex;
      align-items: center;
      padding: 16px;
      background: #f9f9f9;
      border-radius: 12px;
      margin-bottom: 12px;
    }

    .item-info {
      flex: 1;
    }

    .item-name {
      font-weight: 600;
      color: #333;
      display: block;
    }

    .item-quantity {
      font-size: 14px;
      color: #666;
    }

    .item-price {
      font-weight: 600;
      color: #d32f2f;
      margin-right: 12px;
    }

    .remove-btn {
      width: 32px;
      height: 32px;
      border-radius: 50%;
      border: none;
      background: #ffebee;
      color: #d32f2f;
      cursor: pointer;
      font-size: 14px;
      transition: background 0.2s;
    }

    .remove-btn:hover {
      background: #ffcdd2;
    }

    .order-summary {
      padding: 20px 0;
      border-top: 2px solid #f0f0f0;
      margin-bottom: 20px;
    }

    .summary-row {
      display: flex;
      justify-content: space-between;
      padding: 8px 0;
      color: #666;
    }

    .summary-row.total {
      font-size: 20px;
      font-weight: 700;
      color: #333;
      padding-top: 16px;
      margin-top: 8px;
      border-top: 2px solid #eee;
    }

    .order-form {
      margin-bottom: 20px;
    }

    .form-group {
      margin-bottom: 16px;
    }

    .form-group label {
      display: block;
      margin-bottom: 6px;
      font-weight: 500;
      color: #555;
      font-size: 14px;
    }

    .form-input {
      width: 100%;
      padding: 12px;
      border: 2px solid #eee;
      border-radius: 8px;
      font-size: 16px;
      transition: border-color 0.2s;
      box-sizing: border-box;
    }

    .form-input:focus {
      outline: none;
      border-color: #d32f2f;
    }

    textarea.form-input {
      resize: vertical;
    }

    .order-actions {
      display: flex;
      gap: 12px;
    }

    .clear-btn {
      flex: 1;
      padding: 16px;
      border: 2px solid #ddd;
      border-radius: 12px;
      background: white;
      color: #666;
      font-size: 16px;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.2s;
    }

    .clear-btn:hover {
      border-color: #999;
      color: #333;
    }

    .confirm-btn {
      flex: 2;
      padding: 16px;
      border: none;
      border-radius: 12px;
      background: #d32f2f;
      color: white;
      font-size: 16px;
      font-weight: 600;
      cursor: pointer;
      transition: background 0.2s;
    }

    .confirm-btn:hover:not(:disabled) {
      background: #b71c1c;
    }

    .confirm-btn:disabled {
      background: #ccc;
      cursor: not-allowed;
    }

    .order-status {
      margin-top: 20px;
      padding: 16px;
      border-radius: 12px;
      display: flex;
      align-items: center;
      gap: 12px;
      animation: fadeIn 0.3s;
    }

    @keyframes fadeIn {
      from { opacity: 0; transform: translateY(-10px); }
      to { opacity: 1; transform: translateY(0); }
    }

    .status-confirmed {
      background: #e8f5e9;
      color: #2e7d32;
    }

    .status-preparing {
      background: #fff3e0;
      color: #ef6c00;
    }

    .status-ready {
      background: #e3f2fd;
      color: #1565c0;
    }

    .status-cancelled {
      background: #ffebee;
      color: #c62828;
    }

    .status-icon {
      font-size: 24px;
    }

    .status-text {
      font-weight: 600;
    }
  `]
})
export class OrderComponent implements OnInit, OnDestroy {
  items: OrderItem[] = [];
  orderNumber = '';
  orderStatus: OrderStatus | null = null;
  customerName = '';
  phone = '';
  address = '';
  notes = '';
  protected deliveryFee = 5.00;

  private itemsSub?: Subscription;

  constructor(
    private apiService: ApiService,
    private orderState: OrderStateService
  ) {}

  ngOnInit(): void {
    this.itemsSub = this.orderState.items$.subscribe(items => {
      this.items = items;
    });
  }

  ngOnDestroy(): void {
    this.itemsSub?.unsubscribe();
  }

  get subtotal(): number {
    return this.orderState.subtotal;
  }

  get total(): number {
    return this.subtotal + this.deliveryFee;
  }

  removeItem(index: number): void {
    this.orderState.removeItem(index);
  }

  clearOrder(): void {
    this.orderState.clear();
    this.orderNumber = '';
    this.orderStatus = null;
  }

  canConfirm(): boolean {
    return this.items.length > 0 &&
           this.customerName.trim() !== '' &&
           this.address.trim() !== '';
  }

  confirmOrder(): void {
    if (!this.canConfirm()) return;

    const order: Partial<Order> = {
      customerId: this.customerName,
      deliveryAddress: this.address,
      notes: this.notes,
      items: this.items
    };

    this.apiService.createOrder(order).subscribe({
      next: (createdOrder) => {
        this.orderNumber = createdOrder.id.substring(0, 8).toUpperCase();
        this.orderStatus = OrderStatus.Pending;
        this.orderState.clear();
        this.customerName = '';
        this.phone = '';
        this.address = '';
        this.notes = '';
      },
      error: (err) => {
        console.error('Error creating order:', err);
      }
    });
  }

  getStatusIcon(): string {
    switch (this.orderStatus) {
      case OrderStatus.Pending: return '⏳';
      case OrderStatus.Confirmed: return '✅';
      case OrderStatus.Preparing: return '👨‍🍳';
      case OrderStatus.Ready: return '🍕';
      case OrderStatus.OutForDelivery: return '🚚';
      case OrderStatus.Delivered: return '🎉';
      case OrderStatus.Cancelled: return '❌';
      default: return '📦';
    }
  }

  getStatusText(): string {
    switch (this.orderStatus) {
      case OrderStatus.Pending: return 'Pedido criado! Aguardando confirmação...';
      case OrderStatus.Confirmed: return 'Pedido confirmado!';
      case OrderStatus.Preparing: return 'Preparando seu pedido...';
      case OrderStatus.Ready: return 'Seu pedido está pronto!';
      case OrderStatus.OutForDelivery: return 'Saiu para entrega!';
      case OrderStatus.Delivered: return 'Pedido entregue! Bom apetite! 🍕';
      case OrderStatus.Cancelled: return 'Pedido cancelado';
      default: return '';
    }
  }
}
