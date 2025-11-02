import { Component, inject, NgModule, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BookService } from '../../services/book-service';
import { catchError } from 'rxjs';
import { Book } from '../../interfaces/Book';
import { FormsModule } from '@angular/forms';
@Component({
  selector: 'app-book-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './book-list.html',
  styleUrls: ['./book-list.scss'],
})
export class BookListComponent {
  books = signal<Array<Book>>([]);
  pagination = signal<any>({});
  bookService = inject(BookService);
  ngOnInit() {
    this.bookService
      .getBooks()
      .pipe(
        catchError((err) => {
          console.log(err);
          throw err;
        })
      )
      .subscribe((m) => {
        console.log(m);
        const data = m.data;
        const pages = Array.from(
          { length: Math.ceil(data.total / data.pageSize) },
          (_, i) => i + 1
        );
        console.log({
          prev: data.hasPrev,
          next: data.hasNext,
          pages: pages,
          currPage: Math.floor(data.offset / data.pageSize) + 1,
        });

        this.pagination.set({
          prev: data.hasPrev,
          next: data.hasNext,
          pages: pages,
          currPage: data.offset / data.pageSize,
        });
        console.log(data?.books);
        return this.books.set(data?.books);
      });
  }
}
