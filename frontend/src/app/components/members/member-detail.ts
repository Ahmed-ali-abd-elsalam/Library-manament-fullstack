import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { MembersService } from '../../services/members.service';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './member-detail.html',
  styleUrl: './member-detail.scss',
})
export class MemberDetailComponent {
  private route = inject(ActivatedRoute);
  private membersService = inject(MembersService);

  loading = signal(false);
  error = signal('');
  member = signal<any | null>(null);

  ngOnInit() {
    const email = this.route.snapshot.paramMap.get('email') || '';
    if (!email) return;
    this.loading.set(true);
    this.membersService.getMemberByEmail(email).subscribe({
      next: (data) => {
        this.member.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load member');
        this.loading.set(false);
      },
    });
  }

  keys(obj: any): string[] {
    if (!obj || typeof obj !== 'object') return [];
    return Object.keys(obj);
  }
}
