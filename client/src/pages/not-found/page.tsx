import { Link } from 'react-router';
import { PageTransition } from '@/components/shared/page-transition';

export function Component() {
  return (
    <PageTransition>
      <div className="flex flex-col items-center justify-center gap-4 py-24 text-center" role="alert">
        <h1 className="text-6xl font-bold text-muted-foreground">404</h1>
        <p className="text-lg text-muted-foreground">Pagina no encontrada</p>
        <Link
          to="/"
          className="rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground hover:bg-primary/90 transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
        >
          Volver al inicio
        </Link>
      </div>
    </PageTransition>
  );
}
