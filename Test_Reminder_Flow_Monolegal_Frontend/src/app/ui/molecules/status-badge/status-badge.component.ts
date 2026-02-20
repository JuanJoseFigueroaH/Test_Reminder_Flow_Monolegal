import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BadgeComponent } from '../../atoms/badge/badge.component';
import { InvoiceStatus } from '../../../domain/entities/invoice.entity';

@Component({
  selector: 'app-status-badge',
  standalone: true,
  imports: [CommonModule, BadgeComponent],
  template: `
    <app-badge [text]="getStatusText()" [variant]="getVariant()"></app-badge>
  `
})
export class StatusBadgeComponent {
  @Input() status: InvoiceStatus = 'pendiente';

  getStatusText(): string {
    const statusMap: Record<InvoiceStatus, string> = {
      'pendiente': 'Pendiente',
      'primerrecordatorio': 'Primer Recordatorio',
      'segundorecordatorio': 'Segundo Recordatorio',
      'desactivado': 'Desactivado'
    };
    return statusMap[this.status] || this.status;
  }

  getVariant(): 'pendiente' | 'primer' | 'segundo' | 'desactivado' {
    const variantMap: Record<InvoiceStatus, 'pendiente' | 'primer' | 'segundo' | 'desactivado'> = {
      'pendiente': 'pendiente',
      'primerrecordatorio': 'primer',
      'segundorecordatorio': 'segundo',
      'desactivado': 'desactivado'
    };
    return variantMap[this.status] || 'pendiente';
  }
}
