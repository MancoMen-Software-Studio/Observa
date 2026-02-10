import { useQuery } from '@tanstack/react-query';
import { dataSourcesApi } from '@/api/data-sources';

export const dataSourceKeys = {
  all: ['dataSources'] as const,
};

export function useDataSources() {
  return useQuery({
    queryKey: dataSourceKeys.all,
    queryFn: () => dataSourcesApi.getAll(),
  });
}
