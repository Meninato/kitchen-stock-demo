import { apiClient, ApiPaginationRequest } from "@/lib/api-client";
import { Ingredient } from "./types";

export interface IngredientFilters {
  searchTerm?: string;
  lowStockOnly?: boolean;
}

export const INGREDIENTS_API_ROUTES = {
  INGREDIENTS: "/ingredients",
};

interface GetIngredientsParams {
  kitchenId: string;
  config?: {
    pagination?: ApiPaginationRequest;
    filter?: IngredientFilters;
  };
}

export const ingredientEndpoints = {
  getIngredients: async (data: GetIngredientsParams): Promise<Ingredient[]> => {
    const params: Record<string, unknown> = {};

    if (data.config?.pagination) {
      Object.assign(params, data.config.pagination);
    }

    if (data.config?.filter) {
      Object.assign(params, data.config.filter);
    }

    const response = await apiClient.get<Ingredient[]>(
      INGREDIENTS_API_ROUTES.INGREDIENTS,
      {
        params,
      }
    );
    return response.data;
  },
};
