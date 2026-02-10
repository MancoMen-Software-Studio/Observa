import { useState, useCallback } from 'react';
import { Database, Plus, CircleDot } from 'lucide-react';
import { PageHeader } from '@/components/shared/page-header';
import { EmptyState } from '@/components/shared/empty-state';
import { ErrorFallback } from '@/components/shared/error-fallback';
import { PageTransition } from '@/components/shared/page-transition';
import { Skeleton } from '@/components/ui/skeleton';
import { Button } from '@/components/ui/button';
import { useDataSources } from '@/hooks/queries/use-data-sources';
import { useKeyboardShortcut } from '@/hooks/use-keyboard-shortcut';
import { DATA_SOURCE_TYPE_LABELS } from '@/types/enums';
import type { DataSourceResponse, DataSourceType } from '@/types/api';
import { CreateDataSourceDialog } from './create-data-source-dialog';

export function Component() {
  const { data: dataSources, isLoading, isError, refetch } = useDataSources();
  const [createOpen, setCreateOpen] = useState(false);

  const openCreate = useCallback(() => setCreateOpen(true), []);
  useKeyboardShortcut({ key: 'n', ctrl: true, handler: openCreate });

  if (isLoading) {
    return (
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <div className="space-y-2">
            <Skeleton className="h-8 w-48" />
            <Skeleton className="h-4 w-72" />
          </div>
          <Skeleton className="h-9 w-48" />
        </div>
        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {Array.from({ length: 6 }).map((_, i) => (
            <Skeleton key={i} className="h-32 rounded-xl" />
          ))}
        </div>
      </div>
    );
  }

  if (isError) {
    return <ErrorFallback message="Error al cargar origenes de datos" onRetry={() => refetch()} />;
  }

  return (
    <PageTransition>
      <div className="space-y-6">
        <PageHeader
          title="Origenes de datos"
          description="Gestiona los origenes de datos de tus widgets"
          action={
            <Button onClick={openCreate}>
              <Plus className="h-4 w-4" />
              <span className="hidden sm:inline">Crear origen de datos</span>
            </Button>
          }
        />

        {!dataSources || dataSources.length === 0 ? (
          <EmptyState
            icon={Database}
            title="Sin origenes de datos"
            description="Crea un origen de datos para conectar tus widgets"
            action={
              <Button onClick={openCreate}>
                <Plus className="h-4 w-4" />
                Crear origen de datos
              </Button>
            }
          />
        ) : (
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {dataSources.map((ds) => (
              <DataSourceCard key={ds.id} dataSource={ds} />
            ))}
          </div>
        )}

        <CreateDataSourceDialog open={createOpen} onOpenChange={setCreateOpen} />
      </div>
    </PageTransition>
  );
}

function DataSourceCard({ dataSource }: { dataSource: DataSourceResponse }) {
  const typeLabel = DATA_SOURCE_TYPE_LABELS[dataSource.type as DataSourceType] ?? dataSource.type;
  const createdDate = new Date(dataSource.createdAt).toLocaleDateString('es-ES', {
    day: 'numeric',
    month: 'short',
    year: 'numeric',
  });

  return (
    <div className="rounded-xl border border-border bg-card p-5 space-y-3">
      <div className="flex items-start justify-between">
        <div className="flex items-center gap-3">
          <div className="rounded-lg bg-primary/10 p-2">
            <Database className="h-4 w-4 text-primary" />
          </div>
          <div>
            <h3 className="font-semibold text-sm leading-none">{dataSource.name}</h3>
            <p className="text-xs text-muted-foreground mt-1">{typeLabel}</p>
          </div>
        </div>
        <div className="flex items-center gap-1.5">
          <CircleDot className={`h-3 w-3 ${dataSource.isActive ? 'text-green-500' : 'text-muted-foreground'}`} />
          <span className="text-xs text-muted-foreground">
            {dataSource.isActive ? 'Activo' : 'Inactivo'}
          </span>
        </div>
      </div>
      <div className="text-xs text-muted-foreground">
        Creado el {createdDate}
      </div>
    </div>
  );
}
