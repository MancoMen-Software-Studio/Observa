import { useMutation, useQueryClient } from '@tanstack/react-query';
import { dashboardsApi } from '@/api/dashboards';
import { dashboardKeys } from '@/hooks/queries/use-dashboards';

export function useArchiveDashboard() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => dashboardsApi.archive(id),
    onSuccess: (_data, id) => {
      queryClient.invalidateQueries({ queryKey: dashboardKeys.detail(id) });
      queryClient.invalidateQueries({ queryKey: dashboardKeys.all });
    },
  });
}
