import { z } from "zod";

export interface Ingredient {
  id: string;
  name: string;
  description: string;
  unitOfMeasure: string;
  unitSymbol: string;
  currentStock: number;
  minimumStock: number;
  isLowStock: boolean;
  lastUnitPrice: number;
  averageUnitPrice: number;
  createdAt: string;
}
