import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import { Toaster } from 'react-hot-toast';
import { ErrorBoundary } from '@/components/ErrorBoundary';
import { ProtectedRoute } from '@/components/ProtectedRoute';
import { AdminLayout } from '@/components/layout/AdminLayout';
import { HomePage } from '@/pages/public/HomePage';
import { OfferDetailPage } from '@/pages/public/OfferDetailPage';
import { BookingConfirmationPage } from '@/pages/public/BookingConfirmationPage';
import { ProfilePage } from '@/pages/public/ProfilePage';
import { LoginPage } from '@/pages/admin/LoginPage';
import { DashboardPage } from '@/pages/admin/DashboardPage';
import { BusinessPage } from '@/pages/admin/BusinessPage';
import { OffersPage } from '@/pages/admin/OffersPage';
import { CreateOfferPage } from '@/pages/admin/CreateOfferPage';
import { SlotsPage } from '@/pages/admin/SlotsPage';
import { BookingsPage } from '@/pages/admin/BookingsPage';
import { useEffect } from 'react';
import { useThemeStore } from '@/stores/themeStore';

export default function App() {
  const dark = useThemeStore((s) => s.dark);

  useEffect(() => {
    document.documentElement.classList.toggle('dark', dark);
  }, [dark]);

  return (
    <ErrorBoundary>
      <BrowserRouter>
        <Toaster position="top-right" toastOptions={{ duration: 4000 }} />
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/offers/:id" element={<OfferDetailPage />} />
          <Route path="/booking/confirmation/:reference" element={<BookingConfirmationPage />} />
          <Route path="/profile" element={<ProfilePage />} />
          <Route path="/admin/login" element={<LoginPage />} />
          <Route
            path="/admin"
            element={
              <ProtectedRoute>
                <AdminLayout />
              </ProtectedRoute>
            }
          >
            <Route index element={<DashboardPage />} />
            <Route path="business" element={<BusinessPage />} />
            <Route path="offers" element={<OffersPage />} />
            <Route path="offers/create" element={<CreateOfferPage />} />
            <Route path="offers/:id/edit" element={<CreateOfferPage />} />
            <Route path="slots" element={<SlotsPage />} />
            <Route path="bookings" element={<BookingsPage />} />
          </Route>
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </BrowserRouter>
    </ErrorBoundary>
  );
}
