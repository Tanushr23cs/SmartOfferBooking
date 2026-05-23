import { useCallback, useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { Bar, BarChart, ResponsiveContainer, Tooltip, XAxis, YAxis } from 'recharts';
import toast from 'react-hot-toast';
import { Tag, Users, Calendar, TrendingUp, Armchair } from 'lucide-react';
import { dashboardApi, bookingsApi } from '@/lib/api';
import type { BookingDetail, DashboardSummary } from '@/types';
import { Card } from '@/components/ui/Card';
import { Badge } from '@/components/ui/Badge';
import { Button } from '@/components/ui/Button';
import { useAdminSignalR } from '@/hooks/useSignalR';
import { formatDate, formatTime } from '@/lib/utils';

export function DashboardPage() {
  const [summary, setSummary] = useState<DashboardSummary | null>(null);
  const [recent, setRecent] = useState<BookingDetail[]>([]);
  const [loading, setLoading] = useState(true);

  const load = useCallback(async () => {
    try {
      const [s, r] = await Promise.all([dashboardApi.getSummary(), bookingsApi.getRecent(10)]);
      setSummary(s);
      setRecent(r);
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Failed to load dashboard');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { load(); }, [load]);

  useAdminSignalR(() => load(), () => load(), () => load());

  if (loading) {
    return (
      <div className="space-y-4">
        <div className="grid gap-4 md:grid-cols-4">{Array.from({ length: 8 }).map((_, i) => <div key={i} className="h-24 animate-pulse rounded-2xl bg-slate-200 dark:bg-slate-800" />)}</div>
      </div>
    );
  }

  if (!summary) return <p>Failed to load dashboard</p>;

  const chartData = [
    { name: 'Capacity', value: summary.totalCapacity },
    { name: 'Booked', value: summary.bookedSeats },
    { name: 'Available', value: summary.availableSeats },
  ];

  const metrics = [
    { label: 'Total Offers', value: summary.totalOffers, icon: Tag },
    { label: 'Active Offers', value: summary.activeOffers, icon: Calendar },
    { label: 'Total Bookings', value: summary.totalBookings, icon: Users },
    { label: "Today's Bookings", value: summary.todaysBookings, icon: TrendingUp },
    { label: 'Total Capacity', value: summary.totalCapacity, icon: Armchair },
    { label: 'Booked Seats', value: summary.bookedSeats, icon: Users },
    { label: 'Available Seats', value: summary.availableSeats, icon: Armchair },
    { label: 'Conversion Rate', value: `${summary.conversionRate}%`, icon: TrendingUp },
  ];

  return (
    <div className="space-y-8">
      <div className="flex flex-wrap items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold">Dashboard</h1>
          <p className="text-slate-500">Live metrics — updates automatically via SignalR</p>
        </div>
        <Link to="/admin/offers/create"><Button>Create Offer</Button></Link>
      </div>

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        {metrics.map(({ label, value, icon: Icon }) => (
          <Card key={label} className="flex items-center gap-4">
            <div className="rounded-xl bg-brand-50 p-3 text-brand-600 dark:bg-brand-900/30">
              <Icon className="h-5 w-5" />
            </div>
            <div>
              <p className="text-sm text-slate-500">{label}</p>
              <p className="text-xl font-bold">{value}</p>
            </div>
          </Card>
        ))}
      </div>

      <div className="grid gap-6 lg:grid-cols-2">
        <Card>
          <h2 className="mb-4 font-bold">Seat Utilization</h2>
          <div className="h-64">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={chartData}>
                <XAxis dataKey="name" />
                <YAxis />
                <Tooltip />
                <Bar dataKey="value" fill="#f97316" radius={[8, 8, 0, 0]} />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </Card>

        <Card className="overflow-hidden p-0">
          <div className="border-b border-slate-100 p-4 dark:border-slate-800">
            <h2 className="font-bold">Recent Bookings</h2>
          </div>
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead className="bg-slate-50 dark:bg-slate-900">
                <tr>
                  <th className="px-4 py-2 text-left">Customer</th>
                  <th className="px-4 py-2 text-left">Offer</th>
                  <th className="px-4 py-2 text-left">Slot</th>
                  <th className="px-4 py-2 text-left">People</th>
                  <th className="px-4 py-2 text-left">Status</th>
                  <th className="px-4 py-2 text-left">Action</th>
                </tr>
              </thead>
              <tbody>
                {recent.length === 0 ? (
                  <tr><td colSpan={6} className="p-6 text-center text-slate-500">No bookings yet</td></tr>
                ) : recent.map((b) => (
                  <tr key={b.id} className="border-t border-slate-100 dark:border-slate-800">
                    <td className="px-4 py-3 font-medium">{b.customerName}</td>
                    <td className="px-4 py-3">{b.offerTitle}</td>
                    <td className="px-4 py-3 text-xs">
                      {formatDate(b.slotDate)}<br />
                      {formatTime(b.slotStartTime)} – {formatTime(b.slotEndTime)}
                    </td>
                    <td className="px-4 py-3">{b.peopleCount}</td>
                    <td className="px-4 py-3"><Badge status={b.status} /></td>
                    <td className="px-4 py-3">
                      <Link to="/admin/bookings" className="text-brand-600 hover:underline text-xs">Manage</Link>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </Card>
      </div>
    </div>
  );
}
