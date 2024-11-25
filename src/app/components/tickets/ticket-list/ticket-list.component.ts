import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { PAGE_SIZE_OPTIONS } from '../../../models/page-size.options';
import { PRIORITY } from '../../../models/priority.options';
import { STATUS } from '../../../models/status.options';
import { AuthService } from '../../../services/auth.service';
import { LocationService } from '../../../services/location.service';
import { TicketService } from '../../../services/ticket.service';
import { PaginationComponent } from '../../../shared/pagination/pagination.component';
import { PRIORITY_TRANSLATION_MAP, STATUS_TRANSLATION_MAP } from '../../../shared/translations/translations';

@Component({
  selector: 'app-ticket-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
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
  statusOptions = STATUS;
  priorityOptions = PRIORITY;
  statusTranslationMap = STATUS_TRANSLATION_MAP;
  priorityTranslationMap = PRIORITY_TRANSLATION_MAP;
  selectedStatus: string | null = null;
  selectedPriority: string | null = null;
  selectedTicket: any;
  dropdownOpen: boolean = false;
  isModalOpen: boolean = false;
  currentUserId: number = 0;
  allSelected: boolean = false;

  viewMode: 'grid' | 'list' = 'grid';

  constructor(
    private authService: AuthService,
    private ticketService: TicketService,
    private locationService: LocationService
  ) { }

  ngOnInit(): void {
    this.getViewMode();
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

  getLastLocation(location: any): string {
    return this.locationService.getLastLocation(location);
  }

  getFullLocation(location: any): string {
    return this.locationService.getFullLocation(location);
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

  setViewMode(mode: 'grid' | 'list'): void {
    this.viewMode = mode;
    localStorage.setItem('viewMode', mode);
  }

  getViewMode(): void {
    const savedMode = localStorage.getItem('viewMode');
    if (savedMode === 'grid' || savedMode === 'list') {
      this.viewMode = savedMode as 'grid' | 'list';
    }
  }
}
