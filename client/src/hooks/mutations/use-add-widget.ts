import { useMutation, useQueryClient } from '@tanstack/react-query';
import { dashboardsApi } from '@/api/dashboards';
import { dashboardKeys } from '@/hooks/queries/use-dashboards';
import type { AddWidgetRequest } from '@/types/api';

export function useAddWidget() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ dashboardId, ...data }: AddWidgetRequest) =>
      dashboardsApi.addWidget(dashboardId, data),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({ queryKey: dashboardKeys.detail(variables.dashboardId) });
    },
  });
}
