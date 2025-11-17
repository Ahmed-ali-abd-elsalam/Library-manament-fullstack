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
    const token = localStorage.getItem('access_token'); // or from a service
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7205/notifyHub', {
        accessTokenFactory: () => token ?? '',
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        this.started = true;
      })
      .catch((err) => console.error(err));
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
