# Smart Offer Slot Booking System

Production-grade **full-stack** real-time booking platform for limited-time offers (gym trials, salon slots, restaurant deals, and more).

## Stack

| Layer | Technology |
|-------|------------|
| Frontend | React 18, TypeScript, Vite, Tailwind, Zustand, SignalR, React Hook Form, Zod |
| Backend | .NET 8 Web API, EF Core, JWT, SignalR, Swagger |
| Database | PostgreSQL (Supabase in production) |
| Deploy | Vercel (frontend), Render (API), Supabase (DB) |

## Project structure

```
SmartOfferBooking/
├── frontend/                 # React SPA (Vercel)
├── src/
│   ├── SmartOfferBooking.API/
│   ├── SmartOfferBooking.Application/
│   ├── SmartOfferBooking.Domain/
│   └── SmartOfferBooking.Infrastructure/
├── Dockerfile                # Render deployment
├── render.yaml
├── docker-compose.yml        # Local Postgres
├── DEPLOYMENT.md             # Full deploy guide
└── README.md
```

## Quick start (local)

### 1. Database

```bash
docker compose up -d
```

### 2. Backend

```bash
dotnet restore
dotnet ef database update --project src/SmartOfferBooking.Infrastructure --startup-project src/SmartOfferBooking.API
dotnet run --project src/SmartOfferBooking.API
```

API: http://localhost:5000/swagger

### 3. Frontend (requires API running on port 5000)

**Terminal 1 — backend (keep open):**
```powershell
cd SmartOfferBooking
.\start-backend.ps1
```

**Terminal 2 — frontend:**
```powershell
cd SmartOfferBooking
.\start-frontend.ps1
```

Or manually:
```bash
cd frontend
npm install
npm run dev
```

App: http://localhost:5173

### Fix `ECONNREFUSED` / Vite proxy errors

These mean the **API is not running**. The frontend proxies `/api` and `/hubs` to `http://localhost:5000`.

1. Start `.\start-backend.ps1` and wait for `Now listening on: http://localhost:5000`
2. Then start `npm run dev` in `frontend/`
3. Verify: open http://localhost:5000/health — should return `healthy`

### Default admin

| Email | Password |
|-------|----------|
| `admin@smartoffer.local` | `Admin@123` |

## Features

### Public website
- Offer listing with filters (category, business type, price, availability)
- Real-time seat updates via SignalR
- Offer detail + slot selection + booking form
- Waitlist when slots are full
- Booking confirmation with QR code

### Admin dashboard
- JWT email/password auth with persisted session
- Business profile CRUD
- Offers CRUD with search, filters, pagination
- Slots management
- Bookings management + status updates
- Live dashboard metrics + charts
- SignalR live updates

### Backend
- Clean architecture (Domain / Application / Infrastructure / API)
- Transactional booking with concurrency safety
- Waitlist API
- Paginated offers & bookings
- Supabase-ready PostgreSQL connection

## API overview

See Swagger at `/swagger` when running locally.

Key public endpoints:
- `GET /api/offers?publicOnly=true` — paginated public offers
- `GET /api/offers/{id}?publicView=true`
- `GET /api/offers/{offerId}/slots`
- `POST /api/bookings`
- `POST /api/waitlist`
- `GET /api/bookings/reference/{ref}`

SignalR hub: `/hubs/booking`  
Events: `SlotUpdated`, `BookingCreated`, `OfferUpdated`

## Deployment

See **[DEPLOYMENT.md](./DEPLOYMENT.md)** for Supabase, Render, and Vercel setup.

## Environment variables

### Frontend (`frontend/.env`)

VITE_API_URL=https://smartoffer-api-pite.onrender.com
VITE_HUB_URL=https://smartoffer-api-pite.onrender.com/hubs/booking

### Backend

```
ConnectionStrings__DefaultConnection=<supabase-connection-string>
Jwt__Secret=<32+ char secret>
CORS_ALLOWED_ORIGINS=https://your-app.vercel.app
```

## License

MIT
