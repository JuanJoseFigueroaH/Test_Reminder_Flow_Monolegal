import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardComponent } from '../../molecules/card/card.component';
import { SpinnerComponent } from '../../atoms/spinner/spinner.component';
import { ClientAdapter } from '../../../infrastructure/adapters/client.adapter';
import { Client } from '../../../domain/entities/client.entity';

@Component({
  selector: 'app-clients',
  standalone: true,
  imports: [CommonModule, CardComponent, SpinnerComponent],
  template: `
    <div class="clients-page">
      <div class="page-header">
        <h2 class="page-title">Clientes</h2>
      </div>

      <app-spinner *ngIf="loading"></app-spinner>

      <app-card title="Listado de Clientes" *ngIf="!loading">
        <div class="table-container">
          <table class="table">
            <thead>
              <tr>
                <th>Nombre</th>
                <th>Email</th>
                <th>Teléfono</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let client of clients">
                <td>{{ client.name }}</td>
                <td>{{ client.email }}</td>
                <td>{{ client.phone || 'N/A' }}</td>
              </tr>
              <tr *ngIf="clients.length === 0">
                <td colspan="3" class="empty-state">No hay clientes disponibles</td>
              </tr>
            </tbody>
          </table>
        </div>
      </app-card>
    </div>
  `,
  styles: [`
    .clients-page {
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
export class ClientsComponent implements OnInit {
  private clientAdapter = inject(ClientAdapter);

  clients: Client[] = [];
  loading = true;

  ngOnInit(): void {
    this.loadClients();
  }

  loadClients(): void {
    this.loading = true;
    this.clientAdapter.getAll().subscribe({
      next: (response) => {
        if (response.data) {
          this.clients = response.data.clients;
        }
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }
}
