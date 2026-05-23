# Deployment Guide

## Architecture

| Component | Platform | URL example |
|-----------|----------|-------------|
| Frontend | Vercel | `https://your-app.vercel.app` |
| Backend API | Render | `https://smart-offer-api.onrender.com` |
| Database | Supabase PostgreSQL | Connection string from dashboard |

---

## 1. Supabase (Database)

1. Create a project at [supabase.com](https://supabase.com)
2. Go to **Project Settings → Database**
3. Copy the **Connection string** (URI mode), e.g.:

```
Host=db.xxxxx.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=YOUR_PASSWORD;SSL Mode=Require;Trust Server Certificate=true
```

4. Use this as `ConnectionStrings__DefaultConnection` on Render

---

## 2. Render (Backend)

### Option A: Docker (recommended)

1. Push repo to GitHub
2. Create **Web Service** on Render → connect repo
3. Set **Root Directory** to `SmartOfferBooking` (or repo root if monorepo)
4. Environment variables:

| Key | Value |
|-----|-------|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ConnectionStrings__DefaultConnection` | Supabase connection string |
| `Jwt__Secret` | Long random string (32+ chars) |
| `Jwt__Issuer` | `SmartOfferBooking` |
| `Jwt__Audience` | `SmartOfferBookingClients` |
| `CORS_ALLOWED_ORIGINS` | `https://your-app.vercel.app` |

5. Deploy using `Dockerfile` in repo root

### Option B: Native .NET

- Build command: `dotnet publish src/SmartOfferBooking.API -c Release -o out`
- Start command: `dotnet out/SmartOfferBooking.API.dll`

### Migrations

On first deploy, migrations run automatically via `DbSeeder.SeedAsync` in `Program.cs`.

Manual:

```bash
dotnet ef database update --project src/SmartOfferBooking.Infrastructure --startup-project src/SmartOfferBooking.API
```

### Health check

Render health path: `/health`

---

## 3. Vercel (Frontend)

1. Import GitHub repo
2. Set **Root Directory** to `SmartOfferBooking/frontend`
3. Framework: **Vite**
4. Environment variables:

| Key | Value |
|-----|-------|
| `VITE_API_URL` | `https://your-api.onrender.com` |
| `VITE_HUB_URL` | `https://your-api.onrender.com/hubs/booking` |

5. Deploy

---

## 4. Local development

```bash
# Database (optional local Postgres)
cd SmartOfferBooking && docker compose up -d

# Backend
dotnet run --project src/SmartOfferBooking.API

# Frontend
cd frontend
cp .env.example .env
npm install
npm run dev
```

Default admin: `admin@smartoffer.local` / `Admin@123`

---

## 5. Troubleshooting

| Issue | Fix |
|-------|-----|
| CORS errors | Add Vercel URL to `CORS_ALLOWED_ORIGINS` on Render |
| SignalR fails | Ensure `VITE_HUB_URL` is correct; WebSockets enabled on Render |
| 401 on admin | Re-login; check JWT secret matches |
| DB connection failed | Verify Supabase SSL + password; allow Render IP if restricted |
| Empty offers | Create business profile → offers (Active) → slots |

---

## 6. Security checklist

- [ ] Change `Jwt__Secret` in production
- [ ] Use Supabase strong password
- [ ] Restrict CORS to your domains only
- [ ] Enable HTTPS only on Vercel and Render
