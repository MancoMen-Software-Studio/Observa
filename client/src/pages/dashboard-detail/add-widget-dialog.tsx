import { useEffect, useMemo } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { useAddWidget } from '@/hooks/mutations/use-add-widget';
import { addWidgetSchema, type AddWidgetFormData } from '@/schemas/widget';
import { WIDGET_TYPE_LABELS, REFRESH_INTERVAL_LABELS, REFRESH_INTERVAL_VALUES } from '@/types/enums';
import type { WidgetType, RefreshInterval, WidgetResponse } from '@/types/api';
import { toast } from '@/hooks/use-toast';

interface AddWidgetDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  dashboardId: string;
  existingWidgets: WidgetResponse[];
}

const GRID_COLS = 12;
const DEFAULT_WIDTH = 4;
const DEFAULT_HEIGHT = 3;

const widgetTypeEntries = Object.entries(WIDGET_TYPE_LABELS) as [WidgetType, string][];
const refreshIntervalEntries = Object.entries(REFRESH_INTERVAL_LABELS) as [RefreshInterval, string][];

function findNextPosition(widgets: WidgetResponse[], width: number, height: number) {
  if (widgets.length === 0) {
    return { column: 0, row: 0 };
  }

  const maxRow = Math.max(...widgets.map((w) => w.row + w.height));
  const grid: boolean[][] = [];
  for (let r = 0; r <= maxRow + height; r++) {
    grid[r] = new Array<boolean>(GRID_COLS).fill(false);
  }

  for (const w of widgets) {
    for (let r = w.row; r < w.row + w.height; r++) {
      for (let c = w.column; c < w.column + w.width; c++) {
        if (grid[r]) {
          grid[r][c] = true;
        }
      }
    }
  }

  for (let r = 0; r <= maxRow + height; r++) {
    for (let c = 0; c <= GRID_COLS - width; c++) {
      let fits = true;
      for (let dr = 0; dr < height && fits; dr++) {
        for (let dc = 0; dc < width && fits; dc++) {
          if (grid[r + dr]?.[c + dc]) {
            fits = false;
          }
        }
      }
      if (fits) {
        return { column: c, row: r };
      }
    }
  }

  return { column: 0, row: maxRow };
}

export function AddWidgetDialog({ open, onOpenChange, dashboardId, existingWidgets }: AddWidgetDialogProps) {
  const addWidget = useAddWidget();

  const nextPos = useMemo(
    () => findNextPosition(existingWidgets, DEFAULT_WIDTH, DEFAULT_HEIGHT),
    [existingWidgets],
  );

  const {
    register,
    handleSubmit,
    reset,
    setValue,
    formState: { errors },
  } = useForm<AddWidgetFormData>({
    resolver: zodResolver(addWidgetSchema),
    defaultValues: {
      title: '',
      type: 0,
      column: nextPos.column,
      row: nextPos.row,
      width: DEFAULT_WIDTH,
      height: DEFAULT_HEIGHT,
      dataSourceId: '',
      refreshInterval: 30,
    },
  });

  useEffect(() => {
    if (open) {
      const pos = findNextPosition(existingWidgets, DEFAULT_WIDTH, DEFAULT_HEIGHT);
      setValue('column', pos.column);
      setValue('row', pos.row);
    }
  }, [open, existingWidgets, setValue]);

  function onSubmit(data: AddWidgetFormData) {
    addWidget.mutate(
      { dashboardId, ...data },
      {
        onSuccess: () => {
          toast({ title: 'Widget agregado', variant: 'success' });
          reset();
          onOpenChange(false);
        },
        onError: (error) => {
          toast({
            title: 'Error al agregar widget',
            description: error.message,
            variant: 'destructive',
          });
        },
      },
    );
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-md">
        <DialogHeader>
          <DialogTitle>Agregar widget</DialogTitle>
          <DialogDescription>Configura el nuevo widget para el dashboard.</DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="widget-title">Titulo</Label>
            <Input id="widget-title" placeholder="Mi widget" {...register('title')} />
            {errors.title && <p className="text-xs text-destructive">{errors.title.message}</p>}
          </div>

          <div className="space-y-2">
            <Label>Tipo</Label>
            <Select
              defaultValue="0"
              onValueChange={(v) => setValue('type', Number(v))}
            >
              <SelectTrigger>
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                {widgetTypeEntries.map(([key, label], i) => (
                  <SelectItem key={key} value={String(i)}>
                    {label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          <div className="grid grid-cols-2 gap-3">
            <div className="space-y-2">
              <Label htmlFor="widget-col">Columna</Label>
              <Input id="widget-col" type="number" min="0" max="11" {...register('column')} />
              {errors.column && <p className="text-xs text-destructive">{errors.column.message}</p>}
            </div>
            <div className="space-y-2">
              <Label htmlFor="widget-row">Fila</Label>
              <Input id="widget-row" type="number" min="0" {...register('row')} />
              {errors.row && <p className="text-xs text-destructive">{errors.row.message}</p>}
            </div>
            <div className="space-y-2">
              <Label htmlFor="widget-width">Ancho</Label>
              <Input id="widget-width" type="number" min="1" max="12" {...register('width')} />
              {errors.width && <p className="text-xs text-destructive">{errors.width.message}</p>}
            </div>
            <div className="space-y-2">
              <Label htmlFor="widget-height">Alto</Label>
              <Input id="widget-height" type="number" min="1" max="8" {...register('height')} />
              {errors.height && <p className="text-xs text-destructive">{errors.height.message}</p>}
            </div>
          </div>

          <div className="space-y-2">
            <Label htmlFor="widget-datasource">ID del origen de datos</Label>
            <Input
              id="widget-datasource"
              placeholder="UUID del data source"
              {...register('dataSourceId')}
            />
            {errors.dataSourceId && (
              <p className="text-xs text-destructive">{errors.dataSourceId.message}</p>
            )}
          </div>

          <div className="space-y-2">
            <Label>Intervalo de actualizacion</Label>
            <Select
              defaultValue="30"
              onValueChange={(v) => setValue('refreshInterval', Number(v))}
            >
              <SelectTrigger>
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                {refreshIntervalEntries.map(([key, label]) => (
                  <SelectItem key={key} value={String(REFRESH_INTERVAL_VALUES[key])}>
                    {label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          <DialogFooter>
            <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
              Cancelar
            </Button>
            <Button type="submit" disabled={addWidget.isPending}>
              {addWidget.isPending ? 'Agregando...' : 'Agregar'}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
