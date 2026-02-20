import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-badge',
  standalone: true,
  imports: [CommonModule],
  template: `
    <span [class]="'badge badge-' + variant">
      {{ text }}
    </span>
  `
})
export class BadgeComponent {
  @Input() text = '';
  @Input() variant: 'pendiente' | 'primer' | 'segundo' | 'desactivado' = 'pendiente';
}
