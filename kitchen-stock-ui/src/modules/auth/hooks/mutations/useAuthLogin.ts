import { useMutation, UseMutationOptions } from "@tanstack/react-query";

import { ApiError } from "@/lib/api-client";
import { authEndpoints } from "@/modules/auth/api/endpoint";
import { LoginDto } from "../../api/types";

export const useAuthLogin = (
  options?: Omit<UseMutationOptions<void, ApiError, LoginDto>, "mutationFn">
) => {
  return useMutation({
    mutationFn: authEndpoints.authenticate,
    onSuccess: async (data, variables, onMutateResult, context) => {
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
