import { useCallback, useEffect, useState } from 'react';
import toast from 'react-hot-toast';
import { bookingsApi } from '@/lib/api';
import type { Booking, BookingStatus } from '@/types';
import { Card } from '@/components/ui/Card';
import { Badge } from '@/components/ui/Badge';
import { Pagination } from '@/components/ui/Pagination';
import { BOOKING_STATUSES, formatDate } from '@/lib/utils';

export function BookingsPage() {
  const [bookings, setBookings] = useState<Booking[]>([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [status, setStatus] = useState<BookingStatus | ''>('');
  const [loading, setLoading] = useState(true);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const result = await bookingsApi.getPaged(page, 15, status || undefined);
      setBookings(result.items);
      setTotalPages(result.totalPages);
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Failed to load');
    } finally {
      setLoading(false);
    }
  }, [page, status]);

  useEffect(() => { load(); }, [load]);

  const updateStatus = async (id: string, newStatus: BookingStatus) => {
    try {
      await bookingsApi.updateStatus(id, newStatus);
      toast.success('Status updated');
      load();
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Update failed');
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex flex-wrap items-center justify-between gap-4">
        <h1 className="text-2xl font-bold">Bookings</h1>
        <select className="rounded-xl border border-slate-200 px-3 py-2 text-sm dark:border-slate-700 dark:bg-slate-900" value={status} onChange={(e) => { setStatus(e.target.value as BookingStatus | ''); setPage(1); }}>
          <option value="">All Statuses</option>
          {BOOKING_STATUSES.map((s) => <option key={s} value={s}>{s}</option>)}
        </select>
      </div>

      <Card className="overflow-x-auto p-0">
        <table className="w-full text-sm">
          <thead className="bg-slate-50 dark:bg-slate-900">
            <tr>
              <th className="px-4 py-3 text-left">Reference</th>
              <th className="px-4 py-3 text-left">Customer</th>
              <th className="px-4 py-3 text-left">Phone</th>
              <th className="px-4 py-3 text-left">People</th>
              <th className="px-4 py-3 text-left">Status</th>
              <th className="px-4 py-3 text-left">Date</th>
              <th className="px-4 py-3 text-left">Actions</th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr><td colSpan={7} className="p-8 text-center">Loading...</td></tr>
            ) : bookings.map((b) => (
              <tr key={b.id} className="border-t border-slate-100 dark:border-slate-800">
                <td className="px-4 py-3 font-mono text-xs">{b.bookingReference}</td>
                <td className="px-4 py-3">{b.customerName}</td>
                <td className="px-4 py-3">{b.customerPhone}</td>
                <td className="px-4 py-3">{b.peopleCount}</td>
                <td className="px-4 py-3"><Badge status={b.status} /></td>
                <td className="px-4 py-3">{formatDate(b.createdAt)}</td>
                <td className="px-4 py-3">
                  <select
                    className="rounded-lg border border-slate-200 px-2 py-1 text-xs dark:border-slate-700 dark:bg-slate-900"
                    value={b.status}
                    onChange={(e) => updateStatus(b.id, e.target.value as BookingStatus)}
                  >
                    {BOOKING_STATUSES.map((s) => <option key={s} value={s}>{s}</option>)}
                  </select>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </Card>
      <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
    </div>
  );
}
