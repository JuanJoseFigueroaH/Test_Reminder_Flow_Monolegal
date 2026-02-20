import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-stat-card',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="stat-card">
      <div class="stat-icon" [style.background-color]="iconBg">
        <span>{{ icon }}</span>
      </div>
      <div class="stat-content">
        <span class="stat-label">{{ label }}</span>
        <span class="stat-value">{{ value }}</span>
      </div>
    </div>
  `,
  styles: [`
    .stat-card {
      display: flex;
      align-items: center;
      gap: 1rem;
      padding: 1.25rem;
      background: var(--card-background);
      border-radius: var(--radius-lg);
      box-shadow: var(--shadow-sm);
      border: 1px solid var(--border-color);
    }

    .stat-icon {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 3rem;
      height: 3rem;
      border-radius: var(--radius-md);
      font-size: 1.25rem;
    }

    .stat-content {
      display: flex;
      flex-direction: column;
    }

    .stat-label {
      font-size: 0.875rem;
      color: var(--text-secondary);
    }

    .stat-value {
      font-size: 1.5rem;
      font-weight: 600;
      color: var(--text-primary);
    }
  `]
})
export class StatCardComponent {
  @Input() icon = '';
  @Input() iconBg = '#e0f2fe';
  @Input() label = '';
  @Input() value: string | number = 0;
}
