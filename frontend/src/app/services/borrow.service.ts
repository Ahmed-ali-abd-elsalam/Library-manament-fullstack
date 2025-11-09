import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class BorrowService {
  private readonly baseUrl = 'https://localhost:7205/api/borrowBooks';
  private http = inject(HttpClient);

  getAll(offset: number = 0, pagesize: number = 10) {
    let params = new HttpParams().set('offset', String(offset)).set('pagesize', String(pagesize));
    const url = `${this.baseUrl}/All`;
    return this.http.get<any>(url, { params });
  }

  getMy(offset: number = 0, pagesize: number = 10) {
    let params = new HttpParams().set('offset', String(offset)).set('pagesize', String(pagesize));
    const url = `${this.baseUrl}/me`;
    return this.http.get<any>(url, { params });
  }

  patchStatus(id: number | string, status: string) {
    const url = `${this.baseUrl}/${encodeURIComponent(String(id))}`;
    const params = new HttpParams().set('status', String(status));
    return this.http.patch<any>(url, null, { params });
  }

  borrowRequest(bookId: number | string, borrowDuration: number) {
    const url = `${this.baseUrl}/borrowrequest/${encodeURIComponent(String(bookId))}`;
    const params = new HttpParams().set('borrowDuration', String(borrowDuration));
    // POST with empty body
    return this.http.post<any>(url, null, { params });
  }
}
