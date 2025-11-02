import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-header',
  imports: [RouterLink],
  templateUrl: './header.html',
  styleUrl: './header.scss',
})
export class Header implements OnInit {
  authservice = inject(AuthService);
  router = inject(Router);

  userName = signal('xxx');
  isLoggedIn = computed(() => !!this.authservice.getToken());

  ngOnInit() {
    const userInfo = this.authservice.getUserInfo();
    // transfer userInfo json to object
    if (userInfo?.UserName) {
      this.userName.set(userInfo.UserName);
    }
  }

  logOut() {
    this.authservice.clearToken();
    this.router.navigateByUrl('/');
    this.isLoggedIn();
  }
}
