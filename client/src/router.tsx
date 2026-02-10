import { createBrowserRouter } from 'react-router';

export const router = createBrowserRouter([
  {
    path: '/',
    lazy: () => import('@/components/layout/root-layout'),
    children: [
      {
        index: true,
        lazy: () => import('@/pages/dashboard-list/page'),
      },
      {
        path: 'dashboards/:id',
        lazy: () => import('@/pages/dashboard-detail/page'),
      },
      {
        path: 'data-sources',
        lazy: () => import('@/pages/data-sources/page'),
      },
      {
        path: 'settings',
        lazy: () => import('@/pages/settings/page'),
      },
      {
        path: '*',
        lazy: () => import('@/pages/not-found/page'),
      },
    ],
  },
]);
