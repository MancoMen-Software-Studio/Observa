import { useMutation, useQueryClient } from '@tanstack/react-query';
import { dashboardsApi } from '@/api/dashboards';
import { dashboardKeys } from '@/hooks/queries/use-dashboards';

export function usePublishDashboard() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => dashboardsApi.publish(id),
    onSuccess: (_data, id) => {
      queryClient.invalidateQueries({ queryKey: dashboardKeys.detail(id) });
      queryClient.invalidateQueries({ queryKey: dashboardKeys.all });
    },
  });
}
