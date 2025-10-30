import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.html',
  styleUrls: ['./login.scss']
})
export class LoginComponent {
  email = '';
  password = '';
  loading = false;
  error: string | null = null;

  constructor(private router: Router, private auth: AuthService) {}

  onSubmit() {
    this.loading = true;
    this.error = null;

    this.auth.login({ email: this.email, password: this.password }).subscribe({
      next: () => {
        this.router.navigateByUrl('/');
        this.loading = false;
      },
      error: (err) => {
        const message = err?.error?.message || err?.message || 'Login failed';
        this.error = message;
        this.loading = false;
      }
    });
  }
}
