import { Link } from 'react-router';
import { ChevronRight, Home } from 'lucide-react';
import { Fragment } from 'react';

interface BreadcrumbProps {
  pathname: string;
}

const labelMap: Record<string, string> = {
  dashboards: 'Dashboards',
  'data-sources': 'Origenes de datos',
  settings: 'Configuracion',
};

const pathOverrides: Record<string, string> = {
  '/dashboards': '/',
};

export function Breadcrumb({ pathname }: BreadcrumbProps) {
  const segments = pathname.split('/').filter(Boolean);

  return (
    <nav className="flex items-center gap-1 text-sm">
      <Link to="/" className="flex items-center text-muted-foreground hover:text-foreground transition-colors">
        <Home className="h-4 w-4" />
      </Link>
      {segments.map((segment, index) => {
        const rawPath = '/' + segments.slice(0, index + 1).join('/');
        const path = pathOverrides[rawPath] ?? rawPath;
        const label = labelMap[segment] ?? segment;
        const isLast = index === segments.length - 1;

        return (
          <Fragment key={rawPath}>
            <ChevronRight className="h-4 w-4 text-muted-foreground" />
            {isLast ? (
              <span className="font-medium text-foreground">{label}</span>
            ) : (
              <Link to={path} className="text-muted-foreground hover:text-foreground transition-colors">
                {label}
              </Link>
            )}
          </Fragment>
        );
      })}
    </nav>
  );
}
