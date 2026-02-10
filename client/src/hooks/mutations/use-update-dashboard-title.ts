import { useMutation, useQueryClient } from '@tanstack/react-query';
import { dashboardsApi } from '@/api/dashboards';
import { dashboardKeys } from '@/hooks/queries/use-dashboards';

export function useUpdateDashboardTitle() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, newTitle }: { id: string; newTitle: string }) =>
      dashboardsApi.updateTitle(id, { newTitle }),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({ queryKey: dashboardKeys.detail(variables.id) });
      queryClient.invalidateQueries({ queryKey: dashboardKeys.all });
    },
  });
}
