import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div style="padding: 20px; max-width: 800px; margin: 0 auto;">
      <h1 style="color: #d32f2f; margin-bottom: 20px;">🍕 KernelMind</h1>
      <p>Pizzaria Inteligente com IA</p>
      <p style="color: #666; margin-top: 20px;">
        Status: <strong>Em desenvolvimento</strong>
      </p>
      <div style="margin-top: 30px; padding: 20px; background: #e3f2fd; border-radius: 8px;">
        <h3>Próximos passos:</h3>
        <ul style="margin-top: 10px; padding-left: 20px;">
          <li>Implementar componente de chat</li>
          <li>Conectar com API backend</li>
          <li>Adicionar interface do cardápio</li>
        </ul>
      </div>
    </div>
  `
})
export class AppComponent {
  title = 'KernelMind';
}
