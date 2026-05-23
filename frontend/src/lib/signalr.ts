import * as signalR from '@microsoft/signalr';

const HUB_URL = import.meta.env.VITE_HUB_URL || `${import.meta.env.VITE_API_URL || ''}/hubs/booking`;

export function createHubConnection(accessToken?: string): signalR.HubConnection {
  const builder = new signalR.HubConnectionBuilder()
    .withUrl(accessToken ? `${HUB_URL}?access_token=${accessToken}` : HUB_URL, {
      skipNegotiation: false,
      transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling,
    })
    .withAutomaticReconnect([0, 2000, 5000, 10000, 30000]);

  return builder.build();
}
