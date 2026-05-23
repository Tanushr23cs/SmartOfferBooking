import { Link, NavLink, Outlet, useNavigate } from 'react-router-dom';
import { BarChart3, Building2, Calendar, LayoutDashboard, LogOut, Moon, Sun, Tag, Zap } from 'lucide-react';
import { useAuthStore } from '@/stores/authStore';
import { useThemeStore } from '@/stores/themeStore';
import { Button } from '../ui/Button';

const nav = [
  { to: '/admin', icon: LayoutDashboard, label: 'Dashboard', end: true },
  { to: '/admin/business', icon: Building2, label: 'Business' },
  { to: '/admin/offers', icon: Tag, label: 'Offers' },
  { to: '/admin/slots', icon: Calendar, label: 'Slots' },
  { to: '/admin/bookings', icon: BarChart3, label: 'Bookings' },
];

export function AdminLayout() {
  const logout = useAuthStore((s) => s.logout);
  const username = useAuthStore((s) => s.username);
  const navigate = useNavigate();
  const { dark, toggle } = useThemeStore();

  const handleLogout = () => {
    logout();
    navigate('/admin/login');
  };

  return (
    <div className="flex min-h-screen">
      <aside className="hidden w-64 flex-col border-r border-slate-200 bg-white p-4 dark:border-slate-800 dark:bg-slate-900 md:flex">
        <div className="mb-8 flex items-center gap-2 font-bold text-brand-600">
          <Zap className="h-6 w-6" />
          Admin Panel
        </div>
        <nav className="flex flex-1 flex-col gap-1">
          {nav.map(({ to, icon: Icon, label, end }) => (
            <NavLink
              key={to}
              to={to}
              end={end}
              className={({ isActive }) =>
                `flex items-center gap-3 rounded-xl px-3 py-2.5 text-sm font-medium transition ${isActive ? 'bg-brand-50 text-brand-700 dark:bg-brand-900/30 dark:text-brand-300' : 'text-slate-600 hover:bg-slate-100 dark:text-slate-400 dark:hover:bg-slate-800'}`
              }
            >
              <Icon className="h-5 w-5" />
              {label}
            </NavLink>
          ))}
        </nav>
        <div className="mt-auto space-y-2 border-t border-slate-200 pt-4 dark:border-slate-800">
          <p className="truncate text-xs text-slate-500">{username}</p>
          <Button variant="ghost" className="w-full justify-start" onClick={toggle}>
            {dark ? <Sun className="h-4 w-4" /> : <Moon className="h-4 w-4" />}
            Theme
          </Button>
          <Button variant="ghost" className="w-full justify-start text-red-600" onClick={handleLogout}>
            <LogOut className="h-4 w-4" />
            Logout
          </Button>
        </div>
      </aside>
      <div className="flex-1 overflow-auto bg-slate-50 dark:bg-slate-950">
        <div className="border-b border-slate-200 bg-white px-4 py-3 md:hidden dark:border-slate-800 dark:bg-slate-900">
          <div className="flex gap-2 overflow-x-auto">
            {nav.map(({ to, label }) => (
              <Link key={to} to={to} className="whitespace-nowrap rounded-lg bg-slate-100 px-3 py-1.5 text-sm dark:bg-slate-800">
                {label}
              </Link>
            ))}
          </div>
        </div>
        <div className="mx-auto max-w-7xl p-4 md:p-8">
          <Outlet />
        </div>
      </div>
    </div>
  );
}
