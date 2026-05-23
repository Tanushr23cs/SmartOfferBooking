export type OfferStatus = 'Draft' | 'Active' | 'Paused' | 'Expired' | 'Cancelled';
export type SlotStatus = 'Available' | 'Full' | 'Closed' | 'Expired' | 'Cancelled';
export type BookingStatus = 'Pending' | 'Confirmed' | 'Cancelled' | 'Completed' | 'NoShow';

export interface ApiResponse<T> {
  success: boolean;
  message?: string;
  data?: T;
  errors?: string[];
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface LoginResponse {
  token: string;
  expiresAt: string;
  username: string;
  email: string;
  role: string;
}

export interface BusinessProfile {
  id: string;
  businessName: string;
  businessType: string;
  ownerName: string;
  phone: string;
  email: string;
  address: string;
  city: string;
  logoUrl?: string;
  openingTime: string;
  closingTime: string;
}

export interface Offer {
  id: string;
  title: string;
  description: string;
  category: string;
  originalPrice: number;
  offerPrice: number;
  discountPercentage: number;
  startDate: string;
  endDate: string;
  startTime: string;
  endTime: string;
  totalCapacity: number;
  maxBookingPerCustomer: number;
  termsAndConditions?: string;
  status: OfferStatus;
}

export interface OfferPublic extends Offer {
  businessName: string;
  businessType: string;
  logoUrl?: string;
  city: string;
  address?: string;
  availableSlotsCount: number;
  totalAvailableSeats: number;
}

export interface Slot {
  id: string;
  offerId: string;
  slotDate: string;
  startTime: string;
  endTime: string;
  capacity: number;
  bookedCount: number;
  availableCount: number;
  status: SlotStatus;
}

export interface Booking {
  id: string;
  bookingReference: string;
  offerId: string;
  slotId: string;
  customerName: string;
  customerPhone: string;
  customerEmail?: string;
  peopleCount: number;
  specialNote?: string;
  status: BookingStatus;
  createdAt: string;
}

export interface BookingDetail extends Booking {
  offerTitle: string;
  businessName: string;
  slotDate: string;
  slotStartTime: string;
  slotEndTime: string;
  address?: string;
  city?: string;
}

export interface DashboardSummary {
  totalOffers: number;
  activeOffers: number;
  totalBookings: number;
  todaysBookings: number;
  totalCapacity: number;
  bookedSeats: number;
  availableSeats: number;
  conversionRate: number;
}

export interface SlotUpdatedEvent {
  slotId: string;
  offerId: string;
  bookedCount: number;
  availableCount: number;
  status: SlotStatus;
  isFull: boolean;
}

export interface OfferUpdatedEvent {
  offerId: string;
  title: string;
  status: OfferStatus;
  offerPrice: number;
  originalPrice: number;
  totalAvailableSeats: number;
  updatedAt: string;
}
