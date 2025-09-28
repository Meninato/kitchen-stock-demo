import { useQuery, UseQueryOptions } from "@tanstack/react-query";

import {
  GetIngredientsParams,
  ingredientEndpoints,
} from "@/modules/ingredients/api/endpoint";
import { Ingredient } from "@/modules/ingredients/api/types";
import { ingredientQueryKeys } from "./ingredient-query-keys";
import { WithPagination } from "@/lib/api-client";

export const useIngredients = (
  ingredientParams: GetIngredientsParams,
  options?: Omit<
    UseQueryOptions<WithPagination<Ingredient[]>>,
    "queryKey" | "queryFn"
  >
) => {
  return useQuery({
    queryKey: ingredientQueryKeys.byKitchenWithParams(ingredientParams),
    queryFn: () => ingredientEndpoints.getIngredients(ingredientParams),
    staleTime: 5 * 60 * 1000, // 5 minutes
    gcTime: 10 * 60 * 1000, // 10 minutes (formerly cacheTime)
    ...options,
  });
};
