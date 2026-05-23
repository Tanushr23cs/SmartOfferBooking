import { useCallback, useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import toast from 'react-hot-toast';
import { Plus, Trash2 } from 'lucide-react';
import { slotsApi, offersApi } from '@/lib/api';
import type { Offer, Slot } from '@/types';
import { Card } from '@/components/ui/Card';
import { Button } from '@/components/ui/Button';
import { Input } from '@/components/ui/Input';
import { Badge } from '@/components/ui/Badge';
import { Modal } from '@/components/ui/Modal';
import { formatDate, formatTime } from '@/lib/utils';

const schema = z.object({
  offerId: z.string().min(1),
  slotDate: z.string(),
  startTime: z.string(),
  endTime: z.string(),
  capacity: z.coerce.number().min(1),
});

type FormData = z.infer<typeof schema>;

export function SlotsPage() {
  const [slots, setSlots] = useState<Slot[]>([]);
  const [offers, setOffers] = useState<Offer[]>([]);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [saving, setSaving] = useState(false);

  const { register, handleSubmit, reset, formState: { errors } } = useForm<FormData>({
    resolver: zodResolver(schema),
  });

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const [s, o] = await Promise.all([
        slotsApi.getAll(),
        offersApi.getPaged({ page: 1, pageSize: 100, publicOnly: false }) as Promise<{ items: Offer[] }>,
      ]);
      setSlots(s);
      setOffers(o.items);
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Failed to load');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { load(); }, [load]);

  const onSubmit = async (data: FormData) => {
    setSaving(true);
    try {
      await slotsApi.create(data);
      toast.success('Slot created');
      setModalOpen(false);
      load();
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Failed');
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm('Cancel this slot?')) return;
    try {
      await slotsApi.delete(id);
      toast.success('Slot cancelled');
      load();
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Failed');
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex justify-between">
        <h1 className="text-2xl font-bold">Slots</h1>
        <Button onClick={() => { reset({ capacity: 10 }); setModalOpen(true); }}><Plus className="h-4 w-4" /> Add Slot</Button>
      </div>

      <Card className="overflow-x-auto p-0">
        <table className="w-full text-sm">
          <thead className="bg-slate-50 dark:bg-slate-900">
            <tr>
              <th className="px-4 py-3 text-left">Date</th>
              <th className="px-4 py-3 text-left">Time</th>
              <th className="px-4 py-3 text-left">Capacity</th>
              <th className="px-4 py-3 text-left">Available</th>
              <th className="px-4 py-3 text-left">Status</th>
              <th className="px-4 py-3 text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr><td colSpan={6} className="p-8 text-center">Loading...</td></tr>
            ) : slots.map((s) => (
              <tr key={s.id} className="border-t border-slate-100 dark:border-slate-800">
                <td className="px-4 py-3">{formatDate(s.slotDate)}</td>
                <td className="px-4 py-3">{formatTime(s.startTime)} - {formatTime(s.endTime)}</td>
                <td className="px-4 py-3">{s.capacity}</td>
                <td className="px-4 py-3">{s.availableCount}</td>
                <td className="px-4 py-3"><Badge status={s.status} /></td>
                <td className="px-4 py-3 text-right">
                  <button onClick={() => handleDelete(s.id)} className="p-2 hover:text-red-600"><Trash2 className="h-4 w-4" /></button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </Card>

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title="Create Slot">
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div>
            <label className="text-sm font-medium">Offer</label>
            <select className="mt-1 w-full rounded-xl border border-slate-200 px-3 py-2 dark:border-slate-700 dark:bg-slate-900" {...register('offerId')}>
              <option value="">Select offer</option>
              {offers.map((o) => <option key={o.id} value={o.id}>{o.title}</option>)}
            </select>
          </div>
          <Input label="Date" type="date" {...register('slotDate')} error={errors.slotDate?.message} />
          <div className="grid grid-cols-2 gap-3">
            <Input label="Start" type="time" {...register('startTime')} />
            <Input label="End" type="time" {...register('endTime')} />
          </div>
          <Input label="Capacity" type="number" {...register('capacity')} />
          <Button type="submit" loading={saving} className="w-full">Create</Button>
        </form>
      </Modal>
    </div>
  );
}
