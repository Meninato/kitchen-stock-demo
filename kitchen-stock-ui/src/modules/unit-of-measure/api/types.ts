import { z } from "zod";

export interface UnitOfMeasure {
  id: string;
  name: string;
  symbol: string;
  ingredientCount: number;
  isSystemUnit: boolean;
}

export const createUnitOfMeasureSchema = z.object({
  name: z.string().min(1).max(50),
  symbol: z.string().min(1).max(10),
});

export const updateUnitOfMeasureSchema = createUnitOfMeasureSchema;

export type CreateUnitOfMeasureDto = z.infer<typeof createUnitOfMeasureSchema>;
export type UpdateUnitOfMeasureDto = z.infer<typeof updateUnitOfMeasureSchema>;
