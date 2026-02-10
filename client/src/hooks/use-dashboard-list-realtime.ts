import { useEffect } from 'react';
import { useQueryClient } from '@tanstack/react-query';
import { getConnection } from '@/lib/signalr';
import { HubConnectionState } from '@microsoft/signalr';
import { dashboardKeys } from '@/hooks/queries/use-dashboards';

/// Hook que escucha eventos globales de cambios en la lista de dashboards.
/// Cuando el backend notifica que un dashboard fue creado, publicado, archivado, etc.,
/// invalida la cache de la lista para refrescar automaticamente.
export function useDashboardListRealtime() {
  const queryClient = useQueryClient();

  useEffect(() => {
    const connection = getConnection();

    function handleListChanged() {
      queryClient.invalidateQueries({ queryKey: dashboardKeys.all });
    }

    connection.on('DashboardListChanged', handleListChanged);

    return () => {
      connection.off('DashboardListChanged', handleListChanged);
    };
  }, [queryClient]);
}
