import { z } from "zod";

export interface Kitchen {
  id: string;
  name: string;
  description: string;
  ingredientCount: number;
  recipeCount: number;
  lowStockItems: number;
  createAt: string;
}

export const kitchenCreateSchema = z.object({
  name: z.string().min(1).max(100),
  description: z.string().max(500).optional(),
});
export const kitchenUpdateSchema = kitchenCreateSchema;

export type FormCreateKitchenDto = z.infer<typeof kitchenCreateSchema>;
export type FormUpdateKitchenDto = z.infer<typeof kitchenUpdateSchema>;

export type ApiCreateKitchenDto = FormCreateKitchenDto;
export type ApiUpdateKitchenDto = FormUpdateKitchenDto;
