import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { OrderItem, Pizza } from '../models';

@Injectable({
  providedIn: 'root'
})
export class OrderStateService {
  private readonly itemsSubject = new BehaviorSubject<OrderItem[]>([]);
  readonly items$ = this.itemsSubject.asObservable();

  get items(): OrderItem[] {
    return this.itemsSubject.value;
  }

  get itemCount(): number {
    return this.items.reduce((sum, item) => sum + item.quantity, 0);
  }

  get subtotal(): number {
    return this.items.reduce((sum, item) => sum + item.subtotal, 0);
  }

  addItem(pizza: Pizza, quantity = 1): void {
    const items = [...this.items];
    const existing = items.find(i => i.pizzaId === pizza.id);

    if (existing) {
      const updated = { ...existing };
      updated.quantity += quantity;
      updated.subtotal = updated.quantity * updated.unitPrice;
      const index = items.indexOf(existing);
      items[index] = updated;
    } else {
      items.push({
        id: Date.now().toString(),
        pizzaId: pizza.id,
        pizzaName: pizza.name,
        quantity,
        unitPrice: pizza.price,
        subtotal: pizza.price * quantity
      });
    }

    this.itemsSubject.next(items);
  }

  removeItem(index: number): void {
    const items = [...this.items];
    if (index < 0 || index >= items.length) {
      return;
    }
    items.splice(index, 1);
    this.itemsSubject.next(items);
  }

  clear(): void {
    this.itemsSubject.next([]);
  }
}

