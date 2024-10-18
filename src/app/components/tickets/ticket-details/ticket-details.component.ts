import { CommonModule } from '@angular/common';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
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
    AutosizeModule
  ],
  templateUrl: './ticket-details.component.html',
  styleUrls: ['./ticket-details.component.scss']
})
export class TicketDetailsComponent implements OnInit {
  activeMenuIndex: number | null = null;
  availablePriorities: string[] = [];
  availableStatus: string[] = [];
  comment: string = '';
  editedDescription: string = '';
  editedSubject: string = '';
  isEditModalOpen: boolean = false;
  isPriorityDropdownOpen: boolean = false;
  isStatusDropdownOpen: boolean = false;
  locationHierarchy: string[] = [];
  priorityTranslationMap = PRIORITY_TRANSLATION_MAP;
  selectedPriority: string | null = null;
  selectedStatus: string | null = null;
  showLocationModal: boolean = false;
  statusTranslationMap = STATUS_TRANSLATION_MAP;
  ticket: any;
  userId: number | null = null;

  @ViewChild('lastComment', { static: false }) lastComment!: ElementRef;

  constructor(
    private authService: AuthService,
    private route: ActivatedRoute,
    private title: Title,
    private ticketService: TicketService,
    private locationService: LocationService
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

  loadData(): void {
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
          this.loadData();
        }
      });
    }
  }

  assignCurrentUserAsCustomer(): void {
    if (this.ticket?.id) {
      this.ticketService.assignCurrentUser(this.ticket.id, false).subscribe({
        next: () => {
          this.loadData();
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

  deleteComment(ticketId: number, commentId: number): void {
    this.ticketService.deleteComment(ticketId, commentId).subscribe({
      next: () => {
        this.loadData();
      }
    });
  }

  removeCustomer(ticketId: number, customerId: number): void {
    this.ticketService.removeCustomer(ticketId, customerId).subscribe({
      next: () => {
        this.loadData();
      }
    });
  }

  removeAssignee(ticketId: number, assigneeId: number): void {
    this.ticketService.removeAssignee(ticketId, assigneeId).subscribe({
      next: () => {
        this.loadData();
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

  closeModal(): void {
    this.showLocationModal = false;
  }

  toggleMenu(index: number): void {
    this.activeMenuIndex = this.activeMenuIndex === index ? null : index;
  }

  formatHierarchy(hierarchy: string[]): string[] {
    return hierarchy.map((level, index) => {
      const indent = '    '.repeat(index);
      const prefix = index === hierarchy.length - 1 ? '└── ' : '├── ';
      return `${indent}${prefix}${level}`;
    });
  }

  getInitials(fullName: string): string {
    if (!fullName) return '';
    const names = fullName.split(' ');
    const initials = names.map(name => name.charAt(0).toUpperCase()).join('');
    return initials.length > 2 ? initials.charAt(0) + initials.charAt(initials.length - 1) : initials;
  }

  getGradient(fullName: string): string {
    const gradients = [
      'bg-gradient-to-t from-indigo-400 to-pink-300',
      'bg-gradient-to-t from-green-300 to-yellow-200',
      'bg-gradient-to-t from-blue-400 to-blue-200',
      'bg-gradient-to-t from-cyan-400 to-teal-200',
      'bg-gradient-to-t from-yellow-400 to-pink-200',
      'bg-gradient-to-t from-lime-400 to-green-200',
      'bg-gradient-to-t from-purple-400 to-purple-200',
      'bg-gradient-to-t from-pink-300 to-yellow-200'
    ];
    
    let hash = 0;
    for (let i = 0; i < fullName.length; i++)
      hash = fullName.charCodeAt(i) + ((hash << 5) - hash);

    const index = Math.abs(hash % gradients.length);
    return gradients[index];
  }
}
