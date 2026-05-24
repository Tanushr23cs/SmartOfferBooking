import * as signalR from '@microsoft/signalr';

const HUB_URL = import.meta.env.VITE_HUB_URL || `${import.meta.env.VITE_API_URL || ''}/hubs/booking`;

let _sharedConnection: signalR.HubConnection | null = null;
let _sharedToken: string | undefined;

/**
 * Returns a singleton HubConnection. If the token changes (e.g. login/logout),
 * the old connection is stopped and a new one is created.
 */
export function getOrCreateHubConnection(accessToken?: string): signalR.HubConnection {
  if (_sharedConnection && _sharedToken === accessToken) {
    return _sharedConnection;
  }

  // Token changed — tear down the old connection
  if (_sharedConnection) {
    _sharedConnection.stop().catch(() => {});
    _sharedConnection = null;
  }

  _sharedToken = accessToken;

  const connection = new signalR.HubConnectionBuilder()
    .withUrl(HUB_URL, {
      accessTokenFactory: accessToken ? () => accessToken : undefined,
    })
    .withAutomaticReconnect()
    .build();

  connection.onclose((err) => {
    console.log('SignalR disconnected:', err?.message ?? '', new Date().toISOString());
  });

  connection.onreconnecting((err) => {
    console.log('SignalR reconnecting...', err?.message ?? '', new Date().toISOString());
  });

  connection.onreconnected((connectionId) => {
    console.log('SignalR reconnected', connectionId ?? '', new Date().toISOString());
  });

  _sharedConnection = connection;
  return connection;
}

/**
 * Safely starts the given connection if it is not already connected/connecting.
 */
export async function safeStartConnection(connection: signalR.HubConnection): Promise<void> {
  if (connection.state === signalR.HubConnectionState.Connected ||
      connection.state === signalR.HubConnectionState.Connecting ||
      connection.state === signalR.HubConnectionState.Reconnecting) {
    return;
  }
  try {
    await connection.start();
    console.log('SignalR Connected');
  } catch (err) {
    console.error('SignalR Connection Error:', err);
  }
}

/** @deprecated Use getOrCreateHubConnection instead */
export function createHubConnection(accessToken?: string): signalR.HubConnection {
  return getOrCreateHubConnection(accessToken);
}
