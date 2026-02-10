import { RouterProvider } from 'react-router';
import { QueryProvider } from '@/providers/query-provider';
import { router } from '@/router';

export default function App() {
  return (
    <QueryProvider>
      <RouterProvider router={router} />
    </QueryProvider>
  );
}
