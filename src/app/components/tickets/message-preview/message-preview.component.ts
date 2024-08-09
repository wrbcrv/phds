import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-message-preview',
  standalone: true,
  imports: [],
  templateUrl: './message-preview.component.html',
  styleUrl: './message-preview.component.scss'
})
export class MessagePreviewComponent {
  @Input() ticket: any;
  @Output() close = new EventEmitter<void>();

  closeModal(): void { 
    this.close.emit();
  }
}
