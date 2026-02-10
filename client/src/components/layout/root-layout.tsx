import { Outlet } from 'react-router';
import { TooltipProvider } from '@/components/ui/tooltip';
import { Toaster } from '@/components/ui/toaster';
import { useSignalRConnection } from '@/hooks/use-signalr-connection';
import { Sidebar } from './sidebar';
import { Header } from './header';

export function Component() {
  useSignalRConnection();

  return (
    <TooltipProvider delayDuration={300}>
      <div className="flex min-h-dvh bg-background text-foreground">
        <Sidebar />
        <div className="flex flex-1 flex-col overflow-hidden">
          <Header />
          <main className="flex-1 overflow-auto p-6">
            <Outlet />
          </main>
        </div>
      </div>
      <Toaster />
    </TooltipProvider>
  );
}
