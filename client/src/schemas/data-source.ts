import { z } from 'zod';

export const createDataSourceSchema = z.object({
  name: z
    .string()
    .min(1, 'El nombre es requerido')
    .max(100, 'El nombre no puede exceder 100 caracteres'),
  type: z.coerce.number().min(0).max(4),
  connectionString: z
    .string()
    .min(1, 'La cadena de conexion es requerida')
    .max(500, 'La cadena de conexion no puede exceder 500 caracteres'),
});

export type CreateDataSourceFormData = z.infer<typeof createDataSourceSchema>;
