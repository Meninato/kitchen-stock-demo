import { useQuery, UseQueryOptions } from "@tanstack/react-query";

import { unitOfMeasureEndpoints } from "@/modules/unit-of-measure/api/endpoint";
import { UnitOfMeasure } from "@/modules/unit-of-measure/api/types";
import { uomQueryKeys } from "./uom-query-keys";

export const useUnitsOfMeasure = (
  options?: Omit<UseQueryOptions<UnitOfMeasure[]>, "queryKey" | "queryFn">
) => {
  return useQuery({
    queryKey: uomQueryKeys.lists(),
    queryFn: () => unitOfMeasureEndpoints.getUnitOfMeasures(),
    staleTime: 5 * 60 * 1000, // 5 minutes
    gcTime: 10 * 60 * 1000, // 10 minutes (formerly cacheTime)
    ...options,
  });
};
