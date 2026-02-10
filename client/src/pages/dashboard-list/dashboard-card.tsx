import { Link } from 'react-router';
import { LayoutDashboard, MoreVertical, Pencil, Upload, Archive } from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import type { DashboardResponse, DashboardStatus } from '@/types/api';
import { DASHBOARD_STATUS_LABELS } from '@/types/enums';
import { useState } from 'react';

interface DashboardCardProps {
  dashboard: DashboardResponse;
  onPublish: (id: string) => void;
  onArchive: (id: string) => void;
}

const statusVariant: Record<DashboardStatus, 'default' | 'success' | 'secondary'> = {
  Draft: 'secondary',
  Published: 'success',
  Archived: 'default',
};

export function DashboardCard({ dashboard, onPublish, onArchive }: DashboardCardProps) {
  const [menuOpen, setMenuOpen] = useState(false);

  return (
    <Card className="group relative transition-colors hover:border-primary/50">
      <Link to={`/dashboards/${dashboard.id}`} className="absolute inset-0 z-0" />
      <CardHeader className="flex flex-row items-start justify-between gap-2">
        <div className="flex items-center gap-3">
          <div className="rounded-lg bg-primary/10 p-2">
            <LayoutDashboard className="h-5 w-5 text-primary" />
          </div>
          <div>
            <CardTitle className="text-base">{dashboard.title}</CardTitle>
            <p className="mt-1 text-xs text-muted-foreground">
              {dashboard.widgets.length} widget{dashboard.widgets.length !== 1 && 's'}
            </p>
          </div>
        </div>
        <div className="relative z-10">
          <Button
            variant="ghost"
            size="icon"
            className="h-8 w-8"
            onClick={(e) => {
              e.preventDefault();
              setMenuOpen(!menuOpen);
            }}
          >
            <MoreVertical className="h-4 w-4" />
          </Button>
          {menuOpen && (
            <>
              <div className="fixed inset-0 z-20" onClick={() => setMenuOpen(false)} />
              <div className="absolute right-0 top-full z-30 mt-1 w-44 rounded-md border bg-popover p-1 shadow-lg">
                <Link
                  to={`/dashboards/${dashboard.id}`}
                  className="flex w-full items-center gap-2 rounded-sm px-2 py-1.5 text-sm hover:bg-accent"
                  onClick={() => setMenuOpen(false)}
                >
                  <Pencil className="h-4 w-4" />
                  Editar
                </Link>
                {dashboard.status === 'Draft' && (
                  <button
                    className="flex w-full items-center gap-2 rounded-sm px-2 py-1.5 text-sm hover:bg-accent"
                    onClick={() => {
                      onPublish(dashboard.id);
                      setMenuOpen(false);
                    }}
                  >
                    <Upload className="h-4 w-4" />
                    Publicar
                  </button>
                )}
                {dashboard.status === 'Published' && (
                  <button
                    className="flex w-full items-center gap-2 rounded-sm px-2 py-1.5 text-sm hover:bg-accent"
                    onClick={() => {
                      onArchive(dashboard.id);
                      setMenuOpen(false);
                    }}
                  >
                    <Archive className="h-4 w-4" />
                    Archivar
                  </button>
                )}
              </div>
            </>
          )}
        </div>
      </CardHeader>
      <CardContent>
        <p className="mb-3 line-clamp-2 text-sm text-muted-foreground">{dashboard.description}</p>
        <div className="flex items-center justify-between">
          <Badge variant={statusVariant[dashboard.status]}>
            {DASHBOARD_STATUS_LABELS[dashboard.status]}
          </Badge>
          <span className="text-xs text-muted-foreground">
            {new Date(dashboard.updatedAt).toLocaleDateString('es-ES')}
          </span>
        </div>
      </CardContent>
    </Card>
  );
}
