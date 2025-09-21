import {
  useMutation,
  useQueryClient,
  UseMutationOptions,
} from "@tanstack/react-query";

import {
  UnitOfMeasure,
  UpdateUnitOfMeasureDto,
} from "@/modules/unit-of-measure/api/types";
import { unitOfMeasureEndpoints } from "@/modules/unit-of-measure/api/endpoint";
import { ApiError } from "@/lib/api-client";
import { unitOfMeasureKeys } from "../queries/uom-query-keys";

export const useUpdateUnitOfMeasure = (
  options?: Omit<
    UseMutationOptions<
      UnitOfMeasure,
      ApiError,
      { id: string; data: UpdateUnitOfMeasureDto },
      { previousUom: UnitOfMeasure | undefined }
    >,
    "mutationFn"
  >
) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }) =>
      unitOfMeasureEndpoints.updateUnitOfMeasure(id, data),
    onMutate: async ({ id, data }) => {
      // Cancel outgoing refetches
      await queryClient.cancelQueries({
        queryKey: unitOfMeasureKeys.detail(id),
      });

      // Snapshot previous value
      const previousUom = queryClient.getQueryData<UnitOfMeasure>(
        unitOfMeasureKeys.detail(id)
      );

      // Optimistically update
      if (previousUom) {
        queryClient.setQueryData(unitOfMeasureKeys.detail(id), {
          ...previousUom,
          ...data,
        });
      }

      return { previousUom };
    },
    onSuccess: async (data, variables, onMutateResult, context) => {
      await queryClient.invalidateQueries({
        queryKey: unitOfMeasureKeys.lists(),
      });
      await queryClient.invalidateQueries({
        queryKey: unitOfMeasureKeys.detail(variables.id),
      });

      options?.onSuccess?.(data, variables, onMutateResult, context);
    },
    onError: (error, variables, onMudateResult, context) => {
      if (onMudateResult?.previousUom) {
        queryClient.setQueryData(
          unitOfMeasureKeys.detail(variables.id),
          onMudateResult.previousUom
        );
      }

      options?.onError?.(error, variables, onMudateResult, context);
    },
    ...options,
  });
};
