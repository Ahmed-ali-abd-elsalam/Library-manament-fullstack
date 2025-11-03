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
  isLoggedIn = computed(() => !!this.authservice.getToken());

  constructor() {
    // React to user info signal changes
    effect(() => {
      const userjson = this.authservice.getUserInfo();
      if (userjson) {
        try {
          const userInfo = JSON.parse(userjson);
          if (userInfo?.UserName) {
            this.userName.set(userInfo.UserName);
          }
        } catch (e) {
          console.error('Invalid user info JSON', e);
        }
      }
    });
  }
  // ngOnInit() {
  //   const userJson = this.authservice.getUserInfo();
  //   if (userJson) {
  //     const userInfo = JSON.parse(userJson);
  //     // transfer userInfo json to object
  //     if (userInfo?.UserName) {
  //       this.userName.set(userInfo.UserName);
  //     }
  //   }
  // }

  logOut() {
    this.authservice.clearToken();
    this.router.navigateByUrl('/');
    this.isLoggedIn();
  }
}
