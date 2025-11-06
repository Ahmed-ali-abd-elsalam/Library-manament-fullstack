import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MembersService } from '../../services/members.service';

@Component({
  selector: 'app-me-profile',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './me-profile.html',
  styleUrl: './me-profile.scss',
})
export class MeProfileComponent {
  private membersService = inject(MembersService);

  loading = signal(false);
  error = signal('');
  me = signal<any | null>(null);

  ngOnInit() {
    this.loading.set(true);
    this.membersService.getMe().subscribe({
      next: (data) => {
        this.me.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load profile');
        this.loading.set(false);
      },
    });
  }

  keys(obj: any): string[] {
    if (!obj || typeof obj !== 'object') return [];
    return Object.keys(obj);
  }
}
