import { api } from '@/lib/api-client';
import type {
  DashboardResponse,
  CreateDashboardRequest,
  UpdateTitleRequest,
  AddWidgetRequest,
  CreatedResponse,
  PagedResponse,
} from '@/types/api';

const BASE = '/api/dashboards';

export interface GetDashboardsParams {
  page?: number;
  pageSize?: number;
  status?: string;
  search?: string;
}

export const dashboardsApi = {
  getAll(params?: GetDashboardsParams) {
    const query = new URLSearchParams();
    if (params?.page) query.set('page', String(params.page));
    if (params?.pageSize) query.set('pageSize', String(params.pageSize));
    if (params?.status && params.status !== 'all') query.set('status', params.status);
    if (params?.search) query.set('search', params.search);
    const qs = query.toString();
    return api.get<PagedResponse<DashboardResponse>>(qs ? `${BASE}?${qs}` : BASE);
  },

  getById(id: string) {
    return api.get<DashboardResponse>(`${BASE}/${id}`);
  },

  create(data: CreateDashboardRequest) {
    return api.post<CreatedResponse>(BASE, data);
  },

  updateTitle(id: string, data: UpdateTitleRequest) {
    return api.put<void>(`${BASE}/${id}/title`, data);
  },

  publish(id: string) {
    return api.post<void>(`${BASE}/${id}/publish`);
  },

  archive(id: string) {
    return api.post<void>(`${BASE}/${id}/archive`);
  },

  addWidget(id: string, data: Omit<AddWidgetRequest, 'dashboardId'>) {
    return api.post<CreatedResponse>(`${BASE}/${id}/widgets`, data);
  },

  removeWidget(dashboardId: string, widgetId: string) {
    return api.delete<void>(`${BASE}/${dashboardId}/widgets/${widgetId}`);
  },
};
