import { apiClient } from "@/lib/api-client";
import { AuthUser, ApiLoginDto, ApiRegisterDto } from "./types";

export const AUTH_API_ROUTES = {
  LOGIN: "/auth/authenticate",
  LOGOUT: "/auth/logout",
  REFRESH: "/auth/refresh",
  REGISTER: "/auth/register",
  ME: "/auth/me",
  FORGOT_PASSWORD: "/auth/forgot-password",
  RESET_PASSWORD: "/auth/reset-password",
};

export const authEndpoints = {
  authenticate: async (data: ApiLoginDto): Promise<void> => {
    await apiClient.post<void, ApiLoginDto>(AUTH_API_ROUTES.LOGIN, data);
  },
  register: async (data: ApiRegisterDto): Promise<AuthUser> => {
    const response = await apiClient.post<AuthUser, ApiRegisterDto>(
      AUTH_API_ROUTES.REGISTER,
      data
    );
    return response.data;
  },
  me: async (): Promise<AuthUser> => {
    const response = await apiClient.get<AuthUser>(AUTH_API_ROUTES.ME);
    return response.data;
  },
};
