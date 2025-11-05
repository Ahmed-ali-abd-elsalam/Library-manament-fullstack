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
];
