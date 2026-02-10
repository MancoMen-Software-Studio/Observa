import { useMutation, useQueryClient } from '@tanstack/react-query';
import { dataSourcesApi } from '@/api/data-sources';
import { dataSourceKeys } from '@/hooks/queries/use-data-sources';
import type { CreateDataSourceRequest } from '@/types/api';

export function useCreateDataSource() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CreateDataSourceRequest) => dataSourcesApi.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: dataSourceKeys.all });
    },
  });
}
