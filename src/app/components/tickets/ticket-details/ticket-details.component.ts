import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { TicketService } from '../../../services/ticket.service';
import { PRIORITY_TRANSLATION_MAP, STATUS_TRANSLATION_MAP } from '../../../shared/translations/translations';
import { TippyDirective } from '@ngneat/helipopper';
import { LocationService } from '../../../services/location.service';

@Component({
  selector: 'app-ticket-details',
  standalone: true,
  imports: [
    FormsModule,
    CommonModule,
    TippyDirective
  ],
  templateUrl: './ticket-details.component.html',
  styleUrls: ['./ticket-details.component.scss']
})
export class TicketDetailsComponent implements OnInit {
  ticket: any;
  userId: number | null = null;
  comment: string = '';
  statusTranslationMap = STATUS_TRANSLATION_MAP;
  priorityTranslationMap = PRIORITY_TRANSLATION_MAP;

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

    this.authService.getUserInfo().subscribe(
      (res) => {
        this.userId = res.id;
      }
    );
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

  getInitials(fullName: string): string {
    if (!fullName) return '';

    const names = fullName.split(' ');
    const initials = names.map(name => name.charAt(0).toUpperCase()).join('');

    return initials.length > 2 ? initials.charAt(0) + initials.charAt(initials.length - 1) : initials;
  }
}
