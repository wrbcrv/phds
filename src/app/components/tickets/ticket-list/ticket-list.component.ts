import { Component, OnInit, HostListener } from '@angular/core';
import { TicketService } from '../../../services/ticket.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PhdsSelectComponent } from '../../../shared/phds-select/phds-select.component';

@Component({
  selector: 'app-ticket-list',
  standalone: true,
  imports: [CommonModule, FormsModule, PhdsSelectComponent],
  templateUrl: './ticket-list.component.html',
  styleUrls: ['./ticket-list.component.scss']
})
export class TicketListComponent implements OnInit {
  tickets: any[] = [];
  page: number = 1;
  size: number = 10;
  totalItems: number = 0;
  pageSizeOptions: number[] = [5, 10, 20, 50];
  dropdownOpen: boolean = false;

  constructor(private ticketService: TicketService) { }

  ngOnInit(): void {
    this.loadTickets();
  }

  loadTickets(): void {
    this.ticketService.findAll(this.page, this.size).subscribe(
      (res) => {
        this.tickets = res.items;
        this.totalItems = res.total;
        console.log(this.tickets);
      },
      (err) => {
        console.error(err);
      }
    );
  }

  onPageSizeChange(size: number): void {
    this.size = size;
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

  get totalPages(): number {
    return Math.ceil(this.totalItems / this.size);
  }

  get pages(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
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
