import { apiClient } from "@/lib/api-client";
import {
  CreateUnitOfMeasureDto,
  UnitOfMeasure,
  UpdateUnitOfMeasureDto,
} from "./types";

const resource = "/units-of-measure";

export const unitOfMeasureEndpoints = {
  getUnitOfMeasures: async (): Promise<UnitOfMeasure[]> => {
    const response = await apiClient.get<UnitOfMeasure[]>(resource);
    return response.data;
  },

  getUnitOfMeasure: async (id: string): Promise<UnitOfMeasure> => {
    const response = await apiClient.get<UnitOfMeasure>(`${resource}/${id}`);
    return response.data;
  },

  createUnitOfMeasure: async (
    data: CreateUnitOfMeasureDto
  ): Promise<UnitOfMeasure> => {
    const response = await apiClient.post<
      UnitOfMeasure,
      CreateUnitOfMeasureDto
    >(resource, data);
    return response.data;
  },

  updateUnitOfMeasure: async (
    id: string,
    data: UpdateUnitOfMeasureDto
  ): Promise<UnitOfMeasure> => {
    const response = await apiClient.put<UnitOfMeasure, UpdateUnitOfMeasureDto>(
      `${resource}/${id}`,
      data
    );
    return response.data;
  },

  deleteProduct: async (id: string): Promise<void> => {
    await apiClient.delete(`${resource}/${id}`);
  },
};
