import {
  useMutation,
  UseMutationOptions,
  useQueryClient,
} from "@tanstack/react-query";

import { ApiError } from "@/lib/api-client";
import { kitchenEndpoints } from "@/modules/kitchen/api/endpoint";
import { ApiUpdateKitchenDto, Kitchen } from "@/modules/kitchen/api/types";
import { kitchenQueryKeys } from "../queries/kitchen-query-keys";

export const useUpdateKitchen = (
  options?: Omit<
    UseMutationOptions<
      Kitchen,
      ApiError,
      { id: string; data: ApiUpdateKitchenDto },
      { previousKitchen: Kitchen | undefined }
    >,
    "mutationFn"
  >
) => {
  const queryClient = useQueryClient();

  const {
    onSuccess: userOnSuccess,
    onError: userOnError,
    ...restOptions
  } = options || {};

  return useMutation({
    mutationFn: ({ id, data }) => kitchenEndpoints.updateKitchen(id, data),
    onMutate: async ({ id, data }) => {
      // Cancel outgoing refetches
      await queryClient.cancelQueries({
        queryKey: kitchenQueryKeys.detail(id),
      });

      // Snapshot previous value
      const previousKitchen = queryClient.getQueryData<Kitchen>(
        kitchenQueryKeys.detail(id)
      );

      // Optimistically update
      if (previousKitchen) {
        queryClient.setQueryData(kitchenQueryKeys.detail(id), {
          ...previousKitchen,
          ...data,
        });
      }

      return { previousKitchen };
    },

    onSuccess: async (data, variables, onMutateResult, context) => {
      await queryClient.invalidateQueries({
        queryKey: kitchenQueryKeys.lists(),
      });
      await queryClient.invalidateQueries({
        queryKey: kitchenQueryKeys.detail(variables.id),
      });

      await userOnSuccess?.(data, variables, onMutateResult, context);
    },
    onError: async (error, variables, onMudateResult, context) => {
      if (onMudateResult?.previousKitchen) {
        queryClient.setQueryData(
          kitchenQueryKeys.detail(variables.id),
          onMudateResult.previousKitchen
        );
      }

      await userOnError?.(error, variables, onMudateResult, context);
    },
    ...restOptions,
  });
};
