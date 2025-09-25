const ingredientBuilderQueryKeys = {
  all: ["ingredients"] as const,
  byKitchen: (kitchenId: string) =>
    [...ingredientBuilderQueryKeys.all, "kitchen", kitchenId] as const,
  byKitchenLists: (kitchenId: string) =>
    [...ingredientBuilderQueryKeys.byKitchen(kitchenId), "list"] as const,
};

export const ingredientQueryKeys = {
  bykitchen: (kitchenId: string) => [
    ingredientBuilderQueryKeys.byKitchenLists(kitchenId),
  ],
};
