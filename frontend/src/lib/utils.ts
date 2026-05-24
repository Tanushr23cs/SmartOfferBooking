export function formatCurrency(amount: number): string {
  return new Intl.NumberFormat('en-IN', { style: 'currency', currency: 'INR', maximumFractionDigits: 0 }).format(amount);
}

export function formatDate(date: string): string {
  return new Date(date).toLocaleDateString('en-IN', { day: 'numeric', month: 'short', year: 'numeric' });
}

export function formatTime(time: string): string {
  if (!time) return '';
  const [h, m] = time.split(':');
  const hour = parseInt(h, 10);
  const ampm = hour >= 12 ? 'PM' : 'AM';
  const h12 = hour % 12 || 12;
  return `${h12}:${m} ${ampm}`;
}

export function getCountdown(endDate: string): { days: number; hours: number; minutes: number; seconds: number; expired: boolean } {
  const end = new Date(endDate + 'T23:59:59');
  const now = new Date();
  const diff = end.getTime() - now.getTime();
  if (diff <= 0) return { days: 0, hours: 0, minutes: 0, seconds: 0, expired: true };
  return {
    days: Math.floor(diff / (1000 * 60 * 60 * 24)),
    hours: Math.floor((diff / (1000 * 60 * 60)) % 24),
    minutes: Math.floor((diff / (1000 * 60)) % 60),
    seconds: Math.floor((diff / 1000) % 60),
    expired: false,
  };
}

export function cn(...classes: (string | false | undefined | null)[]): string {
  return classes.filter(Boolean).join(' ');
}

/** Normalise a time string from HH:mm (HTML input) to HH:mm:ss (backend). */
export function toApiTime(time: string): string {
  if (!time) return time;
  // Already in HH:mm:ss
  if (/^\d{2}:\d{2}:\d{2}$/.test(time)) return time;
  // HH:mm → HH:mm:00
  if (/^\d{2}:\d{2}$/.test(time)) return time + ':00';
  return time;
}

export const OFFER_CATEGORIES = [
  'Lunch Hour Deal',
  'Gym Trial Slot',
  'Salon Happy Hour',
  'Doctor Consultation Discount',
  'Turf Morning Slot Offer',
  'Coaching Demo Class',
  'Restaurant Special',
  'Other',
];
export const BUSINESS_TYPES = ['Restaurant', 'Gym', 'Salon', 'Clinic', 'Coaching', 'Turf', 'Other'];
export const OFFER_STATUSES = ['Draft', 'Active', 'Paused', 'Expired', 'Cancelled'] as const;
export const BOOKING_STATUSES = ['Pending', 'Confirmed', 'Cancelled', 'Completed', 'NoShow'] as const;
