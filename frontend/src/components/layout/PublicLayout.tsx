import { Link } from 'react-router-dom';
import { Moon, Sun, Zap } from 'lucide-react';
import { useThemeStore } from '@/stores/themeStore';

export function PublicLayout({ children }: { children: React.ReactNode }) {
  const { dark, toggle } = useThemeStore();

  return (
    <div className="min-h-screen">
      <header className="sticky top-0 z-40 border-b border-slate-200/80 bg-white/80 backdrop-blur-lg dark:border-slate-800 dark:bg-slate-950/80">
        <div className="mx-auto flex max-w-7xl items-center justify-between px-4 py-4">
          <Link to="/" className="flex items-center gap-2 font-bold text-brand-600">
            <Zap className="h-6 w-6" />
            SmartOffer
          </Link>
          <nav className="flex items-center gap-4">
            <Link to="/" className="text-sm font-medium hover:text-brand-600">
              Offers
            </Link>
            <Link to="/profile" className="text-sm font-medium hover:text-brand-600">
              Profile
            </Link>
            <Link to="/admin/login" className="text-sm font-medium text-slate-600 hover:text-brand-600 dark:text-slate-400">
              Admin
            </Link>
            <button onClick={toggle} className="rounded-lg p-2 hover:bg-slate-100 dark:hover:bg-slate-800" aria-label="Toggle theme">
              {dark ? <Sun className="h-5 w-5" /> : <Moon className="h-5 w-5" />}
            </button>
          </nav>
        </div>
      </header>
      <main>{children}</main>
    </div>
  );
}
