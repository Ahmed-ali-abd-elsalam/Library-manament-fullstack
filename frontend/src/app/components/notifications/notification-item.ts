import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface ToastNotification {
  id: number;
  message: string;
  durationMs: number;
  createdAt: number;
}

@Component({
  selector: 'app-notification-item',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notification-item.html',
  styleUrl: './notification-item.scss',
})
export class NotificationItemComponent implements OnInit, OnDestroy {
  @Input() notification!: ToastNotification;
  @Output() dismiss = new EventEmitter<number>();

  private timeoutId: any;

  ngOnInit(): void {
    const duration = this.notification?.durationMs ?? 5000;
    this.timeoutId = setTimeout(() => this.dismiss.emit(this.notification.id), duration);
  }

  ngOnDestroy(): void {
    if (this.timeoutId) clearTimeout(this.timeoutId);
  }
}
