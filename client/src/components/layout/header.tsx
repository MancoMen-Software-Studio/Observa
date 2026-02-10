import { useLocation } from 'react-router';
import { Menu } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { useSidebar } from '@/hooks/use-sidebar';
import { useMediaQuery } from '@/hooks/use-media-query';
import { Breadcrumb } from './breadcrumb';

export function Header() {
  const location = useLocation();
  const { open } = useSidebar();
  const isMobile = useMediaQuery('(max-width: 768px)');

  return (
    <header className="flex h-14 items-center gap-3 border-b px-4 sm:px-6">
      {isMobile && (
        <Button variant="ghost" size="icon" className="h-8 w-8 shrink-0" onClick={open}>
          <Menu className="h-5 w-5" />
        </Button>
      )}
      <Breadcrumb pathname={location.pathname} />
    </header>
  );
}
