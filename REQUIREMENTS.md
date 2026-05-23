# Requirements Compliance Checklist

## User Roles
- [x] Admin / Business Owner — full management via `/admin`
- [x] Customer — public pages at `/` and `/offers/:id`

## A. Admin Login
- [x] Email + Password
- [x] JWT persistence (Zustand + localStorage)
- [x] Redirect to dashboard after login

## B. Business Profile
- [x] All fields (name, type, owner, phone, email, address, city, logo URL, hours)
- [x] Business types: Restaurant, Gym, Salon, Clinic, Coaching, Turf, Other

## C. Offer Management
- [x] All offer fields including discount (auto-calculated server-side)
- [x] Statuses: Draft, Active, Paused, Expired, Cancelled
- [x] Dedicated **Create Offer** page (`/admin/offers/create`)
- [x] **Manage Offers** page with search, status filter, pause, cancel

## D. Slot Management
- [x] All slot fields; capacity / booked / available counts
- [x] Slot statuses enforced in backend

## E. Public Offer Listing
- [x] Filters: Business Type, Category, **Date**, Price Range, Available Only
- [x] Card: title, business, prices, discount, slots/seats, countdown, Book Now
- [x] Real-time updates (SignalR `OfferUpdated`, `SlotUpdated`, `BookingCreated`)

## F. Public Offer Detail
- [x] Business, offer, pricing, discount, slots, terms, **location**, book form

## G. Booking Flow
- [x] Form fields + validations (server-side)
- [x] Confirmation: reference, offer, business, slot date/time, customer, status
- [x] Waitlist when slot full
- [x] QR code on confirmation

## H. Admin Dashboard
- [x] All 8 metrics
- [x] Recent bookings table: customer, offer, slot time, people, status, action
- [x] Live refresh via SignalR

## Minimum Screens (8/8)
1. Admin Login  
2. Admin Dashboard  
3. Create Offer Page  
4. Manage Offers  
5. Manage Bookings  
6. Public Offer Listing  
7. Public Offer Detail  
8. Booking Confirmation  

## Business Rules
- [x] Offer price < original price  
- [x] No booking after expiry  
- [x] No booking when full  
- [x] Booked count increases on booking  
- [x] Unique booking reference  
- [x] Admin pause/cancel offer  
- [x] Cancelled/expired hidden from public listing  

## Bonus Features Implemented
- [x] QR code  
- [x] Countdown timer  
- [x] Waitlist  
- [x] Dark/light mode  
- [x] Responsive mobile UI  
- [x] SignalR real-time (no page refresh)
