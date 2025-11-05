import { Component, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-password-reset',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './password-reset.html',
  styleUrls: ['./password-reset.scss'],
})
export class PasswordResetComponent {
  email = '';
  newPassword = '';
  confirmNewPassword = '';
  loading = false;
  error = signal<string>('');
  info = signal<string>('');

  private tokenId = '';
  private emailQuery = '';

  constructor(private route: ActivatedRoute, private router: Router, private auth: AuthService) {
    const params = this.route.snapshot.queryParamMap;
    this.tokenId = params.get('TokenId') || '';
    this.emailQuery = params.get('Email') || '';

    if (!this.tokenId || !this.emailQuery) {
      this.error.set(
        'Missing TokenId or Email in the link. Please use the password reset link from your email.'
      );
    }

    // Prefill email from query but allow editing the body email if user wants
    this.email = this.emailQuery;
  }

  get canSubmit() {
    return (
      !!this.email && !!this.newPassword && !!this.confirmNewPassword
      // this.newPassword === this.confirmNewPassword &&
      // !this.loading &&
      // !this.error()
    );
  }

  onSubmit() {
    if (!this.canSubmit) return;
    if (this.newPassword !== this.confirmNewPassword) {
      this.error.set("passwords don't match");
      return;
    }
    this.loading = true;
    this.error.set('');
    this.info.set('');

    const body = {
      email: this.email,
      newPassword: this.newPassword,
      confirmNewPassword: this.confirmNewPassword,
    };

    this.auth.resetPassword({ tokenId: this.tokenId, email: this.emailQuery, body }).subscribe({
      next: () => {
        this.loading = false;
        this.info.set('Password updated successfully. You can now sign in.');
        setTimeout(() => this.router.navigateByUrl('/login'), 3000);
      },
      error: (err) => {
        this.loading = false;
        const message =
          err?.error?.error?.error ||
          err?.error?.message ||
          'Password reset failed. Please try again.';
        this.error.set(message);
      },
    });
  }
}
