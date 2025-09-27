import { apiClient } from "@/lib/api-client";
import { ApiCreateKitchenDto, ApiUpdateKitchenDto, Kitchen } from "./types";

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
  getKitchen: async (id: string): Promise<Kitchen> => {
    const response = await apiClient.get<Kitchen>(
      `${KITCHEN_API_ROUTES.KITCHENS}/${id}`
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
  updateKitchen: async (
    id: string,
    data: ApiUpdateKitchenDto
  ): Promise<Kitchen> => {
    const response = await apiClient.put<Kitchen, ApiUpdateKitchenDto>(
      `${KITCHEN_API_ROUTES.KITCHENS}/${id}`,
      data
    );
    return response.data;
  },
};
