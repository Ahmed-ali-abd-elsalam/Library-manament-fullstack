import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Header } from "./components/header/header";
import { NotificationContainerComponent } from './components/notifications/notification-container';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Header, NotificationContainerComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('frontend');
}
