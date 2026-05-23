import { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import toast from 'react-hot-toast';
import { ArrowLeft } from 'lucide-react';
import { offersApi } from '@/lib/api';
import type { Offer } from '@/types';
import { Card } from '@/components/ui/Card';
import { OfferForm, offerToFormData, type OfferFormData } from '@/components/admin/OfferForm';

export function CreateOfferPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const isEdit = Boolean(id);
  const [loading, setLoading] = useState(false);
  const [fetching, setFetching] = useState(isEdit);
  const [defaults, setDefaults] = useState<Partial<OfferFormData>>();

  useEffect(() => {
    if (!id) return;
    offersApi.getById(id, false)
      .then((o) => setDefaults(offerToFormData(o as Offer)))
      .catch(() => toast.error('Offer not found'))
      .finally(() => setFetching(false));
  }, [id]);

  const onSubmit = async (data: OfferFormData) => {
    setLoading(true);
    try {
      // Transform time format from HH:MM to HH:mm:ss for backend
      const statusMap: Record<string, number> = {
        Draft: 0,
        Active: 1,
        Paused: 2,
        Expired: 3,
        Cancelled: 4,
      };

      const transformedData = {
        ...data,
        startTime: data.startTime + ':00',
        endTime: data.endTime + ':00',
        status: statusMap[data.status] ?? 0,
      };
      
      if (isEdit && id) {
        await offersApi.update(id, transformedData);
        toast.success('Offer updated');
      } else {
        await offersApi.create(transformedData);
        toast.success('Offer created');
      }
      navigate('/admin/offers');
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Save failed');
    } finally {
      setLoading(false);
    }
  };

  if (fetching) {
    return <div className="h-64 animate-pulse rounded-2xl bg-slate-200 dark:bg-slate-800" />;
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Link to="/admin/offers" className="rounded-lg p-2 hover:bg-slate-100 dark:hover:bg-slate-800">
          <ArrowLeft className="h-5 w-5" />
        </Link>
        <div>
          <h1 className="text-2xl font-bold">{isEdit ? 'Edit Offer' : 'Create Offer'}</h1>
          <p className="text-slate-500">Set pricing, dates, capacity and status</p>
        </div>
      </div>
      <Card>
        <OfferForm
          defaultValues={defaults}
          onSubmit={onSubmit}
          loading={loading}
          submitLabel={isEdit ? 'Update Offer' : 'Create Offer'}
        />
      </Card>
    </div>
  );
}
