import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { TicketService } from '../../../services/ticket.service';
import { SelectComponent } from '../../../shared/select/select.component';
import { MessagePreviewComponent } from '../message-preview/message-preview.component';
import { PRIORITY_OPTIONS } from '../../../models/priority.options';
import { STATUS_OPTIONS } from '../../../models/status.options';
import { PAGE_SIZE_OPTIONS } from '../../../models/page-size.options';
import { PaginationComponent } from '../../../shared/pagination/pagination.component';
import { STATUS_TRANSLATION_MAP, PRIORITY_TRANSLATION_MAP } from '../../../shared/translations/translations';

@Component({
  selector: 'app-ticket-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    SelectComponent,
    MessagePreviewComponent,
    RouterModule,
    PaginationComponent
  ],
  templateUrl: './ticket-list.component.html',
  styleUrls: ['./ticket-list.component.scss']
})
export class TicketListComponent implements OnInit {
  tickets: any[] = [];
  page: number = 1;
  size: number = 10;
  totalItems: number = 0;
  pageSizeOptions = PAGE_SIZE_OPTIONS;
  statusOptions = STATUS_OPTIONS;
  priorityOptions = PRIORITY_OPTIONS;
  statusTranslationMap = STATUS_TRANSLATION_MAP;
  priorityTranslationMap = PRIORITY_TRANSLATION_MAP;
  selectedStatus: string | null = null;
  selectedPriority: string | null = null;
  selectedTicket: any;
  dropdownOpen: boolean = false;
  isModalOpen: boolean = false;
  currentUserId: number = 0;
  allSelected: boolean = false;

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
    const filter = {
      status: this.selectedStatus,
      priority: this.selectedPriority
    };

    this.ticketService.findAll(this.page, this.size, filter).subscribe(
      (res) => {
        this.tickets = res.items;
        this.totalItems = res.total;

        this.tickets.forEach(ticket => ticket.selected = false);
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

  onPriorityChange(priority: string | null): void {
    this.selectedPriority = priority;
    this.page = 1;
    this.loadTickets();
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

  toggleAll(event: Event): void {
    this.allSelected = (event.target as HTMLInputElement).checked;
    this.tickets.forEach(ticket => {
      ticket.selected = this.allSelected;
    });
  }

  onTicketSelect(ticket: any): void {
    if (!ticket.selected) {
      this.allSelected = false;
    } else {
      this.allSelected = this.tickets.every(t => t.selected);
    }
  }
}
