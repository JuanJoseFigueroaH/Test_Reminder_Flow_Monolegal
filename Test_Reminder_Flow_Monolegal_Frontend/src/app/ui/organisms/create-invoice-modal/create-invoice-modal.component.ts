import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ButtonComponent } from '../../atoms/button/button.component';
import { Client } from '../../../domain/entities/client.entity';

@Component({
  selector: 'app-create-invoice-modal',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent],
  template: `
    <div class="modal-overlay" (click)="close.emit()">
      <div class="modal-content" (click)="$event.stopPropagation()">
        <div class="modal-header">
          <h3>Nueva Factura</h3>
          <button class="close-btn" (click)="close.emit()">&times;</button>
        </div>
        <form (ngSubmit)="onSubmit()">
          <div class="form-group">
            <label for="client">Cliente *</label>
            <select id="client" [(ngModel)]="invoice.client_id" name="client_id" required>
              <option value="">Seleccionar cliente</option>
              <option *ngFor="let client of clients" [value]="client.id">{{ client.name }}</option>
            </select>
          </div>
          <div class="form-group">
            <label for="invoice_number">Número de Factura *</label>
            <input type="text" id="invoice_number" [(ngModel)]="invoice.invoice_number" name="invoice_number" required placeholder="INV-001">
          </div>
          <div class="form-group">
            <label for="amount">Monto (COP) *</label>
            <input type="number" id="amount" [(ngModel)]="invoice.amount" name="amount" required min="0" step="1000" placeholder="150000">
          </div>
          <div class="modal-actions">
            <app-button variant="outline" type="button" (onClick)="close.emit()">Cancelar</app-button>
            <app-button variant="primary" type="submit" [disabled]="!isValid()">Crear Factura</app-button>
          </div>
        </form>
      </div>
    </div>
  `,
  styles: [`
    .modal-overlay {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background: rgba(0, 0, 0, 0.5);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1000;
    }

    .modal-content {
      background: white;
      border-radius: 12px;
      width: 100%;
      max-width: 450px;
      box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1);
    }

    .modal-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 1.25rem 1.5rem;
      border-bottom: 1px solid #e2e8f0;
    }

    .modal-header h3 {
      margin: 0;
      font-size: 1.25rem;
      font-weight: 600;
      color: #1e293b;
    }

    .close-btn {
      background: none;
      border: none;
      font-size: 1.5rem;
      cursor: pointer;
      color: #64748b;
      padding: 0;
      line-height: 1;
    }

    .close-btn:hover {
      color: #1e293b;
    }

    form {
      padding: 1.5rem;
    }

    .form-group {
      margin-bottom: 1rem;
    }

    .form-group label {
      display: block;
      font-size: 0.875rem;
      font-weight: 500;
      color: #374151;
      margin-bottom: 0.5rem;
    }

    .form-group input,
    .form-group select {
      width: 100%;
      padding: 0.75rem 1rem;
      border: 1px solid #e2e8f0;
      border-radius: 8px;
      font-size: 0.875rem;
      transition: border-color 0.2s, box-shadow 0.2s;
      box-sizing: border-box;
      background: white;
    }

    .form-group input:focus,
    .form-group select:focus {
      outline: none;
      border-color: #3b82f6;
      box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
    }

    .modal-actions {
      display: flex;
      justify-content: flex-end;
      gap: 0.75rem;
      margin-top: 1.5rem;
      padding-top: 1rem;
      border-top: 1px solid #e2e8f0;
    }
  `]
})
export class CreateInvoiceModalComponent implements OnInit {
  @Input() clients: Client[] = [];
  @Output() close = new EventEmitter<void>();
  @Output() created = new EventEmitter<any>();

  invoice = {
    client_id: '',
    invoice_number: '',
    amount: 0
  };

  ngOnInit(): void {}

  isValid(): boolean {
    return this.invoice.client_id !== '' && 
           this.invoice.invoice_number.trim() !== '' && 
           this.invoice.amount > 0;
  }

  onSubmit(): void {
    if (this.isValid()) {
      this.created.emit(this.invoice);
    }
  }
}
