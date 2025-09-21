import { apiClient } from "@/lib/api-client";
import { AuthUser, ApiLoginDto, ApiRegisterDto } from "./types";

const resource = "/auth";

export const authEndpoints = {
  authenticate: async (data: ApiLoginDto): Promise<void> => {
    await apiClient.post<void, ApiLoginDto>(`${resource}/authenticate`, data);
  },
  register: async (data: ApiRegisterDto): Promise<AuthUser> => {
    const response = await apiClient.post<AuthUser, ApiRegisterDto>(
      `${resource}/register`,
      data
    );
    return response.data;
  },
  me: async (): Promise<AuthUser> => {
    const response = await apiClient.post<AuthUser>(`${resource}/me`);
    return response.data;
  },
};
