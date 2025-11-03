import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './signup.html',
  styleUrls: ['./signup.scss'],
})
export class SignupComponent {
  name = '';
  email = '';
  password = '';
  loading = false;
  success = signal<boolean>(false);
  error = signal<string>('');

  constructor(private router: Router, private auth: AuthService) {}

  onSubmit() {
    this.loading = true;
    const body = { name: this.name, email: this.email, password: this.password };
    this.auth.register(body).subscribe({
      next: () => {
        this.success.set(true);
        this.loading = false;
        setTimeout(() => {
          this.router.navigateByUrl('/login');
        }, 5000);
      },
      error: (err) => {
        console.log(err);
        const message = err?.error.error.error || err.error?.Name[0] || 'Registration failed';
        this.error.set(message);
        this.loading = false;
      },
    });
  }
}
