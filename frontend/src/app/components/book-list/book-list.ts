import { NotificationService } from './../../services/notification-service';
import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BookService } from '../../services/book-service';
import { catchError, forkJoin, of } from 'rxjs';
import { Book } from '../../interfaces/Book';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { BorrowService } from '../../services/borrow.service';
import { BorrowRequests } from '../../interfaces/BorrowRequests';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-book-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './book-list.html',
  styleUrls: ['./book-list.scss'],
})
export class BookListComponent {
  books = signal<Array<Book>>([]);
  searchQuery = '';
  pageSize = 5;
  currentPage = signal(1);
  totalPages = signal(1);

  bookService = inject(BookService);
  authService = inject(AuthService);
  borrowService = inject(BorrowService);
  NotifService = inject(NotificationService);
  borrowedBooks = signal<BorrowRequests[]>([]);

  isLoggedIn = computed(() => !!this.authService.getToken());
  isAdmin() {
    return this.authService.isAdmin();
  }
  filteredBooks = computed(() => {
    const query = this.searchQuery.toLowerCase().trim();
    if (!query) {
      return this.books();
    }
    return this.books().filter(
      (book) =>
        book.title.toLowerCase().includes(query) ||
        book.author.toLowerCase().includes(query) ||
        book.publishedYear.toString().includes(query)
    );
  });

  // local UI state for showing duration input and processing
  showDurationFor = signal<number | null>(null);
  durationValue = signal<number | null>(2);
  processingBorrow = signal<number[]>([]);

  visiblePages = computed(() => {
    const total = this.totalPages();
    const current = this.currentPage();
    const pages: number[] = [];

    if (total <= 7) {
      for (let i = 1; i <= total; i++) {
        pages.push(i);
      }
    } else {
      if (current <= 4) {
        for (let i = 1; i <= 5; i++) pages.push(i);
        pages.push(total);
      } else if (current >= total - 3) {
        pages.push(1);
        for (let i = total - 4; i <= total; i++) pages.push(i);
      } else {
        pages.push(1);
        for (let i = current - 1; i <= current + 1; i++) pages.push(i);
        pages.push(total);
      }
    }

    return pages;
  });

  ngOnInit() {
    this.loadBooks(1, this.pageSize);
  }

  loadBooks(page: number = 1, size: number = 10, title?: string) {
    const borrowedCached = this.borrowedBooks().length > 0;

    forkJoin({
      booksResponse: this.bookService.getBooks(page, size, title).pipe(
        catchError((err: HttpErrorResponse) => {
          console.error('Error loading books:', err);
          return of({ data: { books: [], total: 0, offset: 0 } });
        })
      ),
      borrowedResponse: borrowedCached
        ? of({ data: this.borrowedBooks() })
        : this.borrowService.getMy(0, 1000).pipe(
            catchError((err: HttpErrorResponse) => {
              console.error('Error loading borrowed books:', err);
              return of({ data: [] });
            })
          ),
    }).subscribe(({ booksResponse, borrowedResponse }) => {
      const books = booksResponse.data.books || [];
      const borrowed: BorrowRequests[] = borrowedResponse.data.borrowRecords || [];

      // Cache borrowed books if not already cached
      if (!borrowedCached)
        this.borrowedBooks.set(
          borrowed.filter(
            (b) => b.status === 'approved' || b.status === 'late' || b.status === 'pending'
          )
        );

      // Filter out borrowed books with restricted statuses
      const restrictedStatuses = ['Pending', 'Approved', 'Late'];
      const updatedBooks = books.map((book: any) => {
        const isBorrowed = borrowed.some(
          (b) => b.bookId === book.id && restrictedStatuses.includes(b.status)
        );

        // available only if has copies AND not borrowed with restricted status
        const isAvailable = !isBorrowed && (book.copiesAvailable ?? book.copies) > 0;

        return {
          ...book,
          available: isAvailable,
        };
      });

      this.books.set(updatedBooks);
      // handle pagination (same as before)
      const totalBooks = Number(booksResponse.data?.total ?? 0);
      const totalPages = Math.max(1, Math.ceil(totalBooks / size));
      this.totalPages.set(totalPages);
      this.currentPage.set(page);
    });
  }

  onSearch(title?: string) {
    const query = title ?? this.searchQuery;
    this.loadBooks(1, this.pageSize, query);
    this.currentPage.set(1);
  }

  goToPage(page: number) {
    if (page != this.currentPage() && page <= this.totalPages()) {
      this.currentPage.set(page);
      this.loadBooks(page, this.pageSize);
    }
  }

  onPageSizeChange() {
    // ensure pageSize is numeric (ngModel may pass string)
    this.pageSize = Number(this.pageSize) || 5;
    // reload starting from page 1 with the new size
    this.loadBooks(1, this.pageSize);
    this.currentPage.set(1);
  }
  isBorrowed(bookId: number): boolean {
    return this.borrowedBooks().some((b) => b.bookId === bookId);
  }

  // Update the borrowBook method to prevent borrowing already borrowed books
  borrowBook(bookId: number) {
    if (!this.isLoggedIn() || this.isBorrowed(bookId)) return;

    // toggle input
    if (this.showDurationFor() === bookId) {
      this.showDurationFor.set(null);
    } else {
      this.showDurationFor.set(bookId);
      this.durationValue.set(1);
    }
  }

  submitBorrow(bookId: number) {
    const dur = Number(this.durationValue() ?? 0);
    if (!dur || dur <= 0) return;
    this.processingBorrow.set([...this.processingBorrow(), bookId]);
    this.borrowService.borrowRequest(bookId, dur).subscribe({
      next: (res) => {
        const newBorrow: BorrowRequests = res.data; // expect backend to return full borrow request object

        // Hide duration field and mark processing as done
        this.showDurationFor.set(null);
        this.processingBorrow.update((curr) => curr.filter((id) => id !== bookId));

        // Add new borrowed book to cache
        this.borrowedBooks.update((current) => [...current, newBorrow]);

        // Remove borrowed book from available list (no need to reload all)
        this.books.update((current) => current.filter((b) => b.id !== bookId));
      },
      error: () => {
        this.processingBorrow.set(this.processingBorrow().filter((i) => i !== bookId));
      },
    });
  }

  isProcessing(bookId: number) {
    return this.processingBorrow().includes(bookId);
  }
}
