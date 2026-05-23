import { useCallback, useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import toast from 'react-hot-toast';
import { Pencil, Plus, Search, Trash2 } from 'lucide-react';
import { offersApi } from '@/lib/api';
import type { Offer, OfferStatus, PagedResult } from '@/types';
import { Card } from '@/components/ui/Card';
import { Button } from '@/components/ui/Button';
import { Badge } from '@/components/ui/Badge';
import { Pagination } from '@/components/ui/Pagination';
import { OFFER_STATUSES, formatCurrency } from '@/lib/utils';

export function OffersPage() {
  const [offers, setOffers] = useState<Offer[]>([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState<OfferStatus | ''>('');
  const [loading, setLoading] = useState(true);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const result = await offersApi.getPaged({
        page,
        pageSize: 10,
        search: search || undefined,
        status: statusFilter || undefined,
        publicOnly: false,
      }) as PagedResult<Offer>;
      setOffers(result.items as Offer[]);
      setTotalPages(result.totalPages);
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Failed to load offers');
    } finally {
      setLoading(false);
    }
  }, [page, search, statusFilter]);

  useEffect(() => { load(); }, [load]);

  const handleDelete = async (id: string) => {
    if (!confirm('Cancel this offer? It will be hidden from the public page.')) return;
    try {
      await offersApi.delete(id);
      toast.success('Offer cancelled');
      load();
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Failed');
    }
  };

  const handlePause = async (offer: Offer) => {
    try {
      const payload = {
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
        status: offer.status === 'Paused' ? 'Active' : 'Paused',
      };
      await offersApi.update(offer.id, payload);
      toast.success(offer.status === 'Paused' ? 'Offer activated' : 'Offer paused');
      load();
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Failed');
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex flex-wrap items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold">Manage Offers</h1>
          <p className="text-slate-500">Create, edit, pause or cancel offers</p>
        </div>
        <Link to="/admin/offers/create">
          <Button><Plus className="h-4 w-4" /> Create Offer</Button>
        </Link>
      </div>

      <Card className="flex flex-wrap gap-4">
        <div className="relative min-w-[200px] flex-1">
          <Search className="absolute left-3 top-3 h-4 w-4 text-slate-400" />
          <input
            className="w-full rounded-xl border border-slate-200 py-2 pl-9 pr-4 text-sm dark:border-slate-700 dark:bg-slate-900"
            placeholder="Search offers..."
            value={search}
            onChange={(e) => { setSearch(e.target.value); setPage(1); }}
          />
        </div>
        <select
          className="rounded-xl border border-slate-200 px-3 py-2 text-sm dark:border-slate-700 dark:bg-slate-900"
          value={statusFilter}
          onChange={(e) => { setStatusFilter(e.target.value as OfferStatus | ''); setPage(1); }}
        >
          <option value="">All Statuses</option>
          {OFFER_STATUSES.map((s) => <option key={s} value={s}>{s}</option>)}
        </select>
      </Card>

      <div className="overflow-x-auto rounded-2xl border border-slate-200 dark:border-slate-800">
        <table className="w-full text-sm">
          <thead className="bg-slate-50 dark:bg-slate-900">
            <tr>
              <th className="px-4 py-3 text-left">Title</th>
              <th className="px-4 py-3 text-left">Category</th>
              <th className="px-4 py-3 text-left">Price</th>
              <th className="px-4 py-3 text-left">Dates</th>
              <th className="px-4 py-3 text-left">Status</th>
              <th className="px-4 py-3 text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr><td colSpan={6} className="p-8 text-center">Loading...</td></tr>
            ) : offers.length === 0 ? (
              <tr><td colSpan={6} className="p-8 text-center text-slate-500">No offers yet. Create your first offer.</td></tr>
            ) : offers.map((o) => (
              <tr key={o.id} className="border-t border-slate-100 dark:border-slate-800">
                <td className="px-4 py-3 font-medium">{o.title}</td>
                <td className="px-4 py-3">{o.category}</td>
                <td className="px-4 py-3">
                  <span className="font-semibold text-brand-600">{formatCurrency(o.offerPrice)}</span>
                  <span className="ml-1 text-xs text-slate-400 line-through">{formatCurrency(o.originalPrice)}</span>
                </td>
                <td className="px-4 py-3 text-xs">{o.startDate} → {o.endDate}</td>
                <td className="px-4 py-3"><Badge status={o.status} /></td>
                <td className="px-4 py-3 text-right">
                  <div className="flex justify-end gap-1">
                    {(o.status === 'Active' || o.status === 'Paused') && (
                      <Button variant="ghost" size="sm" onClick={() => handlePause(o)}>
                        {o.status === 'Paused' ? 'Activate' : 'Pause'}
                      </Button>
                    )}
                    <Link to={`/admin/offers/${o.id}/edit`} className="p-2 hover:text-brand-600"><Pencil className="h-4 w-4" /></Link>
                    <button type="button" onClick={() => handleDelete(o.id)} className="p-2 hover:text-red-600"><Trash2 className="h-4 w-4" /></button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
    </div>
  );
}
