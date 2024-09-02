import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class DropdownService {
  private openDropdown: string | null = null;

  setOpenDropdown(dropdown: string): void {
    this.openDropdown = dropdown;
  }

  closeDropdown(): void {
    this.openDropdown = null;
  }

  getOpenDropdown(): string | null {
    return this.openDropdown;
  }

  isOpen(dropdown: string): boolean {
    return this.openDropdown === dropdown;
  }
}
