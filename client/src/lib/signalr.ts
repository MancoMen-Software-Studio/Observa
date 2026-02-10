import * as signalR from '@microsoft/signalr';

let connection: signalR.HubConnection | null = null;

export function getConnection(): signalR.HubConnection {
  if (!connection) {
    connection = new signalR.HubConnectionBuilder()
      .withUrl('/hubs/dashboard')
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds(retryContext) {
          const delays = [0, 1000, 2000, 5000, 10000, 30000];
          return delays[Math.min(retryContext.previousRetryCount, delays.length - 1)] ?? 30000;
        },
      })
      .configureLogging(signalR.LogLevel.Warning)
      .build();
  }

  return connection;
}

export function disposeConnection() {
  if (connection) {
    connection.stop();
    connection = null;
  }
}
