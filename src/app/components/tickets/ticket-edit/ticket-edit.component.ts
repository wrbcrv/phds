import { CommonModule } from '@angular/common';
import { Component, ElementRef, EventEmitter, HostListener, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { PRIORITY } from '../../../models/priority.options';
import { STATUS } from '../../../models/status.options';
import { AgencyService } from '../../../services/agency.service';
import { UserService } from '../../../services/user.service';

type UserType = 'customer' | 'assignee' | 'observer';

@Component({
  selector: 'phds-ticket-edit',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './ticket-edit.component.html',
  styleUrls: ['./ticket-edit.component.scss']
})
export class TicketEditComponent implements OnInit {
  @Input() type!: 'customer' | 'assignee' | 'observer';

  isPriorityDropdownOpen = false;
  isStatusDropdownOpen = false;
  isAgencyDropdownOpen = false;
  isDropdownOpen: { [key in UserType]: boolean } = {
    customer: false,
    assignee: false,
    observer: false
  };
  selectedPriority = '';
  selectedStatus = '';
  selectedAgencyId: string | null = null;
  displayedPriority = '';
  displayedStatus = '';
  displayedAgency = '';
  agencies: any[] = [];
  filteredAgencies: any[] = [];
  highlightedAgencyId: string | null = null;
  searchTerm: string = '';

  STATUS = STATUS;
  PRIORITY = PRIORITY;

  @Input() ticket: any;
  @Output() closeModal = new EventEmitter<void>();
  @Output() ticketUpdated = new EventEmitter<any>();

  @ViewChild('priorityDropdown', { static: false }) priorityDropdown!: ElementRef;
  @ViewChild('statusDropdown', { static: false }) statusDropdown!: ElementRef;

  users: { [key in UserType]: any[] } = {
    customer: [],
    assignee: [],
    observer: []
  };

  filteredUsers: { [key in UserType]: any[] } = {
    customer: [],
    assignee: [],
    observer: []
  };

  selectedUserIds: { [key in UserType]: number[] } = {
    customer: [],
    assignee: [],
    observer: []
  };

  displayedUserNames: { [key in UserType]: string } = {
    customer: '',
    assignee: '',
    observer: ''
  };

  constructor(
    private agencyService: AgencyService,
    private userService: UserService
  ) { }

  ngOnInit(): void {
    this.init();
    this.loadAgencies(0, this.agencies.length);
    this.loadUsers('customer');
    this.loadUsers('assignee');
    this.loadUsers('observer');
  }

  @HostListener('document:click', ['$event'])
  handleClickOutside(event: MouseEvent): void {
    const target = event.target as HTMLElement;

    if (this.statusDropdown && !this.statusDropdown.nativeElement.contains(target)) {
      this.isStatusDropdownOpen = false;
    }

    if (this.priorityDropdown && !this.priorityDropdown.nativeElement.contains(target)) {
      this.isPriorityDropdownOpen = false;
    }

    if (!this.isAgencyDropdownOpen || (this.isAgencyDropdownOpen && target.closest('#agency') == null)) {
      this.isAgencyDropdownOpen = false;
    }

    if (this.isDropdownOpen['customer'] && !target.closest('#customer')) {
      this.isDropdownOpen['customer'] = false;
    }

    if (this.isDropdownOpen['assignee'] && !target.closest('#assignee')) {
      this.isDropdownOpen['assignee'] = false;
    }

    if (this.isDropdownOpen['observer'] && !target.closest('#observer')) {
      this.isDropdownOpen['observer'] = false;
    }
  }


  close(): void {
    this.closeModal.emit();
  }

  init(): void {
    if (this.ticket) {
      this.selectedStatus = this.ticket.status || '';
      this.selectedPriority = this.ticket.priority || '';

      if (this.ticket.location) {
        this.selectedAgencyId = this.ticket.location.id || null;
        this.displayedAgency = this.ticket.location.name || '';
      }

      if (this.ticket.customers && Array.isArray(this.ticket.customers)) {
        this.selectedUserIds['customer'] = this.ticket.customers.map((customer: any) => customer.id);
        this.displayedUserNames['customer'] = this.ticket.customers.map((customer: any) => customer.fullName).join(', ');
      }

      if (this.ticket.assignees && Array.isArray(this.ticket.assignees)) {
        this.selectedUserIds['assignee'] = this.ticket.assignees.map((assignee: any) => assignee.id);
        this.displayedUserNames['assignee'] = this.ticket.assignees.map((assignee: any) => assignee.fullName).join(', ');
      }

      if (this.ticket.observers && Array.isArray(this.ticket.observers)) {
        this.selectedUserIds['observer'] = this.ticket.observers.map((observer: any) => observer.id);
        this.displayedUserNames['observer'] = this.ticket.observers.map((observer: any) => observer.fullName).join(', ');
      }

      this.updateDisplayedValues();
    }
  }


  loadAgencies(page: number, size: number): void {
    this.agencyService.findAll(page, size).subscribe(res => {
      this.agencies = this.flattenAgencies(res.items);
      this.filteredAgencies = [...this.agencies];
    });
  }

  filterAgencies(): void {
    if (this.searchTerm.trim() === '') {
      this.highlightedAgencyId = null;
      return;
    }

    const normalizeString = (str: string): string => {
      return str.normalize('NFD').replace(/[\u0300-\u036f]/g, '').toLowerCase();
    };

    const normalizedSearchTerm = normalizeString(this.searchTerm);

    const matchingAgency = this.agencies.find(agency => {
      const normalizedAgencyName = normalizeString(agency.name);
      return normalizedAgencyName.includes(normalizedSearchTerm);
    });

    if (matchingAgency) {
      this.highlightedAgencyId = matchingAgency.id;
      this.scrollToHighlightedAgency();
    } else {
      this.highlightedAgencyId = null;
    }
  }

  scrollToHighlightedAgency(): void {
    setTimeout(() => {
      const highlightedElement = document.querySelector('.underline');
      if (highlightedElement) {
        highlightedElement.scrollIntoView({ behavior: 'smooth', block: 'center' });
      }
    }, 100);
  }

  selectAgency(id: string, name: string): void {
    this.selectedAgencyId = id;
    this.displayedAgency = name;
    this.isAgencyDropdownOpen = false;
  }

  selectValue(type: 'status' | 'priority', value: string): void {
    switch (type) {
      case 'status':
        this.selectedStatus = value;
        this.isStatusDropdownOpen = false;
        break;
      case 'priority':
        this.selectedPriority = value;
        this.isPriorityDropdownOpen = false;
        break;
    }

    this.updateDisplayedValues();
  }

  toggleDropdown(type: 'status' | 'priority' | 'agency' | UserType): void {
    this.isStatusDropdownOpen = type === 'status' ? !this.isStatusDropdownOpen : false;
    this.isPriorityDropdownOpen = type === 'priority' ? !this.isPriorityDropdownOpen : false;
    this.isAgencyDropdownOpen = type === 'agency' ? !this.isAgencyDropdownOpen : false;
    this.isDropdownOpen[type as UserType] = type === 'customer' || type === 'assignee' || type === 'observer' ? !this.isDropdownOpen[type as UserType] : false;

    if (type === 'agency' && this.isAgencyDropdownOpen) {
      this.scrollToSelectedAgency();
    }
  }

  scrollToSelectedAgency(): void {
    setTimeout(() => {
      const selected = document.querySelector('.bg-sky-100');
      if (selected) {
        selected.scrollIntoView({ behavior: 'smooth', block: 'center' });
      }
    }, 100);
  }

  updateDisplayedValues(): void {
    const selectedStatusOption = STATUS.find(option => option.value === this.selectedStatus);
    this.displayedStatus = selectedStatusOption ? selectedStatusOption.label : '';

    const selectedPriorityOption = PRIORITY.find(option => option.value === this.selectedPriority);
    this.displayedPriority = selectedPriorityOption ? selectedPriorityOption.label : '';

    if (this.selectedAgencyId && this.agencies.length) {
      const selectedAgency = this.agencies.find(agency => agency.id === this.selectedAgencyId);
      this.displayedAgency = selectedAgency ? selectedAgency.name : '';
    }
  }

  loadUsers(type: UserType): void {
    this.userService.findAll(0, 1000).subscribe(res => {
      this.users[type] = res.items;
      this.filteredUsers[type] = [...this.users[type]];
    });
  }

  filterUsers(type: UserType): void {
    const normalizedSearchTerm = this.searchTerm.toLowerCase();
    this.filteredUsers[type] = this.users[type].filter(user =>
      user.fullName.toLowerCase().includes(normalizedSearchTerm)
    );
  }

  selectUser(type: UserType, id: number): void {
    const index = this.selectedUserIds[type].indexOf(id);
    if (index === -1) this.selectedUserIds[type].push(id);
    else this.selectedUserIds[type].splice(index, 1);
    this.updateDisplayedUsers(type);
  }

  updateDisplayedUsers(type: UserType): void {
    const selectedUsers = this.users[type].filter(user => this.selectedUserIds[type].includes(user.id));
    this.displayedUserNames[type] = selectedUsers.map(user => user.fullName).join(', ');
  }

  updateTicket(): void {
    const updatedTicket = {
      ...this.ticket,
      priority: this.selectedPriority,
      status: this.selectedStatus,
      locationId: this.selectedAgencyId,
      customerIds: this.selectedUserIds['customer'],
      assigneeIds: this.selectedUserIds['assignee'],
      observerIds: this.selectedUserIds['observer'],
    };
    this.ticketUpdated.emit(updatedTicket);
  }

  private flattenAgencies(agencies: any[]): any[] {
    let flatList: any[] = [];

    const recurseAgencies = (agencies: any[], level: number = 0) => {
      for (const agency of agencies) {
        flatList.push({
          id: agency.id,
          name: agency.name,
          level: level,
          isParent: agency.children && agency.children.length > 0,
          isTopLevel: agency.isTopLevel
        });

        if (agency.children && agency.children.length > 0) {
          recurseAgencies(agency.children, level + 1);
        }
      }
    };

    recurseAgencies(agencies);
    return flatList;
  }
}
