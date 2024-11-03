import { CommonModule } from '@angular/common';
import { Component, ElementRef, EventEmitter, HostListener, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { PRIORITY } from '../../../models/priority.options';
import { STATUS } from '../../../models/status.options';
import { AgencyService } from '../../../services/agency.service';

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
  isPriorityDropdownOpen = false;
  isStatusDropdownOpen = false;
  isAgencyDropdownOpen = false;
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

  constructor(private agencyService: AgencyService) { }

  ngOnInit(): void {
    this.init();
    this.loadAgencies(0, this.agencies.length);
  }

  @HostListener('document:click', ['$event'])
  handleClickOutside(event: MouseEvent): void {
    const target = event.target as HTMLElement;

    if (this.statusDropdown && !this.statusDropdown.nativeElement.contains(target))
      this.isStatusDropdownOpen = false;

    if (this.priorityDropdown && !this.priorityDropdown.nativeElement.contains(target))
      this.isPriorityDropdownOpen = false;

    if (!this.isAgencyDropdownOpen || (this.isAgencyDropdownOpen && target.closest('#agency') == null))
      this.isAgencyDropdownOpen = false;
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
      return str.normalize('NFD') .replace(/[\u0300-\u036f]/g, '').toLowerCase();
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
      const highlightedElement = document.querySelector('.bg-yellow-100');
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

  toggleDropdown(type: 'status' | 'priority' | 'agency'): void {
    this.isStatusDropdownOpen = type === 'status' ? !this.isStatusDropdownOpen : false;
    this.isPriorityDropdownOpen = type === 'priority' ? !this.isPriorityDropdownOpen : false;
    this.isAgencyDropdownOpen = type === 'agency' ? !this.isAgencyDropdownOpen : false;
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

  updateTicket(): void {
    const updatedTicket = {
      ...this.ticket,
      priority: this.selectedPriority,  
      status: this.selectedStatus,
      locationId: this.selectedAgencyId
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
