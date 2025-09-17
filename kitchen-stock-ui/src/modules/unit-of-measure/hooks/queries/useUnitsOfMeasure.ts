import { useQuery, UseQueryOptions } from "@tanstack/react-query";

import { unitOfMeasureEndpoints } from "@/lib/api/unit-of-measure/endpoint";
import { UnitOfMeasure } from "@/lib/api/unit-of-measure/types";

// Query keys factory
export const unitOfMeasureKeys = {
  all: ["units-of-measure"] as const,
  lists: () => [...unitOfMeasureKeys.all, "list"] as const,
  details: () => [...unitOfMeasureKeys.all, "detail"] as const,
  detail: (id: string) => [...unitOfMeasureKeys.details(), id] as const,
};

export const useUnitsOfMeasure = (
  options?: Omit<UseQueryOptions<UnitOfMeasure[]>, "queryKey" | "queryFn">
) => {
  return useQuery({
    queryKey: unitOfMeasureKeys.lists(),
    queryFn: () => unitOfMeasureEndpoints.getUnitOfMeasures(),
    staleTime: 5 * 60 * 1000, // 5 minutes
    gcTime: 10 * 60 * 1000, // 10 minutes (formerly cacheTime)
    ...options,
  });
};

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
