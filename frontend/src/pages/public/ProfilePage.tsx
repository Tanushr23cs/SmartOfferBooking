import { useCallback, useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import toast from 'react-hot-toast';
import { motion } from 'framer-motion';
import { MapPin, Clock, Phone, Mail, Store } from 'lucide-react';
import { businessApi, offersApi } from '@/lib/api';
import type { BusinessProfile, OfferPublic, PagedResult } from '@/types';
import { PublicLayout } from '@/components/layout/PublicLayout';
import { OfferCard } from '@/components/offers/OfferCard';
import { OfferCardSkeleton } from '@/components/ui/Skeleton';
import { Card } from '@/components/ui/Card';
import { formatTime } from '@/lib/utils';

export function ProfilePage() {
  const [business, setBusiness] = useState<BusinessProfile | null>(null);
  const [offers, setOffers] = useState<OfferPublic[]>([]);
  const [loadingProfile, setLoadingProfile] = useState(true);
  const [loadingOffers, setLoadingOffers] = useState(true);

  useEffect(() => {
    businessApi
      .get()
      .then((p) => setBusiness(p))
      .catch(() => setBusiness(null))
      .finally(() => setLoadingProfile(false));
  }, []);

  const loadOffers = useCallback(async () => {
    setLoadingOffers(true);
    try {
      const result = (await offersApi.getPaged({
        page: 1,
        pageSize: 50,
        publicOnly: true,
      })) as PagedResult<OfferPublic>;
      setOffers(result.items as OfferPublic[]);
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Failed to load offers');
    } finally {
      setLoadingOffers(false);
    }
  }, []);

  useEffect(() => {
    loadOffers();
  }, [loadOffers]);

  return (
    <PublicLayout>
      {/* Business Profile Header */}
      <section className="bg-gradient-to-br from-brand-600 via-brand-500 to-orange-400 px-4 py-16 text-white">
        <div className="mx-auto max-w-5xl">
          {loadingProfile ? (
            <div className="animate-pulse space-y-4">
              <div className="h-10 w-64 rounded-xl bg-white/20" />
              <div className="h-5 w-96 rounded-lg bg-white/20" />
            </div>
          ) : business ? (
            <motion.div initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }}>
              <div className="flex items-center gap-5">
                {business.logoUrl ? (
                  <img
                    src={business.logoUrl}
                    alt={business.businessName}
                    className="h-20 w-20 rounded-2xl border-2 border-white/30 object-cover shadow-lg"
                  />
                ) : (
                  <div className="flex h-20 w-20 items-center justify-center rounded-2xl border-2 border-white/30 bg-white/10 shadow-lg">
                    <Store className="h-10 w-10 opacity-80" />
                  </div>
                )}
                <div>
                  <h1 className="text-3xl font-extrabold md:text-4xl">{business.businessName}</h1>
                  <p className="mt-1 text-lg opacity-90">{business.businessType}</p>
                </div>
              </div>

              <div className="mt-6 flex flex-wrap gap-6 text-sm opacity-90">
                {(business.address || business.city) && (
                  <span className="flex items-center gap-2">
                    <MapPin className="h-4 w-4" />
                    {[business.address, business.city].filter(Boolean).join(', ')}
                  </span>
                )}
                {business.phone && (
                  <span className="flex items-center gap-2">
                    <Phone className="h-4 w-4" />
                    {business.phone}
                  </span>
                )}
                {business.email && (
                  <span className="flex items-center gap-2">
                    <Mail className="h-4 w-4" />
                    {business.email}
                  </span>
                )}
                {business.openingTime && business.closingTime && (
                  <span className="flex items-center gap-2">
                    <Clock className="h-4 w-4" />
                    {formatTime(business.openingTime)} – {formatTime(business.closingTime)}
                  </span>
                )}
              </div>
            </motion.div>
          ) : (
            <p className="text-lg opacity-80">Business profile not available.</p>
          )}
        </div>
      </section>

      {/* Offers Section */}
      <section className="mx-auto max-w-7xl px-4 py-10">
        <h2 className="mb-6 text-2xl font-bold">Our Offers</h2>

        {loadingOffers ? (
          <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {Array.from({ length: 6 }).map((_, i) => (
              <OfferCardSkeleton key={i} />
            ))}
          </div>
        ) : offers.length === 0 ? (
          <Card className="py-16 text-center">
            <p className="text-slate-500">No active offers right now. Check back soon!</p>
          </Card>
        ) : (
          <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {offers.map((offer) => (
              <OfferCard key={offer.id} offer={offer} />
            ))}
          </div>
        )}
      </section>
    </PublicLayout>
  );
}
