import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

interface SearchQuery {
  Title?: string;
  Author?: string;
  PublishedYear?: number | string;
}

export interface BookPayload {
  author: string;
  title: string;
  publishedYear: string;
  copies: number;
}

@Injectable({
  providedIn: 'root',
})
export class BookService {
  private readonly baseUrl = 'https://localhost:7205/api/books';
  http = inject(HttpClient);

  // page: 1-based page number
  // size: page size
  // filter: either a search string (applied to Title) or a SearchQuery object
  getBooks(page: number = 1, size: number = 10, filter: SearchQuery | string = {}) {
    const offsetParam = Math.max(0, (page - 1) * size);
    let params = new HttpParams().set('offset', String(offsetParam)).set('count', String(size));
    if (typeof filter === 'string' && filter.trim().length) {
      params = params.set('Title', filter.trim());
    } else if (typeof filter === 'object' && filter != null) {
      const f = filter as SearchQuery;
      if (f.Author) params = params.set('Author', String(f.Author));
      if (f.Title) params = params.set('Title', String(f.Title));
      if (f.PublishedYear !== undefined && f.PublishedYear !== null)
        params = params.set('PublishedYear', String(f.PublishedYear));
    }
    return this.http.get<any>(this.baseUrl, { params });
  }

  BorrowBook(bookId: number) {
    // TODO
  }

  createBook(payload: BookPayload) {
    const url = `${this.baseUrl}/add`;
    return this.http.post<any>(url, payload);
  }

  updateBook(id: string | number, payload: Partial<BookPayload>) {
    const url = `${this.baseUrl}/${encodeURIComponent(String(id))}`;
    return this.http.put<any>(url, payload);
  }

  deleteBook(id: string | number) {
    const url = `${this.baseUrl}/${encodeURIComponent(String(id))}`;
    return this.http.delete<any>(url);
  }
}
