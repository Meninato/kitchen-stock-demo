import { useMutation, UseMutationOptions } from "@tanstack/react-query";

import { ApiError } from "@/lib/api-client";
import { authEndpoints } from "@/modules/auth/api/endpoint";

export const useAuthLogout = (
  options?: Omit<UseMutationOptions<void, ApiError>, "mutationFn">
) => {
  return useMutation({
    mutationFn: authEndpoints.logout,
    ...options,
  });
};
