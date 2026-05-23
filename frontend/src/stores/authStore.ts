import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import { authApi } from '@/lib/api';

interface AuthState {
  token: string | null;
  email: string | null;
  username: string | null;
  role: string | null;
  expiresAt: string | null;
  isAuthenticated: boolean;
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      token: null,
      email: null,
      username: null,
      role: null,
      expiresAt: null,
      isAuthenticated: false,
      login: async (email, password) => {
        const res = await authApi.login(email, password);
        localStorage.setItem('auth_token', res.token);
        set({
          token: res.token,
          email: res.email,
          username: res.username,
          role: res.role,
          expiresAt: res.expiresAt,
          isAuthenticated: true,
        });
      },
      logout: () => {
        localStorage.removeItem('auth_token');
        set({
          token: null,
          email: null,
          username: null,
          role: null,
          expiresAt: null,
          isAuthenticated: false,
        });
      },
    }),
    {
      name: 'auth-storage',
      partialize: (s) => ({
        token: s.token,
        email: s.email,
        username: s.username,
        role: s.role,
        expiresAt: s.expiresAt,
        isAuthenticated: s.isAuthenticated,
      }),
      onRehydrateStorage: () => (state) => {
        if (state?.token) localStorage.setItem('auth_token', state.token);
      },
    }
  )
);
