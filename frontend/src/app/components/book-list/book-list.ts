import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BookService } from '../../services/book-service';
import { catchError, of } from 'rxjs';
import { Book } from '../../interfaces/Book';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

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

  isLoggedIn = computed(() => !!this.authService.getToken());

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

  loadBooks(page: number = 1, size: number = 10, title?: any) {
    /* sets books , total pages , pagesize */
    this.bookService
      .getBooks(page, size, title)
      .pipe(
        catchError((err) => {
          console.error('Error loading books:', err);
          return of({ data: { books: [], total: 0, offset: 0 } });
        })
      )
      .subscribe((response) => {
        const data = response.data;

        this.books.set(data?.books || []);
        const totalBooks = Number(data?.total ?? 0);
        const calculatedTotalPages = Math.max(
          1,
          Math.ceil(totalBooks / Number(size || this.pageSize))
        );
        this.totalPages.set(calculatedTotalPages);

        const offsetVal = Number(data?.offset ?? (page - 1) * size);
        const currentPageNum = Number.isFinite(offsetVal)
          ? Math.floor(offsetVal / Number(size || this.pageSize)) + 1
          : page;
        this.currentPage.set(currentPageNum);
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

  borrowBook(bookId: number) {
    if (!this.isLoggedIn()) {
    }
    console.log('Borrowing book:', bookId);
  }
}
