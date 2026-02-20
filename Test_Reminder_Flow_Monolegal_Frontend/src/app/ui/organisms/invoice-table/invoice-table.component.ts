import { Component, Input } from '@angular/core';
import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { Invoice } from '../../../domain/entities/invoice.entity';
import { StatusBadgeComponent } from '../../molecules/status-badge/status-badge.component';

@Component({
  selector: 'app-invoice-table',
  standalone: true,
  imports: [CommonModule, StatusBadgeComponent, CurrencyPipe, DatePipe],
  template: `
    <div class="table-container">
      <table class="table">
        <thead>
          <tr>
            <th>Número</th>
            <th>Cliente</th>
            <th>Email</th>
            <th>Monto</th>
            <th>Vencimiento</th>
            <th>Estado</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let invoice of invoices">
            <td>{{ invoice.invoice_number }}</td>
            <td>{{ invoice.client_name || 'N/A' }}</td>
            <td>{{ invoice.client_email || 'N/A' }}</td>
            <td>{{ invoice.amount | currency:'COP':'symbol-narrow':'1.0-0' }}</td>
            <td>{{ invoice.due_date | date:'dd/MM/yyyy' }}</td>
            <td>
              <app-status-badge [status]="invoice.status"></app-status-badge>
            </td>
          </tr>
          <tr *ngIf="invoices.length === 0">
            <td colspan="6" class="empty-state">No hay facturas disponibles</td>
          </tr>
        </tbody>
      </table>
    </div>
  `,
  styles: [`
    .table-container {
      overflow-x: auto;
    }

    .empty-state {
      text-align: center;
      padding: 2rem !important;
      color: var(--text-secondary);
    }
  `]
})
export class InvoiceTableComponent {
  @Input() invoices: Invoice[] = [];
}
