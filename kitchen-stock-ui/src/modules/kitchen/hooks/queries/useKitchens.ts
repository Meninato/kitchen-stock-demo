import { useQuery, UseQueryOptions } from "@tanstack/react-query";

import { kitchenEndpoints } from "@/modules/kitchen/api/endpoint";
import { Kitchen } from "@/modules/kitchen/api/types";
import { kitchenQueryKeys } from "./kitchen-query-keys";

export const useKitchens = (
  options?: Omit<UseQueryOptions<Kitchen[]>, "queryKey" | "queryFn">
) => {
  return useQuery({
    queryKey: kitchenQueryKeys.lists(),
    queryFn: () => kitchenEndpoints.getKitchens(),
    staleTime: 5 * 60 * 1000, // 5 minutes
    gcTime: 10 * 60 * 1000, // 10 minutes (formerly cacheTime)
    ...options,
  });
};
