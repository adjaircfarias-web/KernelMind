import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { Pizza } from '../../models';

@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="menu-container">
      <div class="menu-header">
        <h2>🍕 Cardápio</h2>
        <div class="search-box">
          <input
            type="text"
            [(ngModel)]="searchQuery"
            (ngModelChange)="onSearch()"
            placeholder="Buscar pizzas..."
            class="search-input"
          />
        </div>
      </div>

      <div class="menu-content">
        @if (isLoading) {
          <div class="loading">Carregando cardápio...</div>
        } @else {
          @for (category of categories; track category) {
            <div class="category-section">
              <h3 class="category-title">{{ category }}</h3>
              <div class="pizzas-grid">
                @for (pizza of getPizzasByCategory(category); track pizza.id) {
                  <div class="pizza-card" (click)="selectPizza(pizza)">
                    <div class="pizza-image">🍕</div>
                    <div class="pizza-details">
                      <h4>{{ pizza.name }}</h4>
                      <p class="pizza-description">{{ pizza.description }}</p>
                      <div class="pizza-footer">
                        <span class="pizza-price">{{ pizza.price | currency:'BRL' }}</span>
                        <button class="add-btn" (click)="addToOrder(pizza, $event)">
                          + Carrinho
                        </button>
                      </div>
                    </div>
                  </div>
                }
              </div>
            </div>
          }
        }
      </div>

      @if (selectedPizza) {
        <div class="pizza-modal" (click)="closeModal()">
          <div class="modal-content" (click)="$event.stopPropagation()">
            <button class="close-btn" (click)="closeModal()">✕</button>
            <div class="modal-header">
              <span class="pizza-emoji">🍕</span>
              <h3>{{ selectedPizza.name }}</h3>
            </div>
            <div class="modal-body">
              <p class="description">{{ selectedPizza.description }}</p>
              <div class="ingredients">
                <h4>Ingredientes:</h4>
                <ul>
                  @for (ingredient of selectedPizza.ingredients; track ingredient) {
                    <li>{{ ingredient }}</li>
                  }
                </ul>
              </div>
              <div class="price-section">
                <span class="price">{{ selectedPizza.price | currency:'BRL' }}</span>
              </div>
            </div>
            <div class="modal-footer">
              <button class="add-to-cart-btn" (click)="addToOrder(selectedPizza, $event)">
                Adicionar ao Pedido
              </button>
            </div>
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    .menu-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 20px;
    }

    .menu-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 24px;
      flex-wrap: wrap;
      gap: 16px;
    }

    .menu-header h2 {
      margin: 0;
      color: #333;
      font-size: 28px;
    }

    .search-box {
      flex: 1;
      max-width: 400px;
    }

    .search-input {
      width: 100%;
      padding: 12px 20px;
      border: 2px solid #eee;
      border-radius: 25px;
      font-size: 16px;
      transition: border-color 0.3s;
    }

    .search-input:focus {
      outline: none;
      border-color: #d32f2f;
    }

    .menu-content {
      min-height: 400px;
    }

    .loading {
      text-align: center;
      padding: 40px;
      color: #666;
      font-size: 18px;
    }

    .category-section {
      margin-bottom: 32px;
    }

    .category-title {
      color: #d32f2f;
      font-size: 22px;
      border-bottom: 3px solid #d32f2f;
      padding-bottom: 8px;
      margin-bottom: 16px;
    }

    .pizzas-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
      gap: 20px;
    }

    .pizza-card {
      background: white;
      border-radius: 12px;
      overflow: hidden;
      box-shadow: 0 2px 12px rgba(0,0,0,0.08);
      cursor: pointer;
      transition: transform 0.2s, box-shadow 0.2s;
    }

    .pizza-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 8px 24px rgba(0,0,0,0.12);
    }

    .pizza-image {
      height: 140px;
      background: linear-gradient(135deg, #fff5f5 0%, #ffe0e0 100%);
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 64px;
    }

    .pizza-details {
      padding: 16px;
    }

    .pizza-details h4 {
      margin: 0 0 8px;
      font-size: 18px;
      color: #333;
    }

    .pizza-description {
      font-size: 14px;
      color: #666;
      margin: 0 0 12px;
      line-height: 1.4;
    }

    .pizza-footer {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .pizza-price {
      font-size: 20px;
      font-weight: 700;
      color: #d32f2f;
    }

    .add-btn {
      background: #d32f2f;
      color: white;
      border: none;
      padding: 8px 16px;
      border-radius: 20px;
      font-size: 14px;
      cursor: pointer;
      transition: background 0.2s;
    }

    .add-btn:hover {
      background: #b71c1c;
    }

    .pizza-modal {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background: rgba(0,0,0,0.5);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1000;
      padding: 20px;
    }

    .modal-content {
      background: white;
      border-radius: 16px;
      max-width: 500px;
      width: 100%;
      max-height: 90vh;
      overflow-y: auto;
      position: relative;
    }

    .close-btn {
      position: absolute;
      top: 16px;
      right: 16px;
      width: 36px;
      height: 36px;
      border-radius: 50%;
      border: none;
      background: #f5f5f5;
      cursor: pointer;
      font-size: 18px;
    }

    .modal-header {
      padding: 24px 24px 16px;
      display: flex;
      align-items: center;
      gap: 12px;
    }

    .pizza-emoji {
      font-size: 48px;
    }

    .modal-header h3 {
      margin: 0;
      font-size: 24px;
    }

    .modal-body {
      padding: 0 24px;
    }

    .description {
      font-size: 16px;
      color: #555;
      line-height: 1.6;
      margin-bottom: 20px;
    }

    .ingredients h4 {
      margin: 0 0 12px;
      color: #333;
    }

    .ingredients ul {
      margin: 0;
      padding-left: 20px;
    }

    .ingredients li {
      margin: 8px 0;
      color: #555;
    }

    .price-section {
      margin: 24px 0;
      text-align: center;
    }

    .price {
      font-size: 32px;
      font-weight: 700;
      color: #d32f2f;
    }

    .modal-footer {
      padding: 16px 24px 24px;
    }

    .add-to-cart-btn {
      width: 100%;
      padding: 16px;
      background: #d32f2f;
      color: white;
      border: none;
      border-radius: 12px;
      font-size: 18px;
      font-weight: 600;
      cursor: pointer;
      transition: background 0.2s;
    }

    .add-to-cart-btn:hover {
      background: #b71c1c;
    }
  `]
})
export class MenuComponent implements OnInit {
  pizzas: Pizza[] = [];
  categories: string[] = [];
  searchQuery = '';
  selectedPizza: Pizza | null = null;
  isLoading = true;

  @Output() addToCart = new EventEmitter<Pizza>();

  constructor(private apiService: ApiService) {}

  ngOnInit(): void {
    this.loadMenu();
  }

  loadMenu(): void {
    this.isLoading = true;
    this.apiService.getMenu().subscribe({
      next: (pizzas) => {
        this.pizzas = pizzas;
        this.categories = [...new Set(pizzas.map(p => p.category))];
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading menu:', err);
        this.isLoading = false;
      }
    });
  }

  getPizzasByCategory(category: string): Pizza[] {
    return this.pizzas.filter(p => p.category === category);
  }

  onSearch(): void {
    if (this.searchQuery.trim()) {
      this.apiService.searchPizzas(this.searchQuery).subscribe({
        next: (pizzas) => {
          this.pizzas = pizzas;
          this.categories = [...new Set(pizzas.map(p => p.category))];
        }
      });
    } else {
      this.loadMenu();
    }
  }

  selectPizza(pizza: Pizza): void {
    this.selectedPizza = pizza;
  }

  closeModal(): void {
    this.selectedPizza = null;
  }

  addToOrder(pizza: Pizza, event: Event): void {
    event.stopPropagation();
    this.addToCart.emit(pizza);
    console.log('Adding to cart:', pizza.name);
    this.closeModal();
  }
}
