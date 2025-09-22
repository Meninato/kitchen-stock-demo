export const kitchenQueryKeys = {
  all: ["kitchens"] as const,
  lists: () => [...kitchenQueryKeys.all, "list"] as const,
  details: () => [...kitchenQueryKeys.all, "detail"] as const,
  detail: (id: string) => [...kitchenQueryKeys.details(), id] as const,
};
