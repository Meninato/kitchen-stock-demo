import { useMutation, UseMutationOptions } from "@tanstack/react-query";

import { ApiError } from "@/lib/api-client";
import { authEndpoints } from "@/modules/auth/api/endpoint";
import { AuthUser, ApiRegisterDto } from "../../api/types";

export const useAuthRegister = (
  options?: Omit<
    UseMutationOptions<AuthUser, ApiError, ApiRegisterDto>,
    "mutationFn"
  >
) => {
  return useMutation({
    mutationFn: authEndpoints.register,
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
