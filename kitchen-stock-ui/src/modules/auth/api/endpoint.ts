import { apiClient } from "@/lib/api-client";
import { LoginDto } from "./types";

const resource = "/auth";

export const authEndpoints = {
  authenticate: async (data: LoginDto): Promise<void> => {
    await apiClient.post<void, LoginDto>(`${resource}/authenticate`, data);
  },
};
