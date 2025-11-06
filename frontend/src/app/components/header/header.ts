import { Component, inject, signal, computed, OnInit, effect } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-header',
  imports: [RouterLink],
  templateUrl: './header.html',
  styleUrl: './header.scss',
})
export class Header {
  authservice = inject(AuthService);
  router = inject(Router);

  userName = signal('');
  isLoggedIn = this.authservice.isAuthenticated;

  isAdmin() {
    return this.authservice.isAdmin();
  }

  constructor() {
    // React to user info signal changes
    effect(() => {
      const userJson = this.authservice.userInfo();
      if (userJson) {
        try {
          const userInfo = JSON.parse(userJson);
          if (userInfo?.UserName) {
            this.userName.set(userInfo.UserName);
          }
        } catch (e) {
          console.error('Invalid user info JSON', e);
        }
      } else {
        // clear username when no user info
        this.userName.set('');
      }
    });

    // Fallback: if signal is empty but localStorage has user_info (e.g., before signal initialization), populate it
    try {
      const stored = localStorage.getItem('user_info');
      if (stored && !this.authservice.userInfo()) {
        this.authservice.userInfo.set(stored);
      }
    } catch (e) {
      // ignore
    }
  }
  ngOnInit() {
    const stored = localStorage.getItem('user_info');
    if (stored) {
      const user = JSON.parse(stored);
      if (user?.UserName) {
        this.userName.set(user.UserName);
      }
    }
  }

  logOut() {
    this.authservice.clearLocalStorage();
    this.router.navigateByUrl('/');
    this.isLoggedIn();
  }
}
