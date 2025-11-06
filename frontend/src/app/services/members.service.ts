import { Injectable, signal, computed } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { Member } from '../interfaces/Member';

export interface MembersQuery {
  UserName?: string;
  Email?: string;
  PhoneNumber?: string;
  offset?: number;
  pagesize?: number;
}

export interface CreateMemberPayload {
  name: string;
  email: string;
  password: string;
}

@Injectable({ providedIn: 'root' })
export class MembersService {
  private readonly baseUrl = 'https://localhost:7205/api/members';

  constructor(private http: HttpClient) {}

  getMembers(query: MembersQuery = {}): Observable<Member[]> {
    let params = new HttpParams();

    if (query.UserName) params = params.set('UserName', query.UserName);
    if (query.Email) params = params.set('Email', query.Email);
    if (query.PhoneNumber) params = params.set('PhoneNumber', query.PhoneNumber);
    if (query.offset !== undefined && query.offset !== null)
      params = params.set('offset', String(query.offset));
    if (query.pagesize !== undefined && query.pagesize !== null)
      params = params.set('pagesize', String(query.pagesize));

    return this.http.get<any>(this.baseUrl, { params }).pipe(
      map((res) => {
        const data = res?.data ?? res?.items ?? res;
        if (Array.isArray(data)) return data as Member[];
        if (Array.isArray(data?.items)) return data.items as Member[];
        return [] as Member[];
      })
    );
  }

  createMember(payload: CreateMemberPayload): Observable<void> {
    return this.http.post<any>(this.baseUrl, payload).pipe(map(() => void 0));
  }

  getMemberByEmail(email: string): Observable<any> {
    const url = `${this.baseUrl}/${encodeURIComponent(email)}`;
    return this.http.get<any>(url).pipe(map((res) => res?.data ?? res));
  }

  getMe(): Observable<any> {
    const url = `${this.baseUrl}/me`;
    return this.http.get<any>(url).pipe(map((res) => res?.data ?? res));
  }
}
