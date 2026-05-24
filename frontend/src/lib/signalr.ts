import * as signalR from '@microsoft/signalr';

const HUB_URL = import.meta.env.VITE_HUB_URL || `${import.meta.env.VITE_API_URL || ''}/hubs/booking`;

export function createHubConnection(accessToken?: string): signalR.HubConnection {
  // SignalR connection logic (strict update)
  const token = accessToken;
  const connection = new signalR.HubConnectionBuilder()
    .withUrl(HUB_URL, {
      accessTokenFactory: () => token || ""
    })
    .withAutomaticReconnect()
    .build();

  // Attach connection event logging
  connection.onclose((err) => {
    console.log("SignalR disconnected:", err);
  });

  connection.onreconnecting(() => {
    console.log("SignalR reconnecting...");
  });

  connection.onreconnected(() => {
    console.log("SignalR reconnected");
  });

  return connection;
}
