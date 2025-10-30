import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

interface Book {
  id: number;
  title: string;
  author: string;
  year: number;
  coverUrl: string;
  rating?: number;
}

@Component({
  selector: 'app-book-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './book-list.html',
  styleUrls: ['./book-list.scss']
})
export class BookListComponent {
  books = signal<Book[]>([
    {
      id: 1,
      title: 'The Great Gatsby',
      author: 'F. Scott Fitzgerald',
      year: 1925,
      coverUrl: 'https://covers.openlibrary.org/b/id/7222246-L.jpg',
      rating: 4.5
    },
    {
      id: 2,
      title: 'To Kill a Mockingbird',
      author: 'Harper Lee',
      year: 1960,
      coverUrl: 'https://covers.openlibrary.org/b/id/8228691-L.jpg',
      rating: 4.8
    },
    {
      id: 3,
      title: '1984',
      author: 'George Orwell',
      year: 1949,
      coverUrl: 'https://covers.openlibrary.org/b/id/7222246-L.jpg',
      rating: 4.7
    }
  ]);
}
