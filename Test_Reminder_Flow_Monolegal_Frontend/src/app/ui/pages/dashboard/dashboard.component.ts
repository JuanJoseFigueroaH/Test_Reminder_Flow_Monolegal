import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { Subscription, interval } from 'rxjs';
import { CommonModule } from '@angular/common';
import { CardComponent } from '../../molecules/card/card.component';
import { StatCardComponent } from '../../molecules/stat-card/stat-card.component';
import { ButtonComponent } from '../../atoms/button/button.component';
import { SpinnerComponent } from '../../atoms/spinner/spinner.component';
import { InvoiceTableComponent } from '../../organisms/invoice-table/invoice-table.component';
import { CreateClientModalComponent } from '../../organisms/create-client-modal/create-client-modal.component';
import { CreateInvoiceModalComponent } from '../../organisms/create-invoice-modal/create-invoice-modal.component';
import { InvoiceAdapter } from '../../../infrastructure/adapters/invoice.adapter';
import { ClientAdapter } from '../../../infrastructure/adapters/client.adapter';
import { Invoice } from '../../../domain/entities/invoice.entity';
import { Client } from '../../../domain/entities/client.entity';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    CardComponent,
    StatCardComponent,
    ButtonComponent,
    SpinnerComponent,
    InvoiceTableComponent,
    CreateClientModalComponent,
    CreateInvoiceModalComponent
  ],
  template: `
    <div class="dashboard">
      <div class="dashboard-header">
        <h2 class="page-title">Dashboard</h2>
        <div class="header-actions">
          <app-button variant="primary" (onClick)="showClientModal = true">
            + Nuevo Cliente
          </app-button>
          <app-button variant="success" (onClick)="showInvoiceModal = true">
            + Nueva Factura
          </app-button>
        </div>
      </div>

      <div class="info-banner" *ngIf="clients.length > 0">
        <span>⏰</span> Los recordatorios se procesan automáticamente cada 2 minutos
      </div>

      <div class="stats-grid" *ngIf="!loading">
        <app-stat-card
          icon="📄"
          iconBg="#e0f2fe"
          label="Total Facturas"
          [value]="totalInvoices"
        ></app-stat-card>
        <app-stat-card
          icon="🕐"
          iconBg="#f0fdf4"
          label="Pendientes"
          [value]="pendientes"
        ></app-stat-card>
        <app-stat-card
          icon="📋"
          iconBg="#dbeafe"
          label="Primer Recordatorio"
          [value]="primerRecordatorio"
        ></app-stat-card>
        <app-stat-card
          icon="⚠️"
          iconBg="#fef3c7"
          label="Segundo Recordatorio"
          [value]="segundoRecordatorio"
        ></app-stat-card>
        <app-stat-card
          icon="⛔"
          iconBg="#fee2e2"
          label="Desactivados"
          [value]="desactivados"
        ></app-stat-card>
      </div>

      <app-spinner *ngIf="loading"></app-spinner>

      <app-card title="Resumen de Facturas" *ngIf="!loading">
        <app-invoice-table [invoices]="invoices"></app-invoice-table>
      </app-card>
    </div>

    <app-create-client-modal 
      *ngIf="showClientModal" 
      (close)="showClientModal = false"
      (created)="createClient($event)">
    </app-create-client-modal>

    <app-create-invoice-modal 
      *ngIf="showInvoiceModal" 
      [clients]="clients"
      (close)="showInvoiceModal = false"
      (created)="createInvoice($event)">
    </app-create-invoice-modal>
  `,
  styles: [`
    .dashboard {
      display: flex;
      flex-direction: column;
      gap: 1.5rem;
    }

    .dashboard-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      flex-wrap: wrap;
      gap: 1rem;
    }

    .page-title {
      font-size: 1.5rem;
      font-weight: 600;
      color: var(--text-primary);
      margin: 0;
    }

    .header-actions {
      display: flex;
      gap: 0.75rem;
    }

    .stats-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 1rem;
    }

    .process-result {
      margin-top: 1rem;
    }

    .result-message {
      font-weight: 500;
      margin-bottom: 1rem;
    }

    .result-list {
      list-style: none;
      padding: 0;
      margin: 0;
    }

    .result-list li {
      padding: 0.5rem 0;
      border-bottom: 1px solid var(--border-color);
    }

    .info-banner {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      padding: 0.75rem 1rem;
      background: linear-gradient(135deg, #dbeafe 0%, #e0f2fe 100%);
      border-radius: 8px;
      color: #1e40af;
      font-size: 0.875rem;
      font-weight: 500;
    }
  `]
})
export class DashboardComponent implements OnInit, OnDestroy {
  private invoiceAdapter = inject(InvoiceAdapter);
  private clientAdapter = inject(ClientAdapter);
  private refreshSubscription?: Subscription;

  invoices: Invoice[] = [];
  clients: Client[] = [];
  loading = true;
  showClientModal = false;
  showInvoiceModal = false;

  totalInvoices = 0;
  pendientes = 0;
  primerRecordatorio = 0;
  segundoRecordatorio = 0;
  desactivados = 0;

  ngOnInit(): void {
    this.loadData();
    this.startAutoRefresh();
  }

  ngOnDestroy(): void {
    this.stopAutoRefresh();
  }

  private startAutoRefresh(): void {
    this.refreshSubscription = interval(30000).subscribe(() => {
      this.refreshData();
    });
  }

  private stopAutoRefresh(): void {
    if (this.refreshSubscription) {
      this.refreshSubscription.unsubscribe();
    }
  }

  private refreshData(): void {
    this.loadInvoices();
    this.loadClients();
  }

  loadData(): void {
    this.loading = true;
    this.loadInvoices();
    this.loadClients();
  }

  loadInvoices(): void {
    this.invoiceAdapter.getAll().subscribe({
      next: (response) => {
        if (response.data) {
          this.invoices = response.data.invoices;
          this.calculateStats();
        }
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  loadClients(): void {
    this.clientAdapter.getAll().subscribe({
      next: (response) => {
        if (response.data) {
          this.clients = response.data.clients;
        }
      }
    });
  }

  calculateStats(): void {
    this.totalInvoices = this.invoices.length;
    this.pendientes = this.invoices.filter(i => i.status === 'pendiente').length;
    this.primerRecordatorio = this.invoices.filter(i => i.status === 'primerrecordatorio').length;
    this.segundoRecordatorio = this.invoices.filter(i => i.status === 'segundorecordatorio').length;
    this.desactivados = this.invoices.filter(i => i.status === 'desactivado').length;
  }

  createClient(clientData: any): void {
    this.clientAdapter.create(clientData).subscribe({
      next: () => {
        this.showClientModal = false;
        this.loadClients();
      }
    });
  }

  createInvoice(invoiceData: any): void {
    this.invoiceAdapter.create(invoiceData).subscribe({
      next: () => {
        this.showInvoiceModal = false;
        this.loadInvoices();
      }
    });
  }
}
