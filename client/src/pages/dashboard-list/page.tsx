import { useState, useCallback } from 'react';
import { LayoutDashboard, Plus, ChevronLeft, ChevronRight } from 'lucide-react';
import { PageHeader } from '@/components/shared/page-header';
import { EmptyState } from '@/components/shared/empty-state';
import { ErrorFallback } from '@/components/shared/error-fallback';
import { PageTransition } from '@/components/shared/page-transition';
import { DashboardListSkeleton } from '@/components/skeletons/dashboard-list-skeleton';
import { Button } from '@/components/ui/button';
import { useDashboards } from '@/hooks/queries/use-dashboards';
import { usePublishDashboard } from '@/hooks/mutations/use-publish-dashboard';
import { useArchiveDashboard } from '@/hooks/mutations/use-archive-dashboard';
import { useKeyboardShortcut } from '@/hooks/use-keyboard-shortcut';
import { useDashboardListRealtime } from '@/hooks/use-dashboard-list-realtime';
import { toast } from '@/hooks/use-toast';
import { DashboardCard } from './dashboard-card';
import { DashboardFilters } from './dashboard-filters';
import { CreateDashboardDialog } from './create-dashboard-dialog';
import type { DashboardStatus } from '@/types/api';

export function Component() {
  const [createOpen, setCreateOpen] = useState(false);
  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState<DashboardStatus | 'all'>('all');
  const [page, setPage] = useState(1);
  const pageSize = 20;

  const { data, isLoading, isError, refetch } = useDashboards({
    page,
    pageSize,
    status: statusFilter !== 'all' ? statusFilter : undefined,
    search: search || undefined,
  });
  useDashboardListRealtime();
  const publishDashboard = usePublishDashboard();
  const archiveDashboard = useArchiveDashboard();

  const openCreate = useCallback(() => setCreateOpen(true), []);
  useKeyboardShortcut({ key: 'n', ctrl: true, handler: openCreate });

  function handleSearchChange(value: string) {
    setSearch(value);
    setPage(1);
  }

  function handleStatusChange(value: DashboardStatus | 'all') {
    setStatusFilter(value);
    setPage(1);
  }

  function handlePublish(id: string) {
    publishDashboard.mutate(id, {
      onSuccess: () => toast({ title: 'Dashboard publicado', variant: 'success' }),
      onError: (err) =>
        toast({ title: 'Error al publicar', description: err.message, variant: 'destructive' }),
    });
  }

  function handleArchive(id: string) {
    archiveDashboard.mutate(id, {
      onSuccess: () => toast({ title: 'Dashboard archivado', variant: 'success' }),
      onError: (err) =>
        toast({ title: 'Error al archivar', description: err.message, variant: 'destructive' }),
    });
  }

  if (isLoading) {
    return <DashboardListSkeleton />;
  }

  if (isError) {
    return <ErrorFallback message="Error al cargar dashboards" onRetry={() => refetch()} />;
  }

  const dashboards = data?.items ?? [];
  const totalCount = data?.totalCount ?? 0;
  const totalPages = data?.totalPages ?? 1;
  const hasNextPage = data?.hasNextPage ?? false;
  const hasPreviousPage = data?.hasPreviousPage ?? false;

  return (
    <PageTransition>
      <div className="space-y-6">
        <PageHeader
          title="Dashboards"
          description="Gestiona tus dashboards de monitoreo"
          action={
            <Button onClick={openCreate}>
              <Plus className="h-4 w-4" />
              <span className="hidden sm:inline">Crear dashboard</span>
            </Button>
          }
        />

        <DashboardFilters
          search={search}
          onSearchChange={handleSearchChange}
          statusFilter={statusFilter}
          onStatusFilterChange={handleStatusChange}
        />

        {dashboards.length === 0 ? (
          <EmptyState
            icon={LayoutDashboard}
            title="Sin dashboards"
            description={
              search || statusFilter !== 'all'
                ? 'No se encontraron dashboards con los filtros aplicados'
                : 'Crea tu primer dashboard para comenzar'
            }
            action={
              !search &&
              statusFilter === 'all' && (
                <Button onClick={openCreate}>
                  <Plus className="h-4 w-4" />
                  Crear dashboard
                </Button>
              )
            }
          />
        ) : (
          <>
            <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
              {dashboards.map((dashboard) => (
                <DashboardCard
                  key={dashboard.id}
                  dashboard={dashboard}
                  onPublish={handlePublish}
                  onArchive={handleArchive}
                />
              ))}
            </div>

            {totalPages > 1 && (
              <div className="flex items-center justify-between border-t border-border pt-4">
                <p className="text-sm text-muted-foreground">
                  {totalCount.toLocaleString()} dashboards en total
                </p>
                <div className="flex items-center gap-2">
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => setPage((p) => p - 1)}
                    disabled={!hasPreviousPage}
                  >
                    <ChevronLeft className="h-4 w-4" />
                    Anterior
                  </Button>
                  <span className="text-sm text-muted-foreground">
                    {page} / {totalPages}
                  </span>
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => setPage((p) => p + 1)}
                    disabled={!hasNextPage}
                  >
                    Siguiente
                    <ChevronRight className="h-4 w-4" />
                  </Button>
                </div>
              </div>
            )}
          </>
        )}

        <CreateDashboardDialog open={createOpen} onOpenChange={setCreateOpen} />
      </div>
    </PageTransition>
  );
}
