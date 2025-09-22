import { apiClient } from "@/lib/api-client";
import { Kitchen } from "./types";

export const KITCHEN_API_ROUTES = {
  USER_KITCHENS: "/kitchens",
};

export const kitchenEndpoints = {
  getKitchens: async (): Promise<Kitchen[]> => {
    const response = await apiClient.get<Kitchen[]>(
      KITCHEN_API_ROUTES.USER_KITCHENS
    );
    return response.data;
  },
};
