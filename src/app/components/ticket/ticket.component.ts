import { Component, OnInit, HostListener } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AgencyService } from '../../services/agency.service';
import { TicketRequest, TicketService } from '../../services/ticket.service';
import { UserService } from '../../services/user.service';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-ticket',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './ticket.component.html',
  styleUrls: ['./ticket.component.scss']
})
export class TicketComponent implements OnInit {
  ticket: TicketRequest = {
    subject: '',
    description: '',
    type: '',
    status: '',
    priority: '',
    locationId: 0,
    customerIds: [],
    assigneeIds: []
  };

  priorities: string[] = [];
  status: string[] = [];
  types: string[] = [];
  agencies: any[] = [];
  users: any[] = [];

  filteredCustomers: any[] = [];
  filteredAssignees: any[] = [];
  selectedCustomers: any[] = [];
  selectedAssignees: any[] = [];

  showCustomersDropdown: boolean = false;
  showAssigneesDropdown: boolean = false;

  showTypeDropdown: boolean = false;
  showStatusDropdown: boolean = false;
  showPriorityDropdown: boolean = false;
  showLocationDropdown: boolean = false;

  selectedType: string = '';
  selectedStatus: string = '';
  selectedPriority: string = '';
  selectedLocation: string = '';

  constructor(
    private agencyService: AgencyService,
    private ticketService: TicketService,
    private userService: UserService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.ticketService.getPriorityValues().subscribe(
      (res) => {
        this.priorities = res;
      },
      (err) => {
        console.error(err);
      }
    );

    this.ticketService.getStatusValues().subscribe(
      (res) => {
        this.status = res;
      },
      (err) => {
        console.error(err);
      }
    );

    this.ticketService.getTypeValues().subscribe(
      (res) => {
        this.types = res;
      },
      (err) => {
        console.error(err);
      }
    );

    this.agencyService.findAll().subscribe(
      (res) => {
        this.agencies = this.formatAgencies(res);
      },
      (err) => {
        console.error(err);
      }
    );

    this.userService.findAll().subscribe(
      (res) => {
        this.users = res;
      },
      (err) => {
        console.error(err);
      }
    );
  }

  onSubmit() {
    this.ticketService.create(this.ticket).subscribe(
      (res) => {
        this.router.navigateByUrl('/');
        console.log(res);
      },
      (err) => {
        console.log(err);
      }
    );
  }

  formatAgencies(agencies: any[], prefix: string = ''): any[] {
    let formatted: any[] = [];

    for (let agency of agencies) {
      formatted.push({
        id: agency.id,
        name: prefix + agency.name
      });

      if (agency.children && agency.children.length > 0) {
        formatted = formatted.concat(this.formatAgencies(agency.children, prefix + '&nbsp;'));
      }
    }

    return formatted;
  }

  toggleDropdown(field: string): void {
    if (field === 'type') {
      this.showTypeDropdown = !this.showTypeDropdown;
    }

    if (field === 'status') {
      this.showStatusDropdown = !this.showStatusDropdown;
    }

    if (field === 'priority') {
      this.showPriorityDropdown = !this.showPriorityDropdown;
    }

    if (field === 'location') {
      this.showLocationDropdown = !this.showLocationDropdown;
    }
  }

  selectOption(field: string, value: any, name?: string): void {
    if (field === 'type') {
      this.ticket.type = value;
      this.selectedType = value;
      this.showTypeDropdown = false;
    }

    if (field === 'status') {
      this.ticket.status = value;
      this.selectedStatus = value;
      this.showStatusDropdown = false;
    }

    if (field === 'priority') {
      this.ticket.priority = value;
      this.selectedPriority = value;
      this.showPriorityDropdown = false;
    }

    if (field === 'location') {
      this.ticket.locationId = value;
      this.selectedLocation = name!.replace(/&nbsp;/g, '');
      this.showLocationDropdown = false;
    }
  }

  searchUsers(event: Event, type: 'customer' | 'assignee'): void {
    const input = event.target as HTMLInputElement;
    const term = input.value;

    if (term) {
      this.userService.findByPartialName(term).subscribe(
        (res) => {
          if (type === 'customer') {
            this.filteredCustomers = res;
            this.showCustomersDropdown = true;
          }

          if (type === 'assignee') {
            this.filteredAssignees = res;
            this.showAssigneesDropdown = true;
          }
        },
        (err) => {
          if (type === 'customer') {
            this.filteredCustomers = [];
          }

          if (type === 'assignee') {
            this.filteredAssignees = [];
          }
        }
      );
    } else {
      if (type === 'customer') {
        this.filteredCustomers = [];
        this.showCustomersDropdown = false;
      }

      if (type === 'assignee') {
        this.filteredAssignees = [];
        this.showAssigneesDropdown = false;
      }
    }
  }

  selectUser(user: any, type: 'customer' | 'assignee'): void {
    if (type === 'customer') {
      if (!this.selectedCustomers.some(u => u.id === user.id)) {
        this.ticket.customerIds.push(user.id);
        this.selectedCustomers.push(user);
      }

      this.showCustomersDropdown = false;
    }

    if (type === 'assignee') {
      if (!this.selectedAssignees.some(u => u.id === user.id)) {
        this.ticket.assigneeIds.push(user.id);
        this.selectedAssignees.push(user);
      }

      this.showAssigneesDropdown = false;
    }

    (document.getElementById(`${type}Search`) as HTMLInputElement).value = '';
  }

  removeUser(userId: number, type: 'customer' | 'assignee'): void {
    if (type === 'customer') {
      this.ticket.customerIds = this.ticket.customerIds.filter(id => id !== userId);
      this.selectedCustomers = this.selectedCustomers.filter(user => user.id !== userId);
    }

    if (type === 'assignee') {
      this.ticket.assigneeIds = this.ticket.assigneeIds.filter(id => id !== userId);
      this.selectedAssignees = this.selectedAssignees.filter(user => user.id !== userId);
    }
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const target = event.target as HTMLElement;
    if (!target.closest('.dropdown')) {
      this.showTypeDropdown = false;
      this.showStatusDropdown = false;
      this.showPriorityDropdown = false;
      this.showLocationDropdown = false;
    }
  }
}
