import { z } from "zod";

export interface Kitchen {
  id: string;
  name: string;
  description: string;
  ingredient_count: number;
  recipe_count: number;
  low_stock_items: number;
  create_at: string;
}
