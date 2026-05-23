import { useCallback, useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import toast from 'react-hot-toast';
import { motion } from 'framer-motion';
import { offersApi, slotsApi, bookingsApi, waitlistApi } from '@/lib/api';
import type { OfferPublic, Slot, SlotUpdatedEvent } from '@/types';
import { PublicLayout } from '@/components/layout/PublicLayout';
import { Card } from '@/components/ui/Card';
import { Button } from '@/components/ui/Button';
import { Input } from '@/components/ui/Input';
import { Badge } from '@/components/ui/Badge';
import { MapPin } from 'lucide-react';
import { formatCurrency, formatDate, formatTime, getCountdown } from '@/lib/utils';
import { useOfferSignalR } from '@/hooks/useSignalR';

const bookingSchema = z.object({
  customerName: z.string().min(2, 'Name is required'),
  customerPhone: z.string().min(10, 'Valid phone required'),
  customerEmail: z.string().email('Invalid email').optional().or(z.literal('')),
  slotId: z.string().min(1, 'Select a slot'),
  peopleCount: z.coerce.number().min(1).max(50),
  specialNote: z.string().optional(),
});

type BookingForm = z.infer<typeof bookingSchema>;

export function OfferDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [offer, setOffer] = useState<OfferPublic | null>(null);
  const [slots, setSlots] = useState<Slot[]>([]);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [waitlistMode, setWaitlistMode] = useState(false);
  const [countdown, setCountdown] = useState(getCountdown(''));

  const load = useCallback(async () => {
    if (!id) return;
    setLoading(true);
    try {
      const [o, s] = await Promise.all([
        offersApi.getById(id, true) as Promise<OfferPublic>,
        slotsApi.getByOffer(id),
      ]);
      setOffer(o);
      setSlots(s);
      setCountdown(getCountdown(o.endDate));
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Offer not found');
    } finally {
      setLoading(false);
    }
  }, [id]);

  useEffect(() => { load(); }, [load]);

  const onSlotUpdated = useCallback((event: SlotUpdatedEvent) => {
    setSlots((prev) => {
      const next = prev.map((s) =>
        s.id === event.slotId
          ? { ...s, bookedCount: event.bookedCount, availableCount: event.availableCount, status: event.status }
          : s
      );
      const totalSeats = next.reduce((sum, s) => sum + s.availableCount, 0);
      setOffer((o) => (o ? { ...o, totalAvailableSeats: totalSeats } : o));
      return next;
    });
  }, []);

  useOfferSignalR(id, onSlotUpdated);

  const { register, handleSubmit, watch, formState: { errors } } = useForm<BookingForm>({
    resolver: zodResolver(bookingSchema),
    defaultValues: { peopleCount: 1 },
  });

  const selectedSlotId = watch('slotId');
  const selectedSlot = slots.find((s) => s.id === selectedSlotId);

  useEffect(() => {
    if (selectedSlot) setWaitlistMode(selectedSlot.status === 'Full' || selectedSlot.availableCount === 0);
  }, [selectedSlot]);

  const onSubmit = async (data: BookingForm) => {
    if (!id || !offer) return;
    setSubmitting(true);
    try {
      if (waitlistMode) {
        await waitlistApi.create({
          offerId: id,
          slotId: data.slotId,
          customerName: data.customerName,
          customerPhone: data.customerPhone,
          customerEmail: data.customerEmail || undefined,
          peopleCount: data.peopleCount,
        });
        toast.success('Added to waitlist! We will notify you when a seat opens.');
        return;
      }
      const booking = await bookingsApi.create({
        offerId: id,
        slotId: data.slotId,
        customerName: data.customerName,
        customerPhone: data.customerPhone,
        customerEmail: data.customerEmail || undefined,
        peopleCount: data.peopleCount,
        specialNote: data.specialNote,
      });
      navigate(`/booking/confirmation/${booking.bookingReference}`);
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Booking failed');
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return (
      <PublicLayout>
        <div className="mx-auto max-w-5xl animate-pulse space-y-4 p-8">
          <div className="h-48 rounded-2xl bg-slate-200 dark:bg-slate-800" />
          <div className="h-64 rounded-2xl bg-slate-200 dark:bg-slate-800" />
        </div>
      </PublicLayout>
    );
  }

  if (!offer) {
    return (
      <PublicLayout>
        <p className="py-20 text-center">Offer not found</p>
      </PublicLayout>
    );
  }

  return (
    <PublicLayout>
      <div className="mx-auto max-w-5xl px-4 py-8">
        <motion.div initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }}>
          <Card className="mb-6 bg-gradient-to-br from-brand-500 to-brand-700 text-white border-0">
            <p className="text-sm opacity-90">{offer.businessName} · {offer.businessType}</p>
            {(offer.address || offer.city) && (
              <p className="mt-2 flex items-center gap-1 text-sm opacity-90">
                <MapPin className="h-4 w-4" />
                {[offer.address, offer.city].filter(Boolean).join(', ')}
              </p>
            )}
            <h1 className="mt-2 text-3xl font-bold">{offer.title}</h1>
            <p className="mt-2 opacity-90">{offer.description}</p>
            <div className="mt-4 flex flex-wrap gap-4">
              <span className="text-3xl font-bold">{formatCurrency(offer.offerPrice)}</span>
              <span className="text-lg line-through opacity-70">{formatCurrency(offer.originalPrice)}</span>
              <Badge status="Active" />
              {!countdown.expired && (
                <span className="rounded-full bg-white/20 px-3 py-1 text-sm">
                  Ends {countdown.days}d {countdown.hours}h {countdown.minutes}m
                </span>
              )}
            </div>
          </Card>

          <div className="grid gap-6 lg:grid-cols-2">
            <Card>
              <h2 className="mb-4 text-lg font-bold">Available Slots</h2>
              <p className="mb-4 text-sm text-slate-500">{offer.totalAvailableSeats} total seats available</p>
              <div className="space-y-2 max-h-80 overflow-y-auto">
                {slots.map((slot) => (
                  <label
                    key={slot.id}
                    className={`flex cursor-pointer items-center justify-between rounded-xl border p-3 transition ${selectedSlotId === slot.id ? 'border-brand-500 bg-brand-50 dark:bg-brand-900/20' : 'border-slate-200 dark:border-slate-700'}`}
                  >
                    <div className="flex items-center gap-3">
                      <input type="radio" value={slot.id} {...register('slotId')} />
                      <div>
                        <p className="font-medium">{formatDate(slot.slotDate)}</p>
                        <p className="text-sm text-slate-500">{formatTime(slot.startTime)} - {formatTime(slot.endTime)}</p>
                      </div>
                    </div>
                    <div className="text-right">
                      <Badge status={slot.status} />
                      <p className="mt-1 text-sm">{slot.availableCount} / {slot.capacity} left</p>
                    </div>
                  </label>
                ))}
              </div>
              {errors.slotId && <p className="mt-2 text-xs text-red-500">{errors.slotId.message}</p>}
            </Card>

            <Card>
              <h2 className="mb-4 text-lg font-bold">{waitlistMode ? 'Join Waitlist' : 'Book Now'}</h2>
              <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
                <Input label="Full Name" {...register('customerName')} error={errors.customerName?.message} />
                <Input label="Phone" {...register('customerPhone')} error={errors.customerPhone?.message} />
                <Input label="Email (optional)" type="email" {...register('customerEmail')} error={errors.customerEmail?.message} />
                <Input label="Number of People" type="number" min={1} {...register('peopleCount')} error={errors.peopleCount?.message} />
                <Input label="Special Note" {...register('specialNote')} />
                <Button type="submit" className="w-full" loading={submitting}>
                  {waitlistMode ? 'Join Waitlist' : 'Confirm Booking'}
                </Button>
              </form>
            </Card>
          </div>

          {offer.termsAndConditions && (
            <Card className="mt-6">
              <h2 className="mb-2 font-bold">Terms & Conditions</h2>
              <p className="text-sm text-slate-600 dark:text-slate-400 whitespace-pre-wrap">{offer.termsAndConditions}</p>
            </Card>
          )}
        </motion.div>
      </div>
    </PublicLayout>
  );
}
