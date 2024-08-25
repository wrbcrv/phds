import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { TicketService } from '../../../services/ticket.service';
import { SelectComponent } from '../../../shared/select/select.component';
import { MessagePreviewComponent } from '../message-preview/message-preview.component';

@Component({
  selector: 'app-ticket-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    SelectComponent,
    MessagePreviewComponent,
    RouterModule,
  ],
  templateUrl: './ticket-list.component.html',
  styleUrls: ['./ticket-list.component.scss']
})
export class TicketListComponent implements OnInit {
  tickets: any[] = [];
  page: number = 1;
  size: number = 10;
  totalItems: number = 0;
  pageSizeOptions = [
    { display: '10', value: 10 },
    { display: '25', value: 25 },
    { display: '50', value: 50 },
    { display: '75', value: 75 },
    { display: '100', value: 100 }
  ];
  statusOptions = [
    { display: 'Aberto', value: 'open' },
    { display: 'Em Progresso', value: 'inProgress' },
    { display: 'Resolvido', value: 'resolved' },
    { display: 'Fechado', value: 'closed' }
  ];
  selectedStatus: string | null = null;
  dropdownOpen: boolean = false;
  selectedTicket: any;
  isModalOpen: boolean = false;
  currentUserId: number = 0;

  constructor(
    private authService: AuthService,
    private ticketService: TicketService) { }

  ngOnInit(): void {
    this.authService.getUserInfo().subscribe(
      (user) => {
        this.currentUserId = user.id;
        this.loadTickets();
      },
      (err) => {
        console.error(err);
      }
    );
  }

  loadTickets(): void {
    const filter = { status: this.selectedStatus };

    this.ticketService.findAll(this.page, this.size, filter).subscribe(
      (res) => {
        this.tickets = res.items;
        this.totalItems = res.total;
      }
    );
  }

  onPageSizeChange(size: number): void {
    this.size = size;
    this.page = 1;
    this.loadTickets();
  }

  onStatusChange(status: string | null): void {
    this.selectedStatus = status;
    this.page = 1;
    this.loadTickets();
  }

  prevPage(): void {
    if (this.page > 1) {
      this.page--;
      this.loadTickets();
    }
  }

  nextPage(): void {
    if (this.page * this.size < this.totalItems) {
      this.page++;
      this.loadTickets();
    }
  }

  goToPage(page: number): void {
    this.page = page;
    this.loadTickets();
  }

  preview(ticket: any) {
    this.selectedTicket = ticket;
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
    this.selectedTicket = null;
  }

  get totalPages(): number {
    return Math.ceil(this.totalItems / this.size);
  }

  get pages(): number[] {
    const total = this.totalPages;
    let start = Math.max(1, this.page - 1);
    let end = Math.min(total, this.page + 1);

    if (this.page === 1) {
      end = Math.min(total, 3);
    }

    if (this.page === total) {
      start = Math.max(1, total - 2);
    }

    const pages: number[] = [];

    for (let i = start; i <= end; i++) {
      pages.push(i);
    }

    return pages;
  }

  get paginationText(): string {
    const start = (this.page - 1) * this.size + 1;
    const end = Math.min(this.page * this.size, this.totalItems);
    return `${start} – ${end} de ${this.totalItems}`;
  }

  getLocationName(location: any): string {
    return location ? location.name : '';
  }

  getLocationHierarchy(location: any): string {
    let names = [];
    while (location) {
      names.push(location.name);
      location = location.parent;
    }
    return names.reverse().join(' > ');
  }
}
