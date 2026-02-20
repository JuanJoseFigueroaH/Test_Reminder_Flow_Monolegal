import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-button',
  standalone: true,
  imports: [CommonModule],
  template: `
    <button
      [type]="type"
      [class]="'btn btn-' + variant"
      [disabled]="disabled || loading"
      (click)="onClick.emit($event)"
    >
      <span *ngIf="loading" class="loading-spinner"></span>
      <ng-content></ng-content>
    </button>
  `,
  styles: [`
    :host {
      display: inline-block;
    }
  `]
})
export class ButtonComponent {
  @Input() type: 'button' | 'submit' = 'button';
  @Input() variant: 'primary' | 'secondary' | 'success' | 'danger' | 'outline' = 'primary';
  @Input() disabled = false;
  @Input() loading = false;
  @Output() onClick = new EventEmitter<Event>();
}
