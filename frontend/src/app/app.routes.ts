import { Routes } from '@angular/router';

import { adminGuard } from './guards/admin.guard';

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
  {
    path: 'passwordreset',
    loadComponent: () =>
      import('./components/auth/password-reset/password-reset').then((m) => m.PasswordResetComponent),
  },
  {
    path: 'forget-password-start',
    loadComponent: () =>
      import('./components/auth/forgot-password-start/forgot-password-start').then((m) => m.ForgotPasswordStartComponent),
  },
  {
    path: 'password-reset-email-sent',
    loadComponent: () =>
      import('./components/auth/password-reset-email-sent/password-reset-email-sent').then((m) => m.PasswordResetEmailSentComponent),
  },
  {
    path: 'signup-confirmation',
    loadComponent: () =>
      import('./components/auth/signup-confirmation/signup-confirmation').then((m) => m.SignupConfirmationComponent),
  },
  {
    path: 'members',
    canActivate: [adminGuard],
    loadComponent: () =>
      import('./components/members/members-list').then((m) => m.MembersListComponent),
  },
  {
    path: 'members/:email',
    canActivate: [adminGuard],
    loadComponent: () =>
      import('./components/members/member-detail').then((m) => m.MemberDetailComponent),
  },
  {
    path: 'me',
    loadComponent: () =>
      import('./components/members/me-profile').then((m) => m.MeProfileComponent),
  },
  {
    path: 'borrowed-books',
    loadComponent: () =>
      import('./components/borrow-requests/borrow-requests').then((m) => m.BorrowRequestsComponent),
  },
  {
    path: 'admin/books',
    canActivate: [adminGuard],
    loadComponent: () => import('./components/admin-books/admin-books').then((m) => m.AdminBooksComponent),
  },
];
