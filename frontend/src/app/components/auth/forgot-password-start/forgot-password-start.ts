import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-forgot-password-start',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './forgot-password-start.html',
  styleUrls: ['./forgot-password-start.scss'],
})
export class ForgotPasswordStartComponent {
  email = '';
  loading = false;
  error = signal<string>('');

  constructor(private auth: AuthService, private router: Router) {}

  onSubmit() {
    if (!this.email) {
      this.error.set('Please enter your email');
      return;
    }
    this.loading = true;
    this.error.set('');
    this.auth.forgetPasswordStart(this.email).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigateByUrl('/password-reset-email-sent');
      },
      error: (err) => {
        console.log(err);

        this.loading = false;
        const msg = err?.error?.message || err?.error || 'Request failed';
        this.error.set(msg);
      },
    });
  }
}
