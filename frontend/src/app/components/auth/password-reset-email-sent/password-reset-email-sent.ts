import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-password-reset-email-sent',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './password-reset-email-sent.html',
  styleUrls: ['./password-reset-email-sent.scss'],
})
export class PasswordResetEmailSentComponent {}
