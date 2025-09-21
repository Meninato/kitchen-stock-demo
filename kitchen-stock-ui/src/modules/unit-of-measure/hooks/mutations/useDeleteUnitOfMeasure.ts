import {
  useMutation,
  useQueryClient,
  UseMutationOptions,
} from "@tanstack/react-query";

import {} from "@/modules/unit-of-measure/api/types";
import { unitOfMeasureEndpoints } from "@/modules/unit-of-measure/api/endpoint";
import { ApiError } from "@/lib/api-client";
import { unitOfMeasureKeys } from "../queries/uom-query-keys";

export const useDeleteUnitOfMeasure = (
  options?: Omit<UseMutationOptions<void, ApiError, string>, "mutationFn">
) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: unitOfMeasureEndpoints.deleteProduct,
    onSuccess: async (data, variables, onMutateResult, context) => {
      await queryClient.invalidateQueries({
        queryKey: unitOfMeasureKeys.lists(),
      });
      queryClient.removeQueries({
        queryKey: unitOfMeasureKeys.detail(variables),
      });

      options?.onSuccess?.(data, variables, onMutateResult, context);
    },
    onError: (data, variables, onMutateResult, context) => {
      options?.onError?.(data, variables, onMutateResult, context);
    },
    ...options,
  });
};
