import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./components/book-list/book-list').then((m) => m.BookListComponent),
  },
  {
    path: 'todo',
    loadComponent: () =>
      import('./components/book-list/book-list').then((m) => m.BookListComponent),
  },
  {
    path: 'login',
    loadComponent: () =>
      import('./components/auth/login/login').then((m) => m.LoginComponent),
  },
  {
    path: 'signup',
    loadComponent: () =>
      import('./components/auth/signup/signup').then((m) => m.SignupComponent),
  },
];
