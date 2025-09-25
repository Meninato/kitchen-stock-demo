import { useQuery, UseQueryOptions } from "@tanstack/react-query";

import { ingredientEndpoints } from "@/modules/ingredients/api/endpoint";
import { Ingredient } from "@/modules/ingredients/api/types";
import { ingredientQueryKeys } from "./ingredient-query-keys";

export const useIngredients = (
  kitchenId: string,
  options?: Omit<UseQueryOptions<Ingredient[]>, "queryKey" | "queryFn">
) => {
  return useQuery({
    queryKey: ingredientQueryKeys.bykitchen(kitchenId),
    queryFn: () => ingredientEndpoints.getIngredients({ kitchenId }),
    staleTime: 5 * 60 * 1000, // 5 minutes
    gcTime: 10 * 60 * 1000, // 10 minutes (formerly cacheTime)
    ...options,
  });
};
