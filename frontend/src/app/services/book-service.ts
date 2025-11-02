import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class BookService {
  private readonly baseUrl = 'https://localhost:7205/api/books';
  http = inject(HttpClient);
  getBooks() {
    return this.http.get<any>(this.baseUrl);
  }
}
