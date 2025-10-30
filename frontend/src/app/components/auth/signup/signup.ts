import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './signup.html',
  styleUrls: ['./signup.scss']
})
export class SignupComponent {
  name = '';
  email = '';
  password = '';
  loading = false;
  error: string | null = null;

  constructor(private router: Router, private auth: AuthService) {}

  onSubmit() {
    this.loading = true;
    this.error = null;
    const body = { name: this.name, email: this.email, password: this.password };
    this.auth.register(body).subscribe({
      next: () => {
        this.router.navigateByUrl('/login');
        this.loading = false;
      },
      error: (err) => {
        const message = err?.error?.message || err?.message || 'Registration failed';
        this.error = message;
        this.loading = false;
      }
    });
  }
}
