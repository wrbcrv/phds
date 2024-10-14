import { CommonModule } from '@angular/common';
import { Component, ElementRef, HostListener, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { TippyDirective } from '@ngneat/helipopper';
import { AutosizeModule } from 'ngx-autosize';
import { AuthService } from '../../../services/auth.service';
import { LocationService } from '../../../services/location.service';
import { TicketService } from '../../../services/ticket.service';
import { PRIORITY_TRANSLATION_MAP, STATUS_TRANSLATION_MAP } from '../../../shared/translations/translations';

@Component({
  selector: 'phds-ticket-details',
  standalone: true,
  imports: [
    FormsModule,
    CommonModule,
    TippyDirective,
    AutosizeModule
  ],
  templateUrl: './ticket-details.component.html',
  styleUrls: ['./ticket-details.component.scss']
})
export class TicketDetailsComponent implements OnInit {
  ticket: any;
  userId: number | null = null;
  comment: string = '';
  isEditModalOpen: boolean = false;
  statusTranslationMap = STATUS_TRANSLATION_MAP;
  priorityTranslationMap = PRIORITY_TRANSLATION_MAP;

  isStatusDropdownOpen: boolean = false;
  isPriorityDropdownOpen: boolean = false;
  selectedStatus: string | null = null;
  selectedPriority: string | null = null;
  availableStatus: string[] = [];
  availablePriorities: string[] = [];

  editedSubject: string = '';
  editedDescription: string = '';

  // Novas propriedades para o modal de hierarquia de localização
  showLocationModal: boolean = false;
  locationHierarchy: string[] = [];

  constructor(
    private authService: AuthService,
    private route: ActivatedRoute,
    private title: Title,
    private ticketService: TicketService,
    private locationService: LocationService,
    private elementRef: ElementRef
  ) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.ticket = data['ticket'];
      this.title.setTitle(`Chamado #${this.ticket.id} • PHDS`);
    });

    this.authService.getUserInfo().subscribe({
      next: (res) => {
        this.userId = res.id;
      }
    });
  }

  loadTicket(): void {
    if (this.ticket?.id) {
      this.ticketService.findOne(this.ticket.id).subscribe({
        next: (ticket) => {
          this.ticket = ticket;
        }
      });
    }
  }

  assignCurrentUserAsAssignee(): void {
    if (this.ticket?.id) {
      this.ticketService.assignCurrentUser(this.ticket.id, true).subscribe({
        next: () => {
          this.loadTicket();
        }
      });
    }
  }

  assignCurrentUserAsCustomer(): void {
    if (this.ticket?.id) {
      this.ticketService.assignCurrentUser(this.ticket.id, false).subscribe({
        next: () => {
          this.loadTicket();
        }
      });
    }
  }

  sendComment(): void {
    if (this.comment.trim() && this.userId) {
      this.ticketService.addComment(this.ticket.id, this.userId, this.comment).subscribe({
        next: (res) => {
          this.ticket.comments.push(res);
          this.comment = '';
        }
      });
    }
  }

  removeCustomer(ticketId: number, customerId: number): void {
    this.ticketService.removeCustomer(ticketId, customerId).subscribe({
      next: () => {
        this.loadTicket();
      }
    });
  }

  removeAssignee(ticketId: number, assigneeId: number): void {
    this.ticketService.removeAssignee(ticketId, assigneeId).subscribe({
      next: () => {
        this.loadTicket();
      }
    });
  }

  isAdmin(): boolean {
    return this.authService.isAdmin();
  }

  isAgent(): boolean {
    return this.authService.isAgent();
  }

  getLastLocation(location: any): string {
    return this.locationService.getLastLocation(location);
  }

  getFullLocation(location: any): string {
    return this.locationService.getFullLocation(location);
  }

  openModal(): void {
    if (this.ticket && this.ticket.location) {
      this.locationHierarchy = this.getFullLocation(this.ticket.location).split(' > ');
      this.locationHierarchy = this.formatHierarchy(this.locationHierarchy);
      this.showLocationModal = true;
    }
  }

  formatHierarchy(hierarchy: string[]): string[] {
    return hierarchy.map((level, index) => {
      const indent = '    '.repeat(index); 
      const prefix = index === hierarchy.length - 1 ? '└── ' : '├── ';
      return `${indent}${prefix}${level}`;
    });
  }

  closeModal(): void {
    this.showLocationModal = false;
  }

  getInitials(fullName: string): string {
    if (!fullName) return '';
    const names = fullName.split(' ');
    const initials = names.map(name => name.charAt(0).toUpperCase()).join('');
    return initials.length > 2 ? initials.charAt(0) + initials.charAt(initials.length - 1) : initials;
  }
}
