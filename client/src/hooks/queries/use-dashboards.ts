import { useQuery, keepPreviousData } from '@tanstack/react-query';
import { dashboardsApi } from '@/api/dashboards';
import type { GetDashboardsParams } from '@/api/dashboards';

export const dashboardKeys = {
  all: ['dashboards'] as const,
  list: (params: GetDashboardsParams) => ['dashboards', 'list', params] as const,
  detail: (id: string) => ['dashboards', id] as const,
};

export function useDashboards(params?: GetDashboardsParams) {
  const queryParams = params ?? {};
  return useQuery({
    queryKey: dashboardKeys.list(queryParams),
    queryFn: () => dashboardsApi.getAll(queryParams),
    placeholderData: keepPreviousData,
  });
}

export function useDashboard(id: string) {
  return useQuery({
    queryKey: dashboardKeys.detail(id),
    queryFn: () => dashboardsApi.getById(id),
    enabled: !!id,
  });
}
