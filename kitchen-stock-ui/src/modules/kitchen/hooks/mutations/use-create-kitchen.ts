import {
  useMutation,
  UseMutationOptions,
  useQueryClient,
} from "@tanstack/react-query";

import { ApiError } from "@/lib/api-client";
import { kitchenEndpoints } from "@/modules/kitchen/api/endpoint";
import { ApiCreateKitchenDto, Kitchen } from "@/modules/kitchen/api/types";
import { kitchenQueryKeys } from "../queries/kitchen-query-keys";

export const useCreateKitchen = (
  options?: Omit<
    UseMutationOptions<Kitchen, ApiError, ApiCreateKitchenDto>,
    "mutationFn"
  >
) => {
  const queryClient = useQueryClient();

  const { onSuccess: userOnSuccess, ...restOptions } = options || {};

  return useMutation({
    mutationFn: kitchenEndpoints.createKitchen,
    onSuccess: async (data, variables, onMutateResult, context) => {
      // Invalidate and refetch kitchen list
      await queryClient.invalidateQueries({
        queryKey: kitchenQueryKeys.lists(),
      });

      // Optionally add the new kitchen to cache immediately
      queryClient.setQueryData(kitchenQueryKeys.detail(data.id), data);

      await userOnSuccess?.(data, variables, onMutateResult, context);
    },
    ...restOptions,
  });
};
