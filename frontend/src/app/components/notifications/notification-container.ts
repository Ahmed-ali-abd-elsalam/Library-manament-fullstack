// notification-container.component.ts
import { Component, inject, OnDestroy, OnInit, signal, effect } from '@angular/core';
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
  private nextId = 1;

  private cleanupFns: Array<() => void> = [];

  ngOnInit(): void {
    // Reactively watch for authentication
    effect(() => {
      const loggedIn = this.auth.isAuthenticated();

      if (loggedIn) {
        this.notif.startConnection();

        const isAdmin = this.auth.isAdmin();

        const adminHandler = (msg: string) => {
          if (isAdmin) this.enqueue(msg, 10000);
        };
        const userHandler = (msg: string) => {
          if (!isAdmin) this.enqueue(msg, 10000);
        };

        this.notif.listenToAdminNotifications(adminHandler);
        this.notif.listenToUserNotifications(userHandler);

        this.cleanupFns.push(() => {
          // future unsubscribe if needed
        });
      }
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
    this.notifications.update((curr) => [item, ...curr]);
  }

  remove(id: number) {
    this.notifications.update((curr) => curr.filter((n) => n.id !== id));
  }

  trackById = (_: number, item: ToastNotification) => item.id;
}
