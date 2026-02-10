import { z } from 'zod';

export const addWidgetSchema = z.object({
  title: z
    .string()
    .min(1, 'El titulo es requerido')
    .max(100, 'El titulo no puede exceder 100 caracteres'),
  type: z.coerce.number().min(0).max(8),
  column: z.coerce.number().min(0).max(11),
  row: z.coerce.number().min(0),
  width: z.coerce.number().min(1).max(12),
  height: z.coerce.number().min(1).max(8),
  dataSourceId: z.string().uuid('Debe seleccionar un origen de datos valido'),
  refreshInterval: z.coerce.number(),
});

export type AddWidgetFormData = z.infer<typeof addWidgetSchema>;
