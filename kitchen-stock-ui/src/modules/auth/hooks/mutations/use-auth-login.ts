import { useMutation, UseMutationOptions } from "@tanstack/react-query";

import { ApiError } from "@/lib/api-client";
import { authEndpoints } from "@/modules/auth/api/endpoint";
import { ApiLoginDto } from "../../api/types";

export const useAuthLogin = (
  options?: Omit<UseMutationOptions<void, ApiError, ApiLoginDto>, "mutationFn">
) => {
  return useMutation({
    mutationFn: authEndpoints.authenticate,
    ...options,
  });
};
