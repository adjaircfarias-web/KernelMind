import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Pizza, Order, Customer, ChatRequest, ChatResponse, OrderStatus } from '../models';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  // Menu endpoints
  getMenu(): Observable<Pizza[]> {
    return this.http.get<Pizza[]>(`${this.baseUrl}/menu`);
  }

  getPizza(id: string): Observable<Pizza> {
    return this.http.get<Pizza>(`${this.baseUrl}/menu/${id}`);
  }

  searchPizzas(query: string): Observable<Pizza[]> {
    const params = new HttpParams().set('query', query);
    return this.http.get<Pizza[]>(`${this.baseUrl}/menu/search`, { params });
  }

  semanticSearch(query: string, threshold = 0.5, maxResults = 5): Observable<Pizza[]> {
    const params = new HttpParams()
      .set('query', query)
      .set('threshold', threshold.toString())
      .set('maxResults', maxResults.toString());
    return this.http.get<Pizza[]>(`${this.baseUrl}/menu/semantic-search`, { params });
  }

  getCategories(): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/menu/categories`);
  }

  getPizzasByCategory(category: string): Observable<Pizza[]> {
    return this.http.get<Pizza[]>(`${this.baseUrl}/menu/category/${category}`);
  }

  // Order endpoints
  getOrders(): Observable<Order[]> {
    return this.http.get<Order[]>(`${this.baseUrl}/orders`);
  }

  getOrder(id: string): Observable<Order> {
    return this.http.get<Order>(`${this.baseUrl}/orders/${id}`);
  }

  createOrder(order: Partial<Order>): Observable<Order> {
    return this.http.post<Order>(`${this.baseUrl}/orders`, order);
  }

  updateOrderStatus(id: string, status: OrderStatus): Observable<Order> {
    return this.http.patch<Order>(`${this.baseUrl}/orders/${id}/status`, { status });
  }

  cancelOrder(id: string): Observable<Order> {
    return this.http.post<Order>(`${this.baseUrl}/orders/${id}/cancel`, {});
  }

  getOrderTotal(id: string): Observable<{ subtotal: number; deliveryFee: number; total: number }> {
    return this.http.get<{ subtotal: number; deliveryFee: number; total: number }>(
      `${this.baseUrl}/orders/${id}/total`
    );
  }

  // Customer endpoints
  getCustomers(): Observable<Customer[]> {
    return this.http.get<Customer[]>(`${this.baseUrl}/customers`);
  }

  getCustomer(id: string): Observable<Customer> {
    return this.http.get<Customer>(`${this.baseUrl}/customers/${id}`);
  }

  createCustomer(customer: Partial<Customer>): Observable<Customer> {
    return this.http.post<Customer>(`${this.baseUrl}/customers`, customer);
  }

  // Chat endpoints
  sendMessage(request: ChatRequest): Observable<ChatResponse> {
    return this.http.post<ChatResponse>(`${this.baseUrl}/chat/message`, request);
  }

  streamMessage(request: ChatRequest): Observable<string> {
    return this.http.post(`${this.baseUrl}/chat/stream/raw`, request, {
      responseType: 'text'
    });
  }

  healthCheck(): Observable<{ status: string; service: string; timestamp: Date; version: string }> {
    return this.http.get<{ status: string; service: string; timestamp: Date; version: string }>(
      `${this.baseUrl}/chat/health`
    );
  }
}
