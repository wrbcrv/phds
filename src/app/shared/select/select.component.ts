import { CommonModule } from '@angular/common';
import { Component, ElementRef, EventEmitter, HostListener, Input, Output } from '@angular/core';

@Component({
  selector: 'phds-select',
  standalone: true,
  imports: [
    CommonModule
  ],
  templateUrl: './select.component.html',
  styleUrl: './select.component.scss'
})
export class SelectComponent {
  @Input() options: number[] = [];
  @Input() selectedValue: number = 10;
  @Output() valueChange = new EventEmitter<number>();

  dropdownOpen: boolean = false;

  constructor(private elementRef: ElementRef) { }

  toggleDropdown(): void {
    this.dropdownOpen = !this.dropdownOpen;
  }

  selectOption(option: number): void {
    this.selectedValue = option;
    this.dropdownOpen = false;
    this.valueChange.emit(this.selectedValue);
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
