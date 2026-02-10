import { useEffect } from 'react';
import { HubConnectionState } from '@microsoft/signalr';
import { getConnection, disposeConnection } from '@/lib/signalr';
import { useConnectionStore } from '@/stores/connection-store';

export function useSignalRConnection() {
  const setState = useConnectionStore((s) => s.setState);

  useEffect(() => {
    const connection = getConnection();

    function updateState() {
      switch (connection.state) {
        case HubConnectionState.Connected:
          setState('connected');
          break;
        case HubConnectionState.Connecting:
        case HubConnectionState.Reconnecting:
          setState('connecting');
          break;
        default:
          setState('disconnected');
      }
    }

    connection.onreconnecting(() => setState('connecting'));
    connection.onreconnected(() => setState('connected'));
    connection.onclose(() => setState('disconnected'));

    if (connection.state === HubConnectionState.Disconnected) {
      setState('connecting');
      connection.start().then(updateState).catch(() => setState('disconnected'));
    } else {
      updateState();
    }

    return () => {
      disposeConnection();
      setState('disconnected');
    };
  }, [setState]);
}
