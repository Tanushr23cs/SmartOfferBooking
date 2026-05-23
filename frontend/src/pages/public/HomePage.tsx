import { useCallback, useEffect, useState } from 'react';
import { Search } from 'lucide-react';
import toast from 'react-hot-toast';
import { offersApi } from '@/lib/api';
import type { OfferPublic, OfferUpdatedEvent, PagedResult } from '@/types';
import { OFFER_CATEGORIES, BUSINESS_TYPES } from '@/lib/utils';
import { PublicLayout } from '@/components/layout/PublicLayout';
import { OfferCard } from '@/components/offers/OfferCard';
import { OfferCardSkeleton } from '@/components/ui/Skeleton';
import { Pagination } from '@/components/ui/Pagination';
import { Input } from '@/components/ui/Input';
import { createHubConnection } from '@/lib/signalr';

export function HomePage() {
  const [offers, setOffers] = useState<OfferPublic[]>([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [search, setSearch] = useState('');
  const [category, setCategory] = useState('');
  const [businessType, setBusinessType] = useState('');
  const [minPrice, setMinPrice] = useState('');
  const [maxPrice, setMaxPrice] = useState('');
  const [availableOnly, setAvailableOnly] = useState(false);
  const [slotDate, setSlotDate] = useState('');

  const loadOffers = useCallback(async () => {
    setLoading(true);
    try {
      const result = (await offersApi.getPaged({
        page,
        pageSize: 12,
        search: search || undefined,
        category: category || undefined,
        businessType: businessType || undefined,
        minPrice: minPrice ? Number(minPrice) : undefined,
        maxPrice: maxPrice ? Number(maxPrice) : undefined,
        availableOnly,
        publicOnly: true,
        slotDate: slotDate || undefined,
      })) as PagedResult<OfferPublic>;
      setOffers(result.items as OfferPublic[]);
      setTotalPages(result.totalPages);
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Failed to load offers');
    } finally {
      setLoading(false);
    }
  }, [page, search, category, businessType, minPrice, maxPrice, availableOnly, slotDate]);

  useEffect(() => {
    loadOffers();
  }, [loadOffers]);

  useEffect(() => {
    const connection = createHubConnection();

    const refresh = () => {
      loadOffers();
    };

    const startConnection = async () => {
      try {
        if (connection.state === 'Disconnected') {
          await connection.start();
          console.log('✅ SignalR Connected');
        }
      } catch (err) {
        console.error('❌ SignalR Connection Error:', err);
      }
    };

    connection.on('OfferUpdated', (event: OfferUpdatedEvent) => {
      setOffers((prev) =>
        prev.map((o) =>
          o.id === event.offerId
            ? {
                ...o,
                status: event.status,
                offerPrice: event.offerPrice,
                totalAvailableSeats: event.totalAvailableSeats,
              }
            : o
        )
      );
    });

    connection.on('SlotUpdated', refresh);
    connection.on('BookingCreated', refresh);

    startConnection();

    return () => {
      connection.off('OfferUpdated');
      connection.off('SlotUpdated');
      connection.off('BookingCreated');

      if (connection.state === 'Connected') {
        connection.stop();
      }
    };
  }, [loadOffers]);

  return (
    <PublicLayout>
      <section className="bg-gradient-to-br from-brand-600 via-brand-500 to-orange-400 px-4 py-16 text-white">
        <div className="mx-auto max-w-4xl text-center">
          <h1 className="text-4xl font-extrabold md:text-5xl">Discover Limited-Time Offers</h1>
          <p className="mt-4 text-lg opacity-90">Book gym trials, salon slots, restaurant deals & more — in real time.</p>
        </div>
      </section>

      <section className="mx-auto max-w-7xl px-4 py-8">
        <div className="mb-8 grid gap-4 rounded-2xl border border-slate-200 bg-white p-4 dark:border-slate-800 dark:bg-slate-900 md:grid-cols-2 lg:grid-cols-4">
          <div className="relative md:col-span-2">
            <Search className="absolute left-3 top-3 h-5 w-5 text-slate-400" />
            <input
              className="w-full rounded-xl border border-slate-200 py-2.5 pl-10 pr-4 text-sm dark:border-slate-700 dark:bg-slate-800"
              placeholder="Search offers..."
              value={search}
              onChange={(e) => { setSearch(e.target.value); setPage(1); }}
            />
          </div>
          <select className="rounded-xl border border-slate-200 px-3 py-2.5 text-sm dark:border-slate-700 dark:bg-slate-800" value={category} onChange={(e) => { setCategory(e.target.value); setPage(1); }}>
            <option value="">All Categories</option>
            {OFFER_CATEGORIES.map((c) => <option key={c} value={c}>{c}</option>)}
          </select>
          <select className="rounded-xl border border-slate-200 px-3 py-2.5 text-sm dark:border-slate-700 dark:bg-slate-800" value={businessType} onChange={(e) => { setBusinessType(e.target.value); setPage(1); }}>
            <option value="">All Business Types</option>
            {BUSINESS_TYPES.map((b) => <option key={b} value={b}>{b}</option>)}
          </select>
          <Input type="date" label="Slot Date" value={slotDate} onChange={(e) => { setSlotDate(e.target.value); setPage(1); }} />
          <Input type="number" placeholder="Min price (₹)" value={minPrice} onChange={(e) => { setMinPrice(e.target.value); setPage(1); }} />
          <Input type="number" placeholder="Max price (₹)" value={maxPrice} onChange={(e) => { setMaxPrice(e.target.value); setPage(1); }} />
          <label className="flex items-center gap-2 text-sm md:col-span-2">
            <input type="checkbox" checked={availableOnly} onChange={(e) => { setAvailableOnly(e.target.checked); setPage(1); }} />
            Available only
          </label>
        </div>

        {loading ? (
          <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">{Array.from({ length: 6 }).map((_, i) => <OfferCardSkeleton key={i} />)}</div>
        ) : offers.length === 0 ? (
          <p className="py-20 text-center text-slate-500">No offers found. Check back soon!</p>
        ) : (
          <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {offers.map((offer) => <OfferCard key={offer.id} offer={offer} />)}
          </div>
        )}

        <div className="mt-8">
          <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
        </div>
      </section>
    </PublicLayout>
  );
}
