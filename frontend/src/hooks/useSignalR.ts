import { useEffect, useRef, useState } from 'react';
import type { HubConnection } from '@microsoft/signalr';
import { getOrCreateHubConnection, safeStartConnection } from '@/lib/signalr';
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

    const connection = getOrCreateHubConnection();
    connectionRef.current = connection;

    const start = async () => {
      try {
        await safeStartConnection(connection);
        await connection.invoke('JoinOfferGroup', offerId);
        setConnected(true);
      } catch (err) {
        console.error('SignalR Connection Error:', err);
        setConnected(false);
      }
    };

    if (onSlotUpdated) connection.on('SlotUpdated', onSlotUpdated);
    if (onOfferUpdated) connection.on('OfferUpdated', onOfferUpdated);
    if (onBookingCreated) connection.on('BookingCreated', onBookingCreated);

    connection.onreconnected(async () => {
      try {
        await connection.invoke('JoinOfferGroup', offerId);
        setConnected(true);
      } catch (err) {
        console.error('SignalR Reconnect Error:', err);
        setConnected(false);
      }
    });

    start();

    return () => {
      connection.off('SlotUpdated');
      connection.off('OfferUpdated');
      connection.off('BookingCreated');
      connectionRef.current = null;
      setConnected(false);
      // Do NOT stop the shared connection on unmount — other components may still use it.
    };
    // Only recreate connection when offerId changes. Handlers may change but
    // we avoid restarting on handler identity changes to prevent duplicate
    // connection starts.
  }, [offerId]);

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

    const connection = getOrCreateHubConnection(token);

    const start = async () => {
      try {
        await safeStartConnection(connection);
        await connection.invoke('JoinAdminDashboard');
        setConnected(true);
      } catch (err) {
        console.error('SignalR Connection Error:', err);
        setConnected(false);
      }
    };

    if (onSlotUpdated) connection.on('SlotUpdated', onSlotUpdated);
    if (onBookingCreated) connection.on('BookingCreated', onBookingCreated);
    if (onOfferUpdated) connection.on('OfferUpdated', onOfferUpdated);

    connection.onreconnected(async () => {
      try {
        await connection.invoke('JoinAdminDashboard');
        setConnected(true);
      } catch (err) {
        console.error('SignalR Reconnect Error:', err);
        setConnected(false);
      }
    });

    start();

    return () => {
      connection.off('SlotUpdated');
      connection.off('OfferUpdated');
      connection.off('BookingCreated');
      setConnected(false);
      // Do NOT stop the shared connection on unmount — it is shared globally.
    };
    // Only recreate when token changes; ignore handler identity to avoid
    // accidental restarts when handlers are re-created by parent components.
  }, [token]);

  return { connected };
}
