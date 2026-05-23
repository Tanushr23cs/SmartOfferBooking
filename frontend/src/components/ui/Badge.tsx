import { cn } from '@/lib/utils';

const colors: Record<string, string> = {
  Active: 'bg-emerald-100 text-emerald-800 dark:bg-emerald-900/40 dark:text-emerald-300',
  Draft: 'bg-slate-100 text-slate-700 dark:bg-slate-800 dark:text-slate-300',
  Paused: 'bg-amber-100 text-amber-800 dark:bg-amber-900/40 dark:text-amber-300',
  Expired: 'bg-red-100 text-red-800 dark:bg-red-900/40 dark:text-red-300',
  Cancelled: 'bg-red-100 text-red-800',
  Available: 'bg-emerald-100 text-emerald-800',
  Full: 'bg-orange-100 text-orange-800',
  Confirmed: 'bg-emerald-100 text-emerald-800',
  Pending: 'bg-amber-100 text-amber-800',
  Completed: 'bg-blue-100 text-blue-800',
  NoShow: 'bg-red-100 text-red-800',
};

export function Badge({ status, className }: { status: string; className?: string }) {
  return (
    <span className={cn('inline-flex rounded-full px-2.5 py-0.5 text-xs font-semibold', colors[status] || 'bg-slate-100 text-slate-700', className)}>
      {status}
    </span>
  );
}
