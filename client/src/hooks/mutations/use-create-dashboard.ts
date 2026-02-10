import { useMutation, useQueryClient } from '@tanstack/react-query';
import { dashboardsApi } from '@/api/dashboards';
import { dashboardKeys } from '@/hooks/queries/use-dashboards';
import type { CreateDashboardRequest } from '@/types/api';

export function useCreateDashboard() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CreateDashboardRequest) => dashboardsApi.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: dashboardKeys.all });
    },
  });
}
