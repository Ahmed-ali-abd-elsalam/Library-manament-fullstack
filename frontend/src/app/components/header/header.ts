import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-header',
  imports: [RouterLink],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class Header implements OnInit {
  authservice = inject(AuthService);
  router = inject(Router);
  
  userName = signal('userName');
  isLoggedIn = computed(() => !!this.authservice.getToken());
  
  ngOnInit() {
    const userInfo = this.authservice.getUserInfo();
    if (userInfo?.name) {
      this.userName.set(userInfo.name);
    }
  }
  
  logOut() {
    this.authservice.clearToken();
    this.router.navigateByUrl("/");
  }
}
