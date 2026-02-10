import { useState, useCallback } from 'react';
import { useParams } from 'react-router';
import { Plus, LayoutGrid } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { EmptyState } from '@/components/shared/empty-state';
import { ErrorFallback } from '@/components/shared/error-fallback';
import { ErrorBoundary } from '@/components/shared/error-boundary';
import { PageTransition } from '@/components/shared/page-transition';
import { DashboardDetailSkeleton } from '@/components/skeletons/dashboard-detail-skeleton';
import { useDashboard } from '@/hooks/queries/use-dashboards';
import { useRemoveWidget } from '@/hooks/mutations/use-remove-widget';
import { useDashboardRealtime } from '@/hooks/use-dashboard-realtime';
import { useKeyboardShortcut } from '@/hooks/use-keyboard-shortcut';
import { toast } from '@/hooks/use-toast';
import { DashboardHeader } from './dashboard-header';
import { WidgetGrid } from './widget-grid';
import { AddWidgetDialog } from './add-widget-dialog';

export function Component() {
  const { id } = useParams<{ id: string }>();
  const { data: dashboard, isLoading, isError, refetch } = useDashboard(id!);
  const removeWidget = useRemoveWidget();
  const [addWidgetOpen, setAddWidgetOpen] = useState(false);

  useDashboardRealtime(id, dashboard?.widgets);

  const openAddWidget = useCallback(() => setAddWidgetOpen(true), []);
  useKeyboardShortcut({ key: 'w', ctrl: true, handler: openAddWidget });

  function handleRemoveWidget(widgetId: string) {
    removeWidget.mutate(
      { dashboardId: id!, widgetId },
      {
        onSuccess: () => toast({ title: 'Widget eliminado', variant: 'success' }),
        onError: (err) =>
          toast({
            title: 'Error al eliminar widget',
            description: err.message,
            variant: 'destructive',
          }),
      },
    );
  }

  if (isLoading) {
    return <DashboardDetailSkeleton />;
  }

  if (isError || !dashboard) {
    return <ErrorFallback message="Error al cargar el dashboard" onRetry={() => refetch()} />;
  }

  return (
    <PageTransition>
      <div className="space-y-6">
        <DashboardHeader dashboard={dashboard} />

        <div className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
          <p className="text-sm text-muted-foreground">{dashboard.description}</p>
          <Button size="sm" onClick={openAddWidget} className="shrink-0">
            <Plus className="h-4 w-4" />
            Agregar widget
          </Button>
        </div>

        {dashboard.widgets.length === 0 ? (
          <EmptyState
            icon={LayoutGrid}
            title="Sin widgets"
            description="Agrega widgets para visualizar datos en este dashboard"
            action={
              <Button onClick={openAddWidget}>
                <Plus className="h-4 w-4" />
                Agregar widget
              </Button>
            }
          />
        ) : (
          <ErrorBoundary>
            <WidgetGrid widgets={dashboard.widgets} onRemoveWidget={handleRemoveWidget} />
          </ErrorBoundary>
        )}

        <AddWidgetDialog
          open={addWidgetOpen}
          onOpenChange={setAddWidgetOpen}
          dashboardId={id!}
          existingWidgets={dashboard.widgets}
        />
      </div>
    </PageTransition>
  );
}
