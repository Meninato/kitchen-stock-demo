import {
  useMutation,
  useQueryClient,
  UseMutationOptions,
} from "@tanstack/react-query";

import {
  CreateUnitOfMeasureDto,
  UnitOfMeasure,
} from "@/modules/unit-of-measure/api/types";
import { unitOfMeasureEndpoints } from "@/modules/unit-of-measure/api/endpoint";
import { ApiError } from "@/lib/api-client";
import { unitOfMeasureKeys } from "../queries/uom-query-keys";

export const useCreateUnitOfMeasure = (
  options?: Omit<
    UseMutationOptions<UnitOfMeasure, ApiError, CreateUnitOfMeasureDto>,
    "mutationFn"
  >
) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: unitOfMeasureEndpoints.createUnitOfMeasure,
    onSuccess: async (data, variables, onMutateResult, context) => {
      // Invalidate and refetch unit of measure list
      await queryClient.invalidateQueries({
        queryKey: unitOfMeasureKeys.lists(),
      });

      // Optionally add the new unit of measure to cache immediately
      queryClient.setQueryData(unitOfMeasureKeys.detail(data.id), data);

      // Call custom onSuccess if provided
      options?.onSuccess?.(data, variables, onMutateResult, context);
    },
    onError: async (error, variables, onMudateResult, context) => {
      // Call custom onError if provided
      options?.onError?.(error, variables, onMudateResult, context);
    },
    ...options,
  });
};
