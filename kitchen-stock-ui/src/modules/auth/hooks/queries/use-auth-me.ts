import { useQuery, UseQueryOptions } from "@tanstack/react-query";

import { AuthUser } from "@/modules/auth/api/types";
import { authEndpoints } from "@/modules/auth/api/endpoint";
import { authQueryKeys } from "./auth-query-keys";

export const useAuthMe = (
  options?: Omit<UseQueryOptions<AuthUser>, "queryKey" | "queryFn">
) => {
  return useQuery({
    queryKey: authQueryKeys.me(),
    queryFn: () => authEndpoints.me(),
    staleTime: 5 * 60 * 1000,
    gcTime: 10 * 60 * 1000,
    ...options,
  });
};
