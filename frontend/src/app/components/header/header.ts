import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-header',
  imports: [RouterLink],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class Header {
  title = signal('Library management system')
  authservice = inject(AuthService);
  router = inject(Router);
  logOut(){
    this.authservice.clearToken();
    this.router.navigateByUrl("/");
  }
  
}
