import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { TicketDetailsComponent } from './components/tickets/ticket-details/ticket-details.component';
import { TicketFormComponent } from './components/tickets/ticket-form/ticket-form.component';
import { TicketListComponent } from './components/tickets/ticket-list/ticket-list.component';
import { TicketResolver } from './resolvers/ticket.resolver';

export const routes: Routes = [
  {
    path: '',
    component: LoginComponent,
    title: 'Entrar • PHDS',
    data: { breadcrumb: 'Entrar' }
  },
  {
    path: 'tickets',
    component: TicketListComponent,
    title: 'Chamados • PHDS',
    data: { breadcrumb: 'Chamados' }
  },
  {
    path: 'ticket/create',
    component: TicketFormComponent,
    title: 'Novo chamado • PHDS',
    data: { breadcrumb: 'Novo Chamado' }
  },
  {
    path: 'ticket/:id',
    component: TicketDetailsComponent,
    title: 'Detalhes do chamado • PHDS',
    resolve: {
      ticket: TicketResolver
    },
    data: { breadcrumb: 'Detalhes do Chamado' }
  }
  
];
