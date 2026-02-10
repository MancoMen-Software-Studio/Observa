import { api } from '@/lib/api-client';
import type { CreateDataSourceRequest, CreatedResponse, DataSourceResponse } from '@/types/api';

const BASE = '/api/datasources';

export const dataSourcesApi = {
  getAll() {
    return api.get<DataSourceResponse[]>(BASE);
  },

  create(data: CreateDataSourceRequest) {
    return api.post<CreatedResponse>(BASE, data);
  },
};
