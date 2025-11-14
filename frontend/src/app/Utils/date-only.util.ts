export function toDateOnlyString(value: string | Date): string {
  if (!value) return '';

  const date = value instanceof Date ? value : new Date(value);

  // Ensure valid date
  if (isNaN(date.getTime())) return '';

  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');

  return `${year}-${month}-${day}`; // DateOnly format
}
