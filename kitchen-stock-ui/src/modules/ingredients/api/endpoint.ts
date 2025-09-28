import {
  apiClient,
  ApiPaginationRequest,
  WithPagination,
} from "@/lib/api-client";
import { Ingredient } from "./types";

export interface IngredientFilters {
  searchTerm?: string;
  lowStockOnly?: boolean;
}

export const INGREDIENTS_API_ROUTES = {
  INGREDIENTS: "/ingredients",
};

export interface GetIngredientsParams {
  kitchenId: string;
  config?: {
    pagination?: ApiPaginationRequest;
    filter?: IngredientFilters;
  };
}

export const ingredientEndpoints = {
  getIngredients: async (
    data: GetIngredientsParams
  ): Promise<WithPagination<Ingredient[]>> => {
    const params: GetIngredientsParams = {
      kitchenId: data.kitchenId,
      ...(data.config?.pagination ?? {}),
      ...(data.config?.filter ?? {}),
    };

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

    if (!response.pagination)
      throw new Error("Missing pagination for ingredients");

    return {
      data: response.data,
      pagination: response.pagination,
    };
  },
};
