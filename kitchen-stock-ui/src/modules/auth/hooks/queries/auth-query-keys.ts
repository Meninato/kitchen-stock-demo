const authBuilderQueryKeys = {
  all: ["auth"] as const,
  me: () => [...authBuilderQueryKeys.all, "me"] as const,
};

export const authQueryKeys = {
  me: () => authBuilderQueryKeys.me(),
};
