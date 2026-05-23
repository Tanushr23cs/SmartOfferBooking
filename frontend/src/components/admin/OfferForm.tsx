import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Input } from '@/components/ui/Input';
import { Button } from '@/components/ui/Button';
import { OFFER_CATEGORIES, OFFER_STATUSES } from '@/lib/utils';
import type { Offer } from '@/types';

export const offerFormSchema = z.object({
  title: z.string().min(2, 'Title is required'),
  description: z.string().min(10, 'Description is required'),
  category: z.string().min(1),
  originalPrice: z.coerce.number().positive(),
  offerPrice: z.coerce.number().positive(),
  startDate: z.string().min(1),
  endDate: z.string().min(1),
  startTime: z.string().min(1),
  endTime: z.string().min(1),
  totalCapacity: z.coerce.number().min(1),
  maxBookingPerCustomer: z.coerce.number().min(1),
  termsAndConditions: z.string().optional(),
  status: z.enum(['Draft', 'Active', 'Paused', 'Expired', 'Cancelled']),
}).refine((d) => d.offerPrice < d.originalPrice, {
  message: 'Offer price must be less than original price',
  path: ['offerPrice'],
});

export type OfferFormData = z.infer<typeof offerFormSchema>;

interface OfferFormProps {
  defaultValues?: Partial<OfferFormData>;
  onSubmit: (data: OfferFormData) => Promise<void>;
  loading?: boolean;
  submitLabel?: string;
}

export function OfferForm({ defaultValues, onSubmit, loading, submitLabel = 'Save Offer' }: OfferFormProps) {
  const { register, handleSubmit, watch, formState: { errors } } = useForm<OfferFormData>({
    resolver: zodResolver(offerFormSchema),
    defaultValues: {
      status: 'Draft',
      maxBookingPerCustomer: 1,
      category: 'Gym Trial Slot',
      ...defaultValues,
    },
  });

  const originalPrice = watch('originalPrice');
  const offerPrice = watch('offerPrice');
  const discount =
    originalPrice > 0 && offerPrice > 0 && offerPrice < originalPrice
      ? Math.round(((originalPrice - offerPrice) / originalPrice) * 100)
      : 0;

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <Input label="Offer Title" {...register('title')} error={errors.title?.message} />
      <div>
        <label className="text-sm font-medium text-slate-700 dark:text-slate-300">Description</label>
        <textarea
          className="mt-1 w-full rounded-xl border border-slate-200 p-3 text-sm dark:border-slate-700 dark:bg-slate-900"
          rows={4}
          {...register('description')}
        />
        {errors.description && <p className="text-xs text-red-500">{errors.description.message}</p>}
      </div>
      <div>
        <label className="text-sm font-medium">Category</label>
        <select className="mt-1 w-full rounded-xl border border-slate-200 px-3 py-2.5 dark:border-slate-700 dark:bg-slate-900" {...register('category')}>
          {OFFER_CATEGORIES.map((c) => (
            <option key={c} value={c}>{c}</option>
          ))}
        </select>
      </div>
      <div className="grid gap-4 md:grid-cols-2">
        <Input label="Original Price (₹)" type="number" {...register('originalPrice')} error={errors.originalPrice?.message} />
        <Input label="Offer Price (₹)" type="number" {...register('offerPrice')} error={errors.offerPrice?.message} />
      </div>
      {discount > 0 && (
        <p className="text-sm font-medium text-brand-600">Discount: {discount}% (auto-calculated on save)</p>
      )}
      <div className="grid gap-4 md:grid-cols-2">
        <Input label="Start Date" type="date" {...register('startDate')} error={errors.startDate?.message} />
        <Input label="End Date" type="date" {...register('endDate')} error={errors.endDate?.message} />
      </div>
      <div className="grid gap-4 md:grid-cols-2">
        <Input label="Start Time" type="time" {...register('startTime')} />
        <Input label="End Time" type="time" {...register('endTime')} />
      </div>
      <div className="grid gap-4 md:grid-cols-2">
        <Input label="Total Capacity" type="number" {...register('totalCapacity')} />
        <Input label="Max Booking Per Customer" type="number" {...register('maxBookingPerCustomer')} />
      </div>
      <div>
        <label className="text-sm font-medium">Terms & Conditions</label>
        <textarea
          className="mt-1 w-full rounded-xl border border-slate-200 p-3 text-sm dark:border-slate-700 dark:bg-slate-900"
          rows={3}
          {...register('termsAndConditions')}
        />
      </div>
      <div>
        <label className="text-sm font-medium">Status</label>
        <select className="mt-1 w-full rounded-xl border border-slate-200 px-3 py-2.5 dark:border-slate-700 dark:bg-slate-900" {...register('status')}>
          {OFFER_STATUSES.map((s) => (
            <option key={s} value={s}>{s}</option>
          ))}
        </select>
      </div>
      <Button type="submit" loading={loading} className="w-full md:w-auto">{submitLabel}</Button>
    </form>
  );
}

export function offerToFormData(offer: Offer): OfferFormData {
  return {
    title: offer.title,
    description: offer.description,
    category: offer.category,
    originalPrice: offer.originalPrice,
    offerPrice: offer.offerPrice,
    startDate: offer.startDate,
    endDate: offer.endDate,
    startTime: offer.startTime.slice(0, 5),
    endTime: offer.endTime.slice(0, 5),
    totalCapacity: offer.totalCapacity,
    maxBookingPerCustomer: offer.maxBookingPerCustomer,
    termsAndConditions: offer.termsAndConditions,
    status: offer.status,
  };
}
