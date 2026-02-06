import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { ChatComponent } from './components/chat/chat.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, HttpClientModule, ChatComponent],
  template: `<app-chat></app-chat>`,
  styles: [`
    :host {
      display: block;
      min-height: 100vh;
      background: #e0e0e0;
      padding: 20px;
    }
  `]
})
export class AppComponent {
  title = 'KernelMind';
}
