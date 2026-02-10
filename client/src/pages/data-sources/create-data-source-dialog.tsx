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
import { useCreateDataSource } from '@/hooks/mutations/use-create-data-source';
import { createDataSourceSchema, type CreateDataSourceFormData } from '@/schemas/data-source';
import { DATA_SOURCE_TYPE_LABELS } from '@/types/enums';
import type { DataSourceType } from '@/types/api';
import { toast } from '@/hooks/use-toast';

interface CreateDataSourceDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

const dataSourceTypeEntries = Object.entries(DATA_SOURCE_TYPE_LABELS) as [DataSourceType, string][];

export function CreateDataSourceDialog({ open, onOpenChange }: CreateDataSourceDialogProps) {
  const createDataSource = useCreateDataSource();

  const {
    register,
    handleSubmit,
    reset,
    setValue,
    formState: { errors },
  } = useForm<CreateDataSourceFormData>({
    resolver: zodResolver(createDataSourceSchema),
    defaultValues: { name: '', type: 0, connectionString: '' },
  });

  function onSubmit(data: CreateDataSourceFormData) {
    createDataSource.mutate(data, {
      onSuccess: (result) => {
        toast({
          title: 'Origen de datos creado',
          description: `ID: ${result.id}`,
          variant: 'success',
        });
        reset();
        onOpenChange(false);
      },
      onError: (error) => {
        toast({
          title: 'Error al crear origen de datos',
          description: error.message,
          variant: 'destructive',
        });
      },
    });
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Crear origen de datos</DialogTitle>
          <DialogDescription>Configura un nuevo origen de datos para tus widgets.</DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="ds-name">Nombre</Label>
            <Input id="ds-name" placeholder="Mi data source" {...register('name')} />
            {errors.name && <p className="text-xs text-destructive">{errors.name.message}</p>}
          </div>

          <div className="space-y-2">
            <Label>Tipo</Label>
            <Select defaultValue="0" onValueChange={(v) => setValue('type', Number(v))}>
              <SelectTrigger>
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                {dataSourceTypeEntries.map(([key, label], i) => (
                  <SelectItem key={key} value={String(i)}>
                    {label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-2">
            <Label htmlFor="ds-connection">Cadena de conexion</Label>
            <Input
              id="ds-connection"
              placeholder="https://api.example.com/data"
              {...register('connectionString')}
            />
            {errors.connectionString && (
              <p className="text-xs text-destructive">{errors.connectionString.message}</p>
            )}
          </div>

          <DialogFooter>
            <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
              Cancelar
            </Button>
            <Button type="submit" disabled={createDataSource.isPending}>
              {createDataSource.isPending ? 'Creando...' : 'Crear'}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
