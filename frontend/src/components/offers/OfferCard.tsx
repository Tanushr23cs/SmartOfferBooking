import { Link } from 'react-router-dom';
import { motion } from 'framer-motion';
import { Clock, MapPin, Users } from 'lucide-react';
import type { OfferPublic } from '@/types';
import { formatCurrency, getCountdown } from '@/lib/utils';
import { Badge } from '../ui/Badge';
import { Button } from '../ui/Button';
import { useEffect, useState } from 'react';

export function OfferCard({ offer }: { offer: OfferPublic }) {
  const [countdown, setCountdown] = useState(getCountdown(offer.endDate));

  useEffect(() => {
    const t = setInterval(() => setCountdown(getCountdown(offer.endDate)), 1000);
    return () => clearInterval(t);
  }, [offer.endDate]);

  return (
    <motion.div whileHover={{ y: -4 }} className="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-card dark:border-slate-800 dark:bg-slate-900">
      <div className="relative bg-gradient-to-br from-brand-500 to-brand-700 p-6 text-white">
        <span className="absolute right-4 top-4 rounded-full bg-white/20 px-3 py-1 text-xs font-bold">
          {offer.discountPercentage}% OFF
        </span>
        <p className="text-sm opacity-90">{offer.businessName}</p>
        <h3 className="mt-1 text-xl font-bold line-clamp-2">{offer.title}</h3>
        <p className="mt-1 text-sm opacity-80">{offer.category}</p>
      </div>
      <div className="space-y-4 p-5">
        <div className="flex items-end gap-2">
          <span className="text-2xl font-bold text-brand-600">{formatCurrency(offer.offerPrice)}</span>
          <span className="text-sm text-slate-400 line-through">{formatCurrency(offer.originalPrice)}</span>
        </div>
        <div className="flex flex-wrap gap-3 text-sm text-slate-600 dark:text-slate-400">
          <span className="flex items-center gap-1">
            <Users className="h-4 w-4" />
            {offer.availableSlotsCount} slots · {offer.totalAvailableSeats} seats
          </span>
          <span className="flex items-center gap-1">
            <MapPin className="h-4 w-4" />
            {offer.city}
          </span>
        </div>
        {!countdown.expired ? (
          <div className="flex items-center gap-2 rounded-xl bg-slate-100 px-3 py-2 text-sm dark:bg-slate-800">
            <Clock className="h-4 w-4 text-brand-600" />
            <span>
              Ends in {countdown.days}d {countdown.hours}h {countdown.minutes}m
            </span>
          </div>
        ) : (
          <Badge status="Expired" />
        )}
        <Link to={`/offers/${offer.id}`}>
          <Button className="w-full" disabled={countdown.expired || offer.totalAvailableSeats === 0}>
            {offer.totalAvailableSeats === 0 ? 'Join Waitlist' : 'Book Now'}
          </Button>
        </Link>
      </div>
    </motion.div>
  );
}
