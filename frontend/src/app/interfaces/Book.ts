export interface Book {
  id: number;
  title: string;
  author: string;
  publishedYear: number;
  copies: number;
  copiesAvailable?: number;
  rating?: number;
  available?: boolean;
}
