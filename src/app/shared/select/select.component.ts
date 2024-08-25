import { CommonModule } from '@angular/common';
import { Component, ElementRef, EventEmitter, HostListener, Input, Output } from '@angular/core';

@Component({
  selector: 'phds-select',
  standalone: true,
  imports: [
    CommonModule
  ],
  templateUrl: './select.component.html',
  styleUrls: ['./select.component.scss']
})
export class SelectComponent<T> {
  @Input() options: { display: string, value: T }[] = [];
  @Input() selectedValue: T | null = null;
  
  @Output() valueChange = new EventEmitter<T>();

  dropdownOpen: boolean = false;

  constructor(private elementRef: ElementRef) { }

  toggleDropdown(): void {
    this.dropdownOpen = !this.dropdownOpen;
  }

  selectOption(value: T): void {
    this.selectedValue = value;
    this.dropdownOpen = false;
    this.valueChange.emit(this.selectedValue);
  }

  getDisplayValue(value: T | null): string {
    const option = this.options.find(opt => opt.value === value);
    return option ? option.display : '';
  }

  @HostListener('document:click', ['$event'])
  clickOutside(event: MouseEvent): void {
    const target = event.target as HTMLElement;
    if (this.elementRef.nativeElement.contains(target)) {
      return;
    }
    this.dropdownOpen = false;
  }
}
