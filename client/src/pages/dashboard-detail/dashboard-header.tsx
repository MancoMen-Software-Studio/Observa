import { useState } from 'react';
import { Check, Pencil, X, Upload, Archive } from 'lucide-react';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import type { DashboardResponse, DashboardStatus } from '@/types/api';
import { DASHBOARD_STATUS_LABELS } from '@/types/enums';
import { useUpdateDashboardTitle } from '@/hooks/mutations/use-update-dashboard-title';
import { usePublishDashboard } from '@/hooks/mutations/use-publish-dashboard';
import { useArchiveDashboard } from '@/hooks/mutations/use-archive-dashboard';
import { toast } from '@/hooks/use-toast';

interface DashboardHeaderProps {
  dashboard: DashboardResponse;
}

const statusVariant: Record<DashboardStatus, 'default' | 'success' | 'secondary'> = {
  Draft: 'secondary',
  Published: 'success',
  Archived: 'default',
};

export function DashboardHeader({ dashboard }: DashboardHeaderProps) {
  const [isEditing, setIsEditing] = useState(false);
  const [editTitle, setEditTitle] = useState(dashboard.title);

  const updateTitle = useUpdateDashboardTitle();
  const publishDashboard = usePublishDashboard();
  const archiveDashboard = useArchiveDashboard();

  function handleSaveTitle() {
    if (!editTitle.trim() || editTitle === dashboard.title) {
      setIsEditing(false);
      return;
    }
    updateTitle.mutate(
      { id: dashboard.id, newTitle: editTitle },
      {
        onSuccess: () => {
          setIsEditing(false);
          toast({ title: 'Titulo actualizado', variant: 'success' });
        },
        onError: (err) => {
          toast({
            title: 'Error al actualizar titulo',
            description: err.message,
            variant: 'destructive',
          });
        },
      },
    );
  }

  function handlePublish() {
    publishDashboard.mutate(dashboard.id, {
      onSuccess: () => toast({ title: 'Dashboard publicado', variant: 'success' }),
      onError: (err) =>
        toast({ title: 'Error al publicar', description: err.message, variant: 'destructive' }),
    });
  }

  function handleArchive() {
    archiveDashboard.mutate(dashboard.id, {
      onSuccess: () => toast({ title: 'Dashboard archivado', variant: 'success' }),
      onError: (err) =>
        toast({ title: 'Error al archivar', description: err.message, variant: 'destructive' }),
    });
  }

  return (
    <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <div className="flex items-center gap-3">
        {isEditing ? (
          <div className="flex items-center gap-2">
            <Input
              value={editTitle}
              onChange={(e) => setEditTitle(e.target.value)}
              className="h-9 w-64"
              autoFocus
              onKeyDown={(e) => {
                if (e.key === 'Enter') {
                  handleSaveTitle();
                }
                if (e.key === 'Escape') {
                  setIsEditing(false);
                  setEditTitle(dashboard.title);
                }
              }}
            />
            <Button size="icon" variant="ghost" className="h-8 w-8" onClick={handleSaveTitle}>
              <Check className="h-4 w-4" />
            </Button>
            <Button
              size="icon"
              variant="ghost"
              className="h-8 w-8"
              onClick={() => {
                setIsEditing(false);
                setEditTitle(dashboard.title);
              }}
            >
              <X className="h-4 w-4" />
            </Button>
          </div>
        ) : (
          <>
            <h1 className="text-2xl font-bold tracking-tight">{dashboard.title}</h1>
            <Button
              size="icon"
              variant="ghost"
              className="h-8 w-8 text-muted-foreground"
              onClick={() => setIsEditing(true)}
            >
              <Pencil className="h-4 w-4" />
            </Button>
          </>
        )}
        <Badge variant={statusVariant[dashboard.status]}>
          {DASHBOARD_STATUS_LABELS[dashboard.status]}
        </Badge>
      </div>

      <div className="flex gap-2">
        {dashboard.status === 'Draft' && (
          <Button size="sm" onClick={handlePublish} disabled={publishDashboard.isPending}>
            <Upload className="h-4 w-4" />
            Publicar
          </Button>
        )}
        {dashboard.status === 'Published' && (
          <Button
            size="sm"
            variant="outline"
            onClick={handleArchive}
            disabled={archiveDashboard.isPending}
          >
            <Archive className="h-4 w-4" />
            Archivar
          </Button>
        )}
      </div>
    </div>
  );
}
