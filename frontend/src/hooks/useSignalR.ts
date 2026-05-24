import { useEffect, useRef, useState } from 'react';
import type { HubConnection } from '@microsoft/signalr';
import { createHubConnection } from '@/lib/signalr';
import type { OfferUpdatedEvent, SlotUpdatedEvent } from '@/types';
import { useAuthStore } from '@/stores/authStore';

type BookingCreatedHandler = (data: { bookingReference: string; offerId: string; slotId: string }) => void;

export function useOfferSignalR(
  offerId: string | undefined,
  onSlotUpdated?: (event: SlotUpdatedEvent) => void,
  onOfferUpdated?: (event: OfferUpdatedEvent) => void,
  onBookingCreated?: BookingCreatedHandler
) {
  const connectionRef = useRef<HubConnection | null>(null);
  const [connected, setConnected] = useState(false);

  useEffect(() => {
    if (!offerId) return;

    const connection = createHubConnection();
    connectionRef.current = connection;

    const start = async () => {
      try {
        await connection.start();
        console.log("SignalR Connected");
        await connection.invoke('JoinOfferGroup', offerId);
        setConnected(true);
      } catch (err) {
        console.error("SignalR Connection Error:", err);
        setConnected(false);
      }
    };

    if (onSlotUpdated) connection.on('SlotUpdated', onSlotUpdated);
    if (onOfferUpdated) connection.on('OfferUpdated', onOfferUpdated);
    if (onBookingCreated) connection.on('BookingCreated', onBookingCreated);

    connection.onreconnected(async () => {
      await connection.invoke('JoinOfferGroup', offerId);
      setConnected(true);
    });

    start();

    return () => {
      connection.stop();
      connectionRef.current = null;
      setConnected(false);
    };
  }, [offerId, onSlotUpdated, onOfferUpdated, onBookingCreated]);

  return { connected };
}

export function useAdminSignalR(
  onSlotUpdated?: (event: SlotUpdatedEvent) => void,
  onBookingCreated?: BookingCreatedHandler,
  onOfferUpdated?: (event: OfferUpdatedEvent) => void
) {
  const token = useAuthStore((s) => s.token);
  const [connected, setConnected] = useState(false);

  useEffect(() => {
    if (!token) return;

    const connection = createHubConnection(token);

    const start = async () => {
      try {
        await connection.start();
        console.log("SignalR Connected");
        await connection.invoke('JoinAdminDashboard');
        setConnected(true);
      } catch (err) {
        console.error("SignalR Connection Error:", err);
        setConnected(false);
      }
    };

    if (onSlotUpdated) connection.on('SlotUpdated', onSlotUpdated);
    if (onBookingCreated) connection.on('BookingCreated', onBookingCreated);
    if (onOfferUpdated) connection.on('OfferUpdated', onOfferUpdated);

    connection.onreconnected(async () => {
      await connection.invoke('JoinAdminDashboard');
      setConnected(true);
    });

    start();

    return () => {
      connection.stop();
      setConnected(false);
    };
  }, [token, onSlotUpdated, onBookingCreated, onOfferUpdated]);

  return { connected };
}
