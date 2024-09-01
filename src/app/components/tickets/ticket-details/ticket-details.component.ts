import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { TicketService } from '../../../services/ticket.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-ticket-details',
  standalone: true,
  imports: [
    FormsModule,
    CommonModule
  ],
  templateUrl: './ticket-details.component.html',
  styleUrl: './ticket-details.component.scss'
})
export class TicketDetailsComponent implements OnInit {
  ticket: any;
  userId: number | null = null;
  comment: string = '';

  constructor(
    private authService: AuthService,
    private route: ActivatedRoute,
    private title: Title,
    private ticketService: TicketService
  ) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.ticket = data['ticket'];
      this.title.setTitle(`Chamado #${this.ticket.id} • PHDS`)
    });

    this.authService.getUserInfo().subscribe(
      (res) => {
        this.userId = res.id;
      }
    )
  }

  sendComment(): void {
    if (this.comment.trim() && this.userId) {
      this.ticketService.addComment(this.ticket.id, this.userId, this.comment)
        .subscribe(res => {
          this.ticket.comments.push(res);
          this.comment = '';
        });
    }
  }

  getInitials(fullName: string): string {
    if (!fullName) return '';
    
    const names = fullName.split(' ');
    const initials = names.map(name => name.charAt(0).toUpperCase()).join('');
    
    return initials.length > 2 ? initials.charAt(0) + initials.charAt(initials.length - 1) : initials;
  }
}
