export interface BorrowRequests {
  id: number;
  borrowDate: string | null;
  returnDate: string | null;
  borrowDuration: number;
  bookId: number;
  bookTitle: string;
  stockCopies: number;
  memberId: string;
  email: string;
  lateReturns: number;
  status: string;
}
