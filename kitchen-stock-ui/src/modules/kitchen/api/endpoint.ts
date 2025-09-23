import { apiClient } from "@/lib/api-client";
import { ApiCreateKitchenDto, Kitchen } from "./types";

export const KITCHEN_API_ROUTES = {
  KITCHENS: "/kitchens",
};

export const kitchenEndpoints = {
  getKitchens: async (): Promise<Kitchen[]> => {
    const response = await apiClient.get<Kitchen[]>(
      KITCHEN_API_ROUTES.KITCHENS
    );
    return response.data;
  },
  createKitchen: async (data: ApiCreateKitchenDto): Promise<Kitchen> => {
    const response = await apiClient.post<Kitchen, ApiCreateKitchenDto>(
      KITCHEN_API_ROUTES.KITCHENS,
      data
    );
    return response.data;
  },
};
