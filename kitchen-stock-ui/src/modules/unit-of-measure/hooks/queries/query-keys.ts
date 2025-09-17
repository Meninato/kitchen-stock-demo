export const unitOfMeasureKeys = {
  all: ["units-of-measure"] as const,
  lists: () => [...unitOfMeasureKeys.all, "list"] as const,
  details: () => [...unitOfMeasureKeys.all, "detail"] as const,
  detail: (id: string) => [...unitOfMeasureKeys.details(), id] as const,
};
