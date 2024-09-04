import { Component, EventEmitter, Input, Output } from '@angular/core';
import { SelectComponent } from '../select/select.component';
import { CommonModule } from '@angular/common';
import { PAGE_SIZE_OPTIONS } from '../../models/page-size.options';

@Component({
  selector: 'phds-pagination',
  standalone: true,
  imports: [
    CommonModule,
    SelectComponent
  ],
  templateUrl: './pagination.component.html',
  styleUrl: './pagination.component.scss'
})
export class PaginationComponent {
  @Input() page: number = 1;
  @Input() size: number = 10;
  @Input() totalItems: number = 0;
  @Input() pageSizeOptions = PAGE_SIZE_OPTIONS

  @Output() pageChange = new EventEmitter<number>();
  @Output() sizeChange = new EventEmitter<number>();

  get totalPages(): number {
    return Math.ceil(this.totalItems / this.size);
  }

  get pages(): number[] {
    const total = this.totalPages;
    let start = Math.max(1, this.page - 1);
    let end = Math.min(total, this.page + 1);

    if (this.page === 1) {
      end = Math.min(total, 3);
    }

    if (this.page === total) {
      start = Math.max(1, total - 2);
    }

    const pages: number[] = [];

    for (let i = start; i <= end; i++) {
      pages.push(i);
    }

    return pages;
  }

  get paginationText(): string {
    const start = (this.page - 1) * this.size + 1;
    const end = Math.min(this.page * this.size, this.totalItems);
    return `${start} – ${end} de ${this.totalItems}`;
  }

  onPageSizeChange(size: number): void {
    this.sizeChange.emit(size);
  }

  prevPage(): void {
    if (this.page > 1) {
      this.pageChange.emit(this.page - 1);
    }
  }

  nextPage(): void {
    if (this.page * this.size < this.totalItems) {
      this.pageChange.emit(this.page + 1);
    }
  }

  goToPage(page: number): void {
    this.pageChange.emit(page);
  }
}
