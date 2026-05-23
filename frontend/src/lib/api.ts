import axios from 'axios';
import type {
  ApiResponse,
  Booking,
  BookingDetail,
  BookingStatus,
  BusinessProfile,
  DashboardSummary,
  LoginResponse,
  Offer,
  OfferPublic,
  OfferStatus,
  PagedResult,
  Slot,
} from '@/types';

const API_URL = import.meta.env.VITE_API_URL || '';

export const api = axios.create({
  baseURL: API_URL,
  headers: { 'Content-Type': 'application/json' },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('auth_token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

api.interceptors.response.use(
  (res) => res,
  (error) => {
    if (!error.response) {
      return Promise.reject(
        new Error(
          'Cannot reach the API. Start the backend first: from SmartOfferBooking folder run .\\start-backend.ps1 (must listen on http://localhost:5000).'
        )
      );
    }
    const message = error.response?.data?.message || error.message || 'Request failed';
    return Promise.reject(new Error(message));
  }
);

function unwrap<T>(response: { data: ApiResponse<T> }): T {
  if (!response.data.success || response.data.data === undefined) {
    throw new Error(response.data.message || 'Request failed');
  }
  return response.data.data;
}

export const authApi = {
  login: async (email: string, password: string) => {
    const res = await api.post<ApiResponse<LoginResponse>>('/api/auth/login', { email, password });
    return unwrap(res);
  },
};

export const businessApi = {
  get: async () => {
    const res = await api.get<ApiResponse<BusinessProfile>>('/api/business');
    return unwrap(res);
  },
  create: async (data: Omit<BusinessProfile, 'id'>) => {
    const res = await api.post<ApiResponse<BusinessProfile>>('/api/business', data);
    return unwrap(res);
  },
  update: async (id: string, data: Omit<BusinessProfile, 'id'>) => {
    const res = await api.put<ApiResponse<BusinessProfile>>(`/api/business/${id}`, data);
    return unwrap(res);
  },
};

export interface OfferQuery {
  page?: number;
  pageSize?: number;
  search?: string;
  category?: string;
  businessType?: string;
  status?: OfferStatus;
  minPrice?: number;
  maxPrice?: number;
  availableOnly?: boolean;
  publicOnly?: boolean;
  slotDate?: string;
}

export const offersApi = {
  getPaged: async (query: OfferQuery = {}) => {
    const res = await api.get<ApiResponse<PagedResult<OfferPublic | Offer>>>('/api/offers', { params: query });
    return unwrap(res);
  },
  getById: async (id: string, publicView = false) => {
    const res = await api.get<ApiResponse<OfferPublic | Offer>>(`/api/offers/${id}`, {
      params: { publicView },
    });
    return unwrap(res);
  },
  create: async (data: Record<string, unknown>) => {
    const res = await api.post<ApiResponse<Offer>>('/api/offers', data);
    return unwrap(res);
  },
  update: async (id: string, data: Record<string, unknown>) => {
    const res = await api.put<ApiResponse<Offer>>(`/api/offers/${id}`, data);
    return unwrap(res);
  },
  delete: async (id: string) => {
    const res = await api.delete<ApiResponse<object>>(`/api/offers/${id}`);
    return unwrap(res);
  },
};

export const slotsApi = {
  getAll: async () => {
    const res = await api.get<ApiResponse<Slot[]>>('/api/slots');
    return unwrap(res);
  },
  getByOffer: async (offerId: string) => {
    const res = await api.get<ApiResponse<Slot[]>>(`/api/offers/${offerId}/slots`);
    return unwrap(res);
  },
  create: async (data: Record<string, unknown>) => {
    const res = await api.post<ApiResponse<Slot>>('/api/slots', data);
    return unwrap(res);
  },
  update: async (id: string, data: Record<string, unknown>) => {
    const res = await api.put<ApiResponse<Slot>>(`/api/slots/${id}`, data);
    return unwrap(res);
  },
  delete: async (id: string) => {
    const res = await api.delete<ApiResponse<object>>(`/api/slots/${id}`);
    return unwrap(res);
  },
};

export const bookingsApi = {
  create: async (data: Record<string, unknown>) => {
    const res = await api.post<ApiResponse<Booking>>('/api/bookings', data);
    return unwrap(res);
  },
  getPaged: async (page = 1, pageSize = 20, status?: BookingStatus, offerId?: string) => {
    const res = await api.get<ApiResponse<PagedResult<Booking>>>('/api/bookings', {
      params: { page, pageSize, status, offerId },
    });
    return unwrap(res);
  },
  getRecent: async (limit = 10) => {
    const res = await api.get<ApiResponse<BookingDetail[]>>('/api/bookings/recent', { params: { limit } });
    return unwrap(res);
  },
  getByReference: async (reference: string) => {
    const res = await api.get<ApiResponse<BookingDetail>>(`/api/bookings/reference/${reference}`);
    return unwrap(res);
  },
  updateStatus: async (id: string, status: BookingStatus) => {
    const res = await api.put<ApiResponse<Booking>>(`/api/bookings/${id}/status`, { status });
    return unwrap(res);
  },
};

export const waitlistApi = {
  create: async (data: Record<string, unknown>) => {
    const res = await api.post<ApiResponse<unknown>>('/api/waitlist', data);
    return unwrap(res);
  },
};

export const dashboardApi = {
  getSummary: async () => {
    const res = await api.get<ApiResponse<DashboardSummary>>('/api/dashboard/summary');
    return unwrap(res);
  },
};
