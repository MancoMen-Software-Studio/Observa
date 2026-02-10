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
import { Textarea } from '@/components/ui/textarea';
import { Label } from '@/components/ui/label';
import { useCreateDashboard } from '@/hooks/mutations/use-create-dashboard';
import { createDashboardSchema, type CreateDashboardFormData } from '@/schemas/dashboard';
import { toast } from '@/hooks/use-toast';

interface CreateDashboardDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function CreateDashboardDialog({ open, onOpenChange }: CreateDashboardDialogProps) {
  const createDashboard = useCreateDashboard();

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<CreateDashboardFormData>({
    resolver: zodResolver(createDashboardSchema),
    defaultValues: { title: '', description: '' },
  });

  function onSubmit(data: CreateDashboardFormData) {
    createDashboard.mutate(data, {
      onSuccess: () => {
        toast({ title: 'Dashboard creado', variant: 'success' });
        reset();
        onOpenChange(false);
      },
      onError: (error) => {
        toast({ title: 'Error al crear dashboard', description: error.message, variant: 'destructive' });
      },
    });
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Crear dashboard</DialogTitle>
          <DialogDescription>Configura los datos basicos del nuevo dashboard.</DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="title">Titulo</Label>
            <Input id="title" placeholder="Mi dashboard" {...register('title')} />
            {errors.title && (
              <p className="text-xs text-destructive">{errors.title.message}</p>
            )}
          </div>
          <div className="space-y-2">
            <Label htmlFor="description">Descripcion</Label>
            <Textarea
              id="description"
              placeholder="Descripcion del dashboard..."
              {...register('description')}
            />
            {errors.description && (
              <p className="text-xs text-destructive">{errors.description.message}</p>
            )}
          </div>
          <DialogFooter>
            <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
              Cancelar
            </Button>
            <Button type="submit" disabled={createDashboard.isPending}>
              {createDashboard.isPending ? 'Creando...' : 'Crear'}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
