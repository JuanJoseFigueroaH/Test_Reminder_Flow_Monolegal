import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardComponent } from '../../molecules/card/card.component';
import { SpinnerComponent } from '../../atoms/spinner/spinner.component';
import { InvoiceTableComponent } from '../../organisms/invoice-table/invoice-table.component';
import { InvoiceAdapter } from '../../../infrastructure/adapters/invoice.adapter';
import { Invoice } from '../../../domain/entities/invoice.entity';

@Component({
  selector: 'app-invoices',
  standalone: true,
  imports: [CommonModule, CardComponent, SpinnerComponent, InvoiceTableComponent],
  template: `
    <div class="invoices-page">
      <div class="page-header">
        <h2 class="page-title">Facturas</h2>
      </div>

      <app-spinner *ngIf="loading"></app-spinner>

      <app-card title="Listado de Facturas" *ngIf="!loading">
        <app-invoice-table [invoices]="invoices"></app-invoice-table>
      </app-card>
    </div>
  `,
  styles: [`
    .invoices-page {
      display: flex;
      flex-direction: column;
      gap: 1.5rem;
    }

    .page-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .page-title {
      font-size: 1.5rem;
      font-weight: 600;
      color: var(--text-primary);
      margin: 0;
    }
  `]
})
export class InvoicesComponent implements OnInit {
  private invoiceAdapter = inject(InvoiceAdapter);

  invoices: Invoice[] = [];
  loading = true;

  ngOnInit(): void {
    this.loadInvoices();
  }

  loadInvoices(): void {
    this.loading = true;
    this.invoiceAdapter.getAll().subscribe({
      next: (response) => {
        if (response.data) {
          this.invoices = response.data.invoices;
        }
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }
}
