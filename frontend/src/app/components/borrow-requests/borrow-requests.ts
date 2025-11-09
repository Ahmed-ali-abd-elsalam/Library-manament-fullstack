import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { BorrowService } from '../../services/borrow.service';
import { BorrowRequests } from '../../interfaces/BorrowRequests';

@Component({
  selector: 'app-borrow-requests',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './borrow-requests.html',
  styleUrl: './borrow-requests.scss',
})
export class BorrowRequestsComponent {
  private borrowService = inject(BorrowService);

  loading = signal(false);
  error = signal('');

  listLoading = signal(false);
  listError = signal('');
  requests = signal<BorrowRequests[]>([]);
  pageIndex = signal(0);
  pageSize = signal(10);
  hasNext = signal(false);
  hasPrev = signal(false);

  // processing ids for action buttons
  processing = signal<number[]>([]);

  ngOnInit() {
    this.loading.set(false);
    this.loadRequests();
  }

  loadRequests() {
    this.listLoading.set(true);
    this.listError.set('');
    this.borrowService.getAll(this.pageIndex(), this.pageSize()).subscribe({
      next: (res) => {
        const list =
          res?.borrowRecords ??
          res?.items ??
          res?.data?.borrowRecords ??
          (Array.isArray(res) ? res : []);
        this.requests.set(Array.isArray(list) ? list : []);
        this.pageIndex.set(Number(res?.offset ?? this.pageIndex()));
        this.pageSize.set(Number(res?.pageSize ?? this.pageSize()));
        this.hasNext.set(Boolean(res?.hasNext ?? false));
        this.hasPrev.set(Boolean(res?.hasPrev ?? false));
        this.listLoading.set(false);
      },
      error: () => {
        this.listError.set('Failed to load borrow requests');
        this.listLoading.set(false);
      },
    });
  }

  isPending(item: any): boolean {
    return String(item?.status ?? item?.Status ?? '').toLowerCase() === 'pending';
  }

  isProcessing(item: any): boolean {
    const id = Number(item?.Id ?? item?.id ?? item?.Id);
    return this.processing().includes(id);
  }

  onAction(item: any, status: string) {
    const id = Number(item?.Id ?? item?.id ?? item?.Id);
    if (!id) return;
    // add to processing
    this.processing.set([...this.processing(), id]);
    this.borrowService.patchStatus(id, status).subscribe({
      next: () => {
        this.updateRequestStatus(id, status);
        // remove from processing
        this.processing.set(this.processing().filter((i) => i !== id));
      },
      error: () => {
        this.listError.set('Failed to update request');
        this.processing.set(this.processing().filter((i) => i !== id));
      },
    });
  }

  updateRequestStatus(id: number, status: string) {
    const list = [...this.requests()];
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
    this.requests.set(list);
  }

  nextPage() {
    if (!this.hasNext()) return;
    this.pageIndex.set(this.pageIndex() + 1);
    this.loadRequests();
  }

  prevPage() {
    if (!this.hasPrev()) return;
    this.pageIndex.set(Math.max(0, this.pageIndex() - 1));
    this.loadRequests();
  }

  onPageSizeChange(value: string | number) {
    const size = Number(value) || 10;
    this.pageSize.set(size);
    this.pageIndex.set(0);
    this.loadRequests();
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
}
