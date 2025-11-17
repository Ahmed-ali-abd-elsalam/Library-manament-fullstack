import { Component, inject, OnDestroy, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../services/notification-service';
import { AuthService } from '../../services/auth.service';
import { NotificationItemComponent, ToastNotification } from './notification-item';

@Component({
  selector: 'app-notification-container',
  standalone: true,
  imports: [CommonModule, NotificationItemComponent],
  templateUrl: './notification-container.html',
  styleUrl: './notification-container.scss',
})
export class NotificationContainerComponent implements OnInit, OnDestroy {
  private notif = inject(NotificationService);
  private auth = inject(AuthService);

  notifications = signal<ToastNotification[]>([]);

  // helpers
  private nextId = 1;
  private cleanupFns: Array<() => void> = [];

  ngOnInit(): void {
    this.notif.startConnection();

    // Subscribe to both channels; filter by role
    const isAdmin = this.auth.isAdmin();

    const adminHandler = (message: string) => {
      if (isAdmin) this.enqueue(message, 10000);
    };
    const userHandler = (message: string) => {
      if (!isAdmin) this.enqueue(message, 10000);
    };

    this.notif.listenToAdminNotifications(adminHandler);
    this.notif.listenToUserNotifications(userHandler);

    // store cleanup (SignalR off isn't exposed here, but if added later we can unhook)
    this.cleanupFns.push(() => {
      // noop for now
    });
  }

  ngOnDestroy(): void {
    this.cleanupFns.forEach((fn) => fn());
  }

  enqueue(message: string, durationMs = 5000) {
    const id = this.nextId++;
    const item: ToastNotification = {
      id,
      message,
      durationMs,
      createdAt: Date.now(),
    };
    // newest at the bottom (container is column-reverse)
    this.notifications.update((curr) => [item, ...curr]);
  }

  remove(id: number) {
    this.notifications.update((curr) => curr.filter((n) => n.id !== id));
  }

  trackById = (_: number, item: ToastNotification) => item.id;
}
