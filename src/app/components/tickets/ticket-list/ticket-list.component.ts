import { Component, OnInit } from '@angular/core';
import { TicketService } from '../../../services/ticket.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-ticket-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './ticket-list.component.html',
  styleUrl: './ticket-list.component.scss'
})
export class TicketListComponent implements OnInit {
  tickets: any[] = [];
  page: number = 1;
  size: number = 10;

  constructor(private ticketService: TicketService) { }

  ngOnInit(): void {
    this.loadTickets();
  }

  loadTickets(): void {
    this.ticketService.findAll(this.page, this.size).subscribe(
      (res) => {
        this.tickets = res;
        console.log(this.tickets);
      },
      (err) => {
        console.error(err);
      }
    );
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