import { useMutation, useQueryClient } from '@tanstack/react-query';
import { dashboardsApi } from '@/api/dashboards';
import { dashboardKeys } from '@/hooks/queries/use-dashboards';

export function useRemoveWidget() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ dashboardId, widgetId }: { dashboardId: string; widgetId: string }) =>
      dashboardsApi.removeWidget(dashboardId, widgetId),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({ queryKey: dashboardKeys.detail(variables.dashboardId) });
    },
  });
}
