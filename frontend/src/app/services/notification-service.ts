// notification-service.ts
import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private hubConnection!: signalR.HubConnection;
  private started = false;

  startConnection() {
    if (this.started) return;

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7205/notifyHub', {
        withCredentials: true,
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        this.started = true;
        console.log('%cSignalR Connected', 'color: green');
      })
      .catch((err) => console.error('SignalR error:', err));
  }

  listenToAdminNotifications(callback: (message: string) => void) {
    if (!this.hubConnection) return;
    this.hubConnection.on('ReceiveAdminNotification', callback);
  }

  listenToUserNotifications(callback: (message: string) => void) {
    if (!this.hubConnection) return;
    this.hubConnection.on('ReceiveUserNotification', callback);
  }
}
