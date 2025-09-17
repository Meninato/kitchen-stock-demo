import { useQuery, UseQueryOptions } from "@tanstack/react-query";

import { unitOfMeasureEndpoints } from "@/modules/unit-of-measure/api/endpoint";
import { UnitOfMeasure } from "@/modules/unit-of-measure/api/types";
import { unitOfMeasureKeys } from "./query-keys";

export const useUnitOfMeasure = (
  id: string,
  options?: Omit<UseQueryOptions<UnitOfMeasure>, "queryKey" | "queryFn">
) => {
  return useQuery({
    queryKey: unitOfMeasureKeys.detail(id),
    queryFn: () => unitOfMeasureEndpoints.getUnitOfMeasure(id),
    enabled: !!id,
    staleTime: 5 * 60 * 1000,
    gcTime: 10 * 60 * 1000,
    ...options,
  });
};
