import { Routes } from '@angular/router';
import { TicketComponent } from './components/ticket/ticket.component';
import { LoginComponent } from './components/login/login.component';

export const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'ticket/new',
    component: TicketComponent
  }
];
