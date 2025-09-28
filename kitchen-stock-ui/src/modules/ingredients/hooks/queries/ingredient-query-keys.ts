import { GetIngredientsParams } from "../../api/endpoint";

const ingredientBuilderQueryKeys = {
  all: ["ingredients"] as const,
  byKitchen: (kitchenId: string) =>
    [...ingredientBuilderQueryKeys.all, "kitchen", kitchenId] as const,
  byKitchenLists: (kitchenId: string) =>
    [...ingredientBuilderQueryKeys.byKitchen(kitchenId), "list"] as const,
  byKitchenWithParams: (params: GetIngredientsParams) => [
    ...ingredientBuilderQueryKeys.byKitchen(params.kitchenId),
    { ...params["config"] },
  ],
};

export const ingredientQueryKeys = {
  byKitchenWithParams: (ingredientParams: GetIngredientsParams) =>
    ingredientBuilderQueryKeys.byKitchenWithParams(ingredientParams),
};
