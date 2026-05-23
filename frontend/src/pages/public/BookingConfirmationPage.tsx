import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { QRCodeSVG } from 'qrcode.react';
import { CheckCircle } from 'lucide-react';
import { bookingsApi } from '@/lib/api';
import type { BookingDetail } from '@/types';
import { PublicLayout } from '@/components/layout/PublicLayout';
import { Card } from '@/components/ui/Card';
import { Badge } from '@/components/ui/Badge';
import { Button } from '@/components/ui/Button';
import { formatDate, formatTime } from '@/lib/utils';

export function BookingConfirmationPage() {
  const { reference } = useParams<{ reference: string }>();
  const [booking, setBooking] = useState<BookingDetail | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!reference) return;
    bookingsApi.getByReference(reference)
      .then(setBooking)
      .catch(() => setBooking(null))
      .finally(() => setLoading(false));
  }, [reference]);

  if (loading) {
    return (
      <PublicLayout>
        <div className="mx-auto max-w-lg animate-pulse p-8">
          <div className="h-64 rounded-2xl bg-slate-200 dark:bg-slate-800" />
        </div>
      </PublicLayout>
    );
  }

  if (!booking) {
    return (
      <PublicLayout>
        <p className="py-20 text-center">Booking not found</p>
      </PublicLayout>
    );
  }

  const qrData = JSON.stringify({
    ref: booking.bookingReference,
    offer: booking.offerTitle,
    status: booking.status,
  });

  return (
    <PublicLayout>
      <div className="mx-auto max-w-lg px-4 py-12">
        <Card className="text-center">
          <CheckCircle className="mx-auto h-16 w-16 text-emerald-500" />
          <h1 className="mt-4 text-2xl font-bold">Booking Confirmed!</h1>
          <p className="mt-2 text-slate-600 dark:text-slate-400">Booking Reference</p>
          <p className="mt-1 text-2xl font-mono font-bold text-brand-600">{booking.bookingReference}</p>
          <div className="mt-4 flex justify-center">
            <Badge status={booking.status} />
          </div>

          <div className="mx-auto mt-6 flex justify-center rounded-2xl bg-white p-4 dark:bg-slate-800">
            <QRCodeSVG value={qrData} size={180} level="H" />
          </div>

          <div className="mt-6 space-y-2 rounded-xl bg-slate-50 p-4 text-left text-sm dark:bg-slate-800/50">
            <p><strong>Offer:</strong> {booking.offerTitle}</p>
            <p><strong>Business:</strong> {booking.businessName}</p>
            <p><strong>Slot:</strong> {formatDate(booking.slotDate)} · {formatTime(booking.slotStartTime)} – {formatTime(booking.slotEndTime)}</p>
            <p><strong>Customer:</strong> {booking.customerName}</p>
            <p><strong>Phone:</strong> {booking.customerPhone}</p>
            <p><strong>People:</strong> {booking.peopleCount}</p>
            {(booking.address || booking.city) && (
              <p><strong>Location:</strong> {[booking.address, booking.city].filter(Boolean).join(', ')}</p>
            )}
          </div>

          <Link to="/" className="mt-8 block">
            <Button className="w-full">Browse More Offers</Button>
          </Link>
        </Card>
      </div>
    </PublicLayout>
  );
}
