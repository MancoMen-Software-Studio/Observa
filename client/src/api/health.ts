import { api } from '@/lib/api-client';
import type { HealthResponse } from '@/types/api';

export const healthApi = {
  check() {
    return api.get<HealthResponse>('/health');
  },
};
