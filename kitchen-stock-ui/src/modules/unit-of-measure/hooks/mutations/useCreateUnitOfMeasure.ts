import {
  useMutation,
  useQueryClient,
  UseMutationOptions,
} from "@tanstack/react-query";

import {
  CreateUnitOfMeasureDto,
  UnitOfMeasure,
} from "@/lib/api/unit-of-measure/types";
import { unitOfMeasureEndpoints } from "@/lib/api/unit-of-measure/endpoint";
import { ApiError } from "@/lib/api/client";
import { unitOfMeasureKeys } from "../queries/useUnitsOfMeasure";

export const useCreateUnitOfMeasure = (
  options?: Omit<
    UseMutationOptions<UnitOfMeasure, ApiError, CreateUnitOfMeasureDto>,
    "mutationFn"
  >
) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: unitOfMeasureEndpoints.createUnitOfMeasure,
    onSuccess: (data, variables, onMutateResult, context) => {
      // Invalidate and refetch unit of measure list
      queryClient.invalidateQueries({ queryKey: unitOfMeasureKeys.lists() });

      // Optionally add the new unit of measure to cache immediately
      queryClient.setQueryData(unitOfMeasureKeys.detail(data.id), data);

      // Call custom onSuccess if provided
      options?.onSuccess?.(data, variables, onMutateResult, context);
    },
    onError: (error, variables, onMudateResult, context) => {
      // Call custom onError if provided
      options?.onError?.(error, variables, onMudateResult, context);
    },
    ...options,
  });
};
