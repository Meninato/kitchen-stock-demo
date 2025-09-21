export const uomQueryKeys = {
  all: ["units-of-measure"] as const,
  lists: () => [...uomQueryKeys.all, "list"] as const,
  details: () => [...uomQueryKeys.all, "detail"] as const,
  detail: (id: string) => [...uomQueryKeys.details(), id] as const,
};
