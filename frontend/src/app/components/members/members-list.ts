import { CommonModule } from '@angular/common';
import { Component, effect, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { MembersService, MembersQuery, CreateMemberPayload } from '../../services/members.service';
import { Member } from '../../interfaces/Member';

@Component({
  selector: 'app-members-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './members-list.html',
  styleUrl: './members-list.scss',
})
export class MembersListComponent {
  private membersService = inject(MembersService);
  private router = inject(Router);

  // Search state
  userName = '';
  email = '';
  phoneNumber = '';
  pageIndex = signal(0);
  pageSize = signal(5);
  next = signal(false);
  prev = signal(false);

  // Data state
  loading = signal(false);
  error = signal('');
  members = signal<Member[]>([]);

  // Create member form
  newName = '';
  newEmail = '';
  newPassword = '';
  createError = signal('');
  createSuccess = signal('');

  ngOnInit() {
    this.loadMembers();
  }

  buildQuery(): MembersQuery {
    const q: MembersQuery = {};
    if (this.userName.trim()) q.UserName = this.userName.trim();
    if (this.email.trim()) q.Email = this.email.trim();
    if (this.phoneNumber.trim()) q.PhoneNumber = this.phoneNumber.trim();
    q.offset = this.pageIndex();
    q.pagesize = this.pageSize();
    return q;
  }

  loadMembers() {
    this.loading.set(true);
    this.error.set('');
    this.membersService.getMembers(this.buildQuery()).subscribe({
      next: (data) => {
        const allMembers =
          data?.members.filter((m: Member) => m.email !== 'admin@library.com') ?? [];
        this.members.set(allMembers);
        this.pageIndex.set(data?.offset);
        this.pageSize.set(data?.pageSize);
        this.next.set(data?.hasNext ?? false);
        this.prev.set(data?.hasPrev ?? false);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Failed to load members');
        this.loading.set(false);
      },
    });
  }

  onSearch() {
    this.pageIndex.set(0);
    this.loadMembers();
  }

  onReset() {
    this.userName = '';
    this.email = '';
    this.phoneNumber = '';
    this.pageIndex.set(0);
    this.pageSize.set(5);
    this.loadMembers();
  }

  onPageSizeChange(value: number | string) {
    this.pageSize.set(Number(value));
    this.pageIndex.set(0);
    this.loadMembers();
  }

  nextPage() {
    this.pageIndex.set(this.pageIndex() + 1);
    this.loadMembers();
  }

  prevPage() {
    this.pageIndex.set(Math.max(0, this.pageIndex() - 1));
    this.loadMembers();
  }

  createMember() {
    this.createError.set('');
    this.createSuccess.set('');
    const payload: CreateMemberPayload = {
      name: this.newName,
      email: this.newEmail,
      password: this.newPassword,
    };

    if (!payload.name || !payload.email || !payload.password) {
      this.createError.set('All fields are required');
      return;
    }

    this.membersService.createMember(payload).subscribe({
      next: () => {
        this.createSuccess.set('Member created successfully');
        this.newName = '';
        this.newEmail = '';
        this.newPassword = '';
        this.loadMembers();
      },
      error: () => this.createError.set('Failed to create member'),
    });
  }
}
