import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule],
  template: `
    <header class="header">
      <div class="header-content">
        <h1 class="header-title">Reminder Flow</h1>
        <div class="header-actions">
          <span class="header-date">{{ currentDate | date:'fullDate' }}</span>
        </div>
      </div>
    </header>
  `,
  styles: [`
    .header {
      background: var(--card-background);
      border-bottom: 1px solid var(--border-color);
      padding: 1rem 1.5rem;
    }

    .header-content {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .header-title {
      font-size: 1.25rem;
      font-weight: 600;
      color: var(--text-primary);
      margin: 0;
    }

    .header-date {
      font-size: 0.875rem;
      color: var(--text-secondary);
    }
  `]
})
export class HeaderComponent {
  currentDate = new Date();
}
