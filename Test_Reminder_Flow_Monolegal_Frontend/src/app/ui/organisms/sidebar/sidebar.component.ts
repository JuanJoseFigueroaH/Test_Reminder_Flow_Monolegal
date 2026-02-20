import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <aside class="sidebar">
      <div class="sidebar-header">
        <span class="logo">RF</span>
        <span class="logo-text">Reminder Flow</span>
      </div>
      <nav class="sidebar-nav">
        <a routerLink="/dashboard" routerLinkActive="active" class="nav-item">
          <span class="nav-icon">📊</span>
          <span class="nav-text">Dashboard</span>
        </a>
        <a routerLink="/invoices" routerLinkActive="active" class="nav-item">
          <span class="nav-icon">📄</span>
          <span class="nav-text">Facturas</span>
        </a>
        <a routerLink="/clients" routerLinkActive="active" class="nav-item">
          <span class="nav-icon">👥</span>
          <span class="nav-text">Clientes</span>
        </a>
      </nav>
    </aside>
  `,
  styles: [`
    .sidebar {
      position: fixed;
      left: 0;
      top: 0;
      bottom: 0;
      width: 250px;
      background: var(--card-background);
      border-right: 1px solid var(--border-color);
      display: flex;
      flex-direction: column;
    }

    .sidebar-header {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      padding: 1.25rem;
      border-bottom: 1px solid var(--border-color);
    }

    .logo {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 2.5rem;
      height: 2.5rem;
      background: var(--primary-color);
      color: white;
      font-weight: 700;
      border-radius: var(--radius-md);
    }

    .logo-text {
      font-weight: 600;
      color: var(--text-primary);
    }

    .sidebar-nav {
      padding: 1rem;
      display: flex;
      flex-direction: column;
      gap: 0.25rem;
    }

    .nav-item {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      padding: 0.75rem 1rem;
      border-radius: var(--radius-md);
      color: var(--text-secondary);
      text-decoration: none;
      transition: all 0.2s ease;
    }

    .nav-item:hover {
      background: var(--background-color);
      color: var(--text-primary);
    }

    .nav-item.active {
      background: rgba(59, 130, 246, 0.1);
      color: var(--primary-color);
    }

    .nav-icon {
      font-size: 1.125rem;
    }

    .nav-text {
      font-size: 0.875rem;
      font-weight: 500;
    }

    @media (max-width: 768px) {
      .sidebar {
        display: none;
      }
    }
  `]
})
export class SidebarComponent {}
