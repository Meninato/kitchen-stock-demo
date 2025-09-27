"use client";

import { parseAsString, useQueryState } from "nuqs";
import { paramKeys } from "./params-keys";

export interface KitchenQsParam {
  [paramKeys.KITCHEN_ID]: string;
}

export function useKitchenQs() {
  return useQueryState(
    paramKeys.KITCHEN_ID,
    parseAsString.withDefault("").withOptions({ clearOnDefault: false })
  );
}
