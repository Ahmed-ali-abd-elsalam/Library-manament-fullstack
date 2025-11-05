import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-signup-confirmation',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './signup-confirmation.html',
  styleUrls: ['./signup-confirmation.scss'],
})
export class SignupConfirmationComponent {}
