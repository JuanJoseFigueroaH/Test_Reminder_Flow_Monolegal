import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./ui/pages/dashboard/dashboard.component').then(m => m.DashboardComponent)
  },
  {
    path: 'invoices',
    loadComponent: () => import('./ui/pages/invoices/invoices.component').then(m => m.InvoicesComponent)
  },
  {
    path: 'clients',
    loadComponent: () => import('./ui/pages/clients/clients.component').then(m => m.ClientsComponent)
  }
];
