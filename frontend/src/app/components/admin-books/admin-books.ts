import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BookService, BookPayload } from '../../services/book-service';
import { catchError, of } from 'rxjs';
import { Book } from '../../interfaces/Book';

@Component({
  selector: 'app-admin-books',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-books.html',
  styleUrl: './admin-books.scss',
})
export class AdminBooksComponent {
  private bookService = inject(BookService);
  page = signal(1);
  pageSize = signal(10);
  hasNext = signal(false);
  hasPrev = signal(false);
  books = signal<any[]>([]);
  loading = signal(false);
  error = signal('');
  query = signal('');

  newBook: BookPayload = { author: '', title: '', publishedYear: '', copies: 0 };
  adding = false;
  deletingIds: Array<number | string> = [];

  editing = false;
  editBook: any = null;
  updating = false;

  ngOnInit() {
    this.loadBooks();
  }

  getId(b: any) {
    return b?.Id ?? b?.id ?? b?.bookId ?? b?.BookId ?? '';
  }

  loadBooks(page: number = 1, size: number = 10, title?: any) {
    this.loading.set(true);
    this.error.set('');
    this.page.set(page);
    this.pageSize.set(size);
    if (title !== undefined) {
      this.query.set(title);
    }
    this.bookService
      .getBooks(this.page(), this.pageSize(), this.query())
      .pipe(
        catchError((err) => {
          console.error('Error loading books:', err);
          this.error.set('Failed to load books');
          this.loading.set(false);
          return of({ data: { books: [], hasNext: false, hasPrev: false } });
        })
      )
      .subscribe((response) => {
        const data = response.data ?? {};

        // Example: filter or transform if needed
        const allBooks = data.books ?? [];

        this.books.set(allBooks);
        this.hasNext.set(data.hasNext ?? false);
        this.hasPrev.set(data.hasPrev ?? false);

        this.loading.set(false);
      });
  }
  onAdd() {
    this.adding = true;
    this.bookService.createBook(this.newBook).subscribe({
      next: (res) => {
        this.adding = false;
        this.newBook = { author: '', title: '', publishedYear: '', copies: 0 };
        this.loadBooks();
      },
      error: () => {
        this.adding = false;
      },
    });
  }

  startEdit(b: any) {
    this.editing = true;
    this.editBook = { ...b };
  }

  cancelEdit() {
    this.editing = false;
    this.editBook = null;
  }

  onUpdate() {
    if (!this.editBook) return;
    this.updating = true;
    const id = this.getId(this.editBook);
    const payload: Partial<BookPayload> = {
      author: this.editBook.author ?? this.editBook.Author,
      title: this.editBook.title ?? this.editBook.Title,
      publishedYear: this.editBook.publishedYear ?? this.editBook.PublishedYear,
      copies: Number(this.editBook.copies ?? this.editBook.Copies ?? 0),
    };
    this.bookService.updateBook(id, payload).subscribe({
      next: () => {
        this.updating = false;
        this.editing = false;
        this.editBook = null;
        this.loadBooks();
      },
      error: () => {
        this.updating = false;
      },
    });
  }

  onDelete(b: any) {
    const id = this.getId(b);
    if (!id) return;
    this.deletingIds = [...this.deletingIds, id];
    this.bookService.deleteBook(id).subscribe({
      next: () => {
        this.deletingIds = this.deletingIds.filter((i) => i !== id);
        this.loadBooks();
      },
      error: () => {
        this.deletingIds = this.deletingIds.filter((i) => i !== id);
      },
    });
  }

  nextPage() {
    if (!this.hasNext()) return;
    this.page.set(this.page() + 1);
    this.loadBooks(this.page(), this.pageSize());
  }
  prevPage() {
    if (!this.hasPrev()) return;
    this.page.set(Math.max(1, this.page() - 1));
    this.loadBooks(this.page(), this.pageSize());
  }
  onPageSizeChange() {
    this.pageSize.set(Number(this.pageSize()) || 5);
    this.loadBooks(1, this.pageSize());
    this.page.set(1);
  }
}
