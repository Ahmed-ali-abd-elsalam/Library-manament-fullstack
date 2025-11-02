import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class BookService {
  private readonly loginUrl = 'https://localhost:7205/api/Auth/login';
  private readonly registerUrl = 'https://localhost:7205/api/Auth/register';

}
