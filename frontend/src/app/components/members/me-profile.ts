import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MembersService } from '../../services/members.service';
import { BorrowService } from '../../services/borrow.service';
import { NotificationService } from '../../services/notification-service';

@Component({
  selector: 'app-me-profile',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './me-profile.html',
  styleUrl: './me-profile.scss',
})
export class MeProfileComponent {
  private membersService = inject(MembersService);
  private borrowService = inject(BorrowService);
  private notificationService = inject(NotificationService);

  loading = signal(false);
  error = signal('');
  me = signal<any | null>(null);

  // borrow history
  requestsLoading = signal(false);
  requestsError = signal('');
  borrowRequests = signal<any[]>([]);
  pageIndex = signal(0);
  pageSize = signal(10);
  hasNext = signal(false);
  hasPrev = signal(false);

  ngOnInit() {
    this.loading.set(true);
    this.membersService.getMe().subscribe({
      next: (data) => {
        this.me.set(data);
        this.loading.set(false);
        this.loadBorrowHistory();
      },
      error: () => {
        this.error.set('Failed to load profile');
        this.loading.set(false);
      },
    });
  }

  loadBorrowHistory() {
    this.requestsLoading.set(true);
    this.requestsError.set('');
    this.borrowService.getMy(this.pageIndex(), this.pageSize()).subscribe({
      next: (res) => {
        const list = res?.borrowRecords ?? res?.items ?? res?.data?.borrowRecords ?? (Array.isArray(res) ? res : []);
        this.borrowRequests.set(Array.isArray(list) ? list : []);
        this.pageIndex.set(Number(res?.offset ?? this.pageIndex()));
        this.pageSize.set(Number(res?.pageSize ?? this.pageSize()));
        this.hasNext.set(Boolean(res?.hasNext ?? false));
        this.hasPrev.set(Boolean(res?.hasPrev ?? false));
        this.requestsLoading.set(false);
      },
      error: () => {
        this.requestsError.set('Failed to load borrow history');
        this.requestsLoading.set(false);
      },
    });
  }

  nextPage() {
    if (!this.hasNext()) return;
    this.pageIndex.set(this.pageIndex() + 1);
    this.loadBorrowHistory();
  }

  prevPage() {
    if (!this.hasPrev()) return;
    this.pageIndex.set(Math.max(0, this.pageIndex() - 1));
    this.loadBorrowHistory();
  }

  formatDate(val: any): string {
    if (!val) return '-';
    try {
      const d = new Date(val);
      if (isNaN(d as any)) return String(val);
      return d.toLocaleDateString();
    } catch {
      return String(val);
    }
  }

  returnBook(bookId: number) {
    this.borrowService.returnBorrowRequest(bookId).subscribe({
      next: () => {
        // this.notificationService.showSuccess('Book returned successfully');
        this.pageIndex.set(0);
        this.loadBorrowHistory();
      },
      error: (err) => {
        const errorMsg = err?.error?.message || 'Failed to return book';
        // this.notificationService.showError(errorMsg);
      },
    });
  }

  keys(obj: any): string[] {
    if (!obj || typeof obj !== 'object') return [];
    return Object.keys(obj);
  }
}
