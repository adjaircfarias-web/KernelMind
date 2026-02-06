export interface Pizza {
  id: string;
  name: string;
  description: string;
  price: number;
  category: string;
  ingredients: string[];
  isAvailable: boolean;
}

export interface Order {
  id: string;
  customerId: string;
  status: OrderStatus;
  totalAmount: number;
  deliveryAddress?: string;
  notes?: string;
  items: OrderItem[];
  createdAt: Date;
}

export interface OrderItem {
  id: string;
  pizzaId: string;
  pizzaName: string;
  quantity: number;
  unitPrice: number;
  subtotal: number;
  notes?: string;
}

export enum OrderStatus {
  Pending = 'Pending',
  Confirmed = 'Confirmed',
  Preparing = 'Preparing',
  Ready = 'Ready',
  OutForDelivery = 'OutForDelivery',
  Delivered = 'Delivered',
  Cancelled = 'Cancelled'
}

export interface ChatMessage {
  role: 'user' | 'assistant' | 'system';
  content: string;
  timestamp: Date;
}

export interface ChatRequest {
  message: string;
  sessionId?: string;
}

export interface ChatResponse {
  content: string;
  sessionId: string;
  timestamp: Date;
}

export interface Customer {
  id: string;
  name: string;
  phone?: string;
  email: string;
  address?: string;
  createdAt: Date;
}

export interface MenuCategory {
  name: string;
  pizzas: Pizza[];
}
