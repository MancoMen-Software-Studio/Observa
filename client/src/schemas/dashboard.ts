import { z } from 'zod';

export const createDashboardSchema = z.object({
  title: z
    .string()
    .min(1, 'El titulo es requerido')
    .max(100, 'El titulo no puede exceder 100 caracteres'),
  description: z
    .string()
    .min(1, 'La descripcion es requerida')
    .max(500, 'La descripcion no puede exceder 500 caracteres'),
});

export type CreateDashboardFormData = z.infer<typeof createDashboardSchema>;

export const updateTitleSchema = z.object({
  newTitle: z
    .string()
    .min(1, 'El titulo es requerido')
    .max(100, 'El titulo no puede exceder 100 caracteres'),
});

export type UpdateTitleFormData = z.infer<typeof updateTitleSchema>;
