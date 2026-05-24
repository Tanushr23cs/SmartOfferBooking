import { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import toast from 'react-hot-toast';
import { businessApi } from '@/lib/api';
import type { BusinessProfile } from '@/types';
import { Card } from '@/components/ui/Card';
import { Input } from '@/components/ui/Input';
import { Button } from '@/components/ui/Button';
import { BUSINESS_TYPES, toApiTime } from '@/lib/utils';

const schema = z.object({
  businessName: z.string().min(2),
  businessType: z.string().min(1),
  ownerName: z.string().min(2),
  phone: z.string().min(10),
  email: z.string().email(),
  address: z.string().min(5),
  city: z.string().min(2),
  logoUrl: z.string().url().optional().or(z.literal('')),
  openingTime: z.string().min(1),
  closingTime: z.string().min(1),
});

type FormData = z.infer<typeof schema>;

export function BusinessPage() {
  const [profile, setProfile] = useState<BusinessProfile | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  const { register, handleSubmit, reset, formState: { errors } } = useForm<FormData>({
    resolver: zodResolver(schema),
  });

  useEffect(() => {
    businessApi.get()
      .then((p) => { setProfile(p); reset(p); })
      .catch(() => setProfile(null))
      .finally(() => setLoading(false));
  }, [reset]);

  const onSubmit = async (data: FormData) => {
    setSaving(true);
    try {
      const payload = {
        ...data,
        logoUrl: data.logoUrl || undefined,
        openingTime: toApiTime(data.openingTime),
        closingTime: toApiTime(data.closingTime),
      };
      if (profile) {
        const updated = await businessApi.update(profile.id, payload);
        setProfile(updated);
        toast.success('Business profile updated');
      } else {
        const created = await businessApi.create(payload);
        setProfile(created);
        toast.success('Business profile created');
      }
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Save failed');
    } finally {
      setSaving(false);
    }
  };

  if (loading) return <div className="h-64 animate-pulse rounded-2xl bg-slate-200 dark:bg-slate-800" />;

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold">Business Profile</h1>
      <Card>
        <form onSubmit={handleSubmit(onSubmit)} className="grid gap-4 md:grid-cols-2">
          <Input label="Business Name" {...register('businessName')} error={errors.businessName?.message} />
          <div>
            <label className="text-sm font-medium">Business Type</label>
            <select className="mt-1 w-full rounded-xl border border-slate-200 px-4 py-2.5 dark:border-slate-700 dark:bg-slate-900" {...register('businessType')}>
              {BUSINESS_TYPES.map((t) => <option key={t} value={t}>{t}</option>)}
            </select>
          </div>
          <Input label="Owner Name" {...register('ownerName')} error={errors.ownerName?.message} />
          <Input label="Phone" {...register('phone')} error={errors.phone?.message} />
          <Input label="Email" type="email" {...register('email')} error={errors.email?.message} />
          <Input label="City" {...register('city')} error={errors.city?.message} />
          <Input label="Address" className="md:col-span-2" {...register('address')} error={errors.address?.message} />
          <Input label="Logo URL" {...register('logoUrl')} error={errors.logoUrl?.message} />
          <Input label="Opening Time" type="time" {...register('openingTime')} error={errors.openingTime?.message} />
          <Input label="Closing Time" type="time" {...register('closingTime')} error={errors.closingTime?.message} />
          <div className="md:col-span-2">
            <Button type="submit" loading={saving}>{profile ? 'Update Profile' : 'Create Profile'}</Button>
          </div>
        </form>
      </Card>
    </div>
  );
}
