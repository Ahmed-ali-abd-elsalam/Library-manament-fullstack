import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { MembersService } from '../../services/members.service';
import { BorrowService } from '../../services/borrow.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './member-detail.html',
  styleUrl: './member-detail.scss',
})
export class MemberDetailComponent {
  private route = inject(ActivatedRoute);
  private membersService = inject(MembersService);
  private borrowService = inject(BorrowService);

  loading = signal(false);
  error = signal('');
  member = signal<any | null>(null);

  // Borrow requests state
  requestsLoading = signal(false);
  requestsError = signal('');
  borrowRequests = signal<any[]>([]);
  reqPageIndex = signal(0);
  reqPageSize = signal(10);
  reqHasNext = signal(false);
  reqHasPrev = signal(false);
  memberKey = signal<string>('');

  processing = signal<number[]>([]);

  ngOnInit() {
    const email = this.route.snapshot.paramMap.get('email') || '';
    if (!email) return;
    this.loading.set(true);
    this.membersService.getMemberByEmail(email).subscribe({
      next: (data) => {
        this.member.set(data);
        const key = data.email ?? '';
        console.log(key);

        this.memberKey.set(key);
        this.loading.set(false);
        if (key) this.loadBorrowRequests();
      },
      error: () => {
        this.error.set('Failed to load member');
        this.loading.set(false);
      },
    });
  }

  computeMemberKey(m: any): string {
    const id = (m && (m.id ?? m.Id ?? m.memberId ?? m.MemberId ?? m.email ?? m.Email)) as
      | string
      | number
      | undefined;
    return id !== undefined && id !== null ? String(id) : '';
  }

  loadBorrowRequests() {
    const key = this.memberKey();
    if (!key) return;
    this.requestsLoading.set(true);
    this.requestsError.set('');
    this.membersService
      .getBorrowRequestsForMember(key, this.reqPageIndex(), this.reqPageSize())
      .subscribe({
        next: (res) => {
          const list = res.borrowRecords;
          this.borrowRequests.set(Array.isArray(list) ? list : []);
          this.reqPageIndex.set(Number(res?.offset ?? this.reqPageIndex()));
          this.reqPageSize.set(Number(res?.pageSize ?? this.reqPageSize()));
          this.reqHasNext.set(Boolean(res?.hasNext ?? false));
          this.reqHasPrev.set(Boolean(res?.hasPrev ?? false));
          this.requestsLoading.set(false);
        },
        error: () => {
          this.requestsError.set('Failed to load borrow requests');
          this.requestsLoading.set(false);
        },
      });
  }

  isPending(item: any): boolean {
    return String(item?.status ?? item?.Status ?? '').toLowerCase() === 'pending';
  }

  isProcessing(item: any): boolean {
    const id = Number(item?.Id ?? item?.id);
    return this.processing().includes(id);
  }

  onAction(item: any, status: string) {
    const id = Number(item?.Id ?? item?.id);
    if (!id) return;
    this.processing.set([...this.processing(), id]);
    this.borrowService.patchStatus(id, status).subscribe({
      next: () => {
        this.updateRequestStatus(id, status);
        this.processing.set(this.processing().filter((i) => i !== id));
      },
      error: () => {
        this.requestsError.set('Failed to update request');
        this.processing.set(this.processing().filter((i) => i !== id));
      },
    });
  }

  // updateRequestStatus(id: number, status: string) {
  //   const list = [...this.borrowRequests()];
  //   const idx = list.findIndex((r) => Number(r?.Id ?? r?.id) === Number(id));
  //   if (idx === -1) return;
  //   list[idx] = { ...list[idx], status };
  //   this.borrowRequests.set(list);
  // }
  updateRequestStatus(id: number, status: string) {
    const list = [...this.borrowRequests()];
    const idx = list.findIndex((r) => Number(r?.id ?? r?.id) === Number(id));
    if (idx === -1) return;
    list[idx] = {
      ...list[idx],
      status,
      borrowDate: new Date().toISOString(),
      returnDate: new Date(
        Date.now() + list[idx].borrowDuration * 24 * 60 * 60 * 1000
      ).toISOString(),
    };
    const newStock = list[idx].stockCopies - 1;
    list.forEach((r, i) => {
      if (r.bookId === list[idx].bookId) list[i].stockCopies = newStock;
    });
    this.borrowRequests.set(list);
  }

  nextReqPage() {
    if (!this.reqHasNext()) return;
    this.reqPageIndex.set(this.reqPageIndex() + 1);
    this.loadBorrowRequests();
  }

  prevReqPage() {
    if (!this.reqHasPrev()) return;
    this.reqPageIndex.set(Math.max(0, this.reqPageIndex() - 1));
    this.loadBorrowRequests();
  }

  onReqPageSizeChange(value: string | number) {
    const size = Number(value) || 10;
    this.reqPageSize.set(size);
    this.reqPageIndex.set(0);
    this.loadBorrowRequests();
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

  keys(obj: any): string[] {
    if (!obj || typeof obj !== 'object') return [];
    return Object.keys(obj);
  }
}
