import { NavLink, useLocation } from 'react-router';
import { LayoutDashboard, Database, Settings, ChevronLeft, ChevronRight, Activity, X } from 'lucide-react';
import { cn } from '@/lib/utils';
import { useSidebar } from '@/hooks/use-sidebar';
import { useMediaQuery } from '@/hooks/use-media-query';
import { Button } from '@/components/ui/button';
import { Separator } from '@/components/ui/separator';
import { ScrollArea } from '@/components/ui/scroll-area';
import { Tooltip, TooltipContent, TooltipTrigger } from '@/components/ui/tooltip';
import { ConnectionStatus } from './connection-status';
import { useEffect } from 'react';

const navItems = [
  { to: '/', icon: LayoutDashboard, label: 'Dashboards' },
  { to: '/data-sources', icon: Database, label: 'Origenes de datos' },
  { to: '/settings', icon: Settings, label: 'Configuracion' },
];

export function Sidebar() {
  const { isOpen, toggle, close } = useSidebar();
  const isMobile = useMediaQuery('(max-width: 768px)');
  const location = useLocation();

  useEffect(() => {
    if (isMobile) {
      close();
    }
  }, [location.pathname, isMobile, close]);

  if (isMobile) {
    return (
      <>
        {isOpen && (
          <div className="fixed inset-0 z-40 bg-black/60" onClick={close} />
        )}
        <aside
          className={cn(
            'fixed inset-y-0 left-0 z-50 flex w-64 flex-col border-r border-sidebar-border bg-sidebar transition-transform duration-300',
            isOpen ? 'translate-x-0' : '-translate-x-full',
          )}
        >
          <div className="flex h-14 items-center justify-between border-b border-sidebar-border px-4">
            <div className="flex items-center gap-2">
              <Activity className="h-6 w-6 text-primary" />
              <span className="text-lg font-bold text-foreground">Observa</span>
            </div>
            <Button
              variant="ghost"
              size="icon"
              onClick={close}
              className="h-8 w-8 text-sidebar-foreground hover:text-foreground"
            >
              <X className="h-4 w-4" />
            </Button>
          </div>
          <SidebarNav isOpen={true} />
          <Separator className="bg-sidebar-border" />
          <div className="p-4">
            <ConnectionStatus compact={false} />
          </div>
        </aside>
      </>
    );
  }

  return (
    <aside
      className={cn(
        'relative flex h-dvh flex-col border-r border-sidebar-border bg-sidebar transition-all duration-300',
        isOpen ? 'w-64' : 'w-16',
      )}
    >
      <div className={cn('flex h-14 items-center border-b border-sidebar-border px-4', isOpen ? 'justify-between' : 'justify-center')}>
        {isOpen && (
          <div className="flex items-center gap-2">
            <Activity className="h-6 w-6 text-primary" />
            <span className="text-lg font-bold text-foreground">Observa</span>
          </div>
        )}
        <Button
          variant="ghost"
          size="icon"
          onClick={toggle}
          className="h-8 w-8 text-sidebar-foreground hover:text-foreground"
        >
          {isOpen ? <ChevronLeft className="h-4 w-4" /> : <ChevronRight className="h-4 w-4" />}
        </Button>
      </div>

      <SidebarNav isOpen={isOpen} />

      <Separator className="bg-sidebar-border" />
      <div className={cn('p-4', !isOpen && 'flex justify-center p-2')}>
        <ConnectionStatus compact={!isOpen} />
      </div>
    </aside>
  );
}

function SidebarNav({ isOpen }: { isOpen: boolean }) {
  return (
    <ScrollArea className="flex-1 py-4">
      <nav className="flex flex-col gap-1 px-2">
        {navItems.map((item) => (
          <Tooltip key={item.to}>
            <TooltipTrigger asChild>
              <NavLink
                to={item.to}
                end={item.to === '/'}
                className={({ isActive }) =>
                  cn(
                    'flex items-center gap-3 rounded-lg px-3 py-2 text-sm transition-colors',
                    isActive
                      ? 'bg-sidebar-accent text-sidebar-accent-foreground font-medium'
                      : 'text-sidebar-foreground hover:bg-sidebar-accent hover:text-sidebar-accent-foreground',
                    !isOpen && 'justify-center px-2',
                  )
                }
              >
                <item.icon className="h-5 w-5 shrink-0" />
                {isOpen && <span>{item.label}</span>}
              </NavLink>
            </TooltipTrigger>
            {!isOpen && (
              <TooltipContent side="right">
                <p>{item.label}</p>
              </TooltipContent>
            )}
          </Tooltip>
        ))}
      </nav>
    </ScrollArea>
  );
}
