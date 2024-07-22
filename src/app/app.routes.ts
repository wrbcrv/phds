import { Routes } from '@angular/router';
import { TicketComponent } from './components/ticket/ticket.component';
import { LoginComponent } from './components/login/login.component';

export const routes: Routes = [
  {
    path: '',
    component: LoginComponent,
    title: 'Entrar · PHDS'
  },
  {
    path: 'create-ticket',
    component: TicketComponent,
    title: 'Novo chamado · PHDS'
  }
];
