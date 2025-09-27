const kitchenBuilderQueryKeys = {
  all: ["kitchens"] as const,
  lists: () => [...kitchenBuilderQueryKeys.all, "list"] as const,
  details: () => [...kitchenBuilderQueryKeys.all, "detail"] as const,
  detail: (id: string) => [...kitchenBuilderQueryKeys.details(), id] as const,
};

export const kitchenQueryKeys = {
  lists: () => kitchenBuilderQueryKeys.lists(),
  detail: (id: string) => kitchenBuilderQueryKeys.detail(id),
};
