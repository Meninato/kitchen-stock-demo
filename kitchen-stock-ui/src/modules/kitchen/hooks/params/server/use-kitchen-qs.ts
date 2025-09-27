import { createLoader, parseAsString } from "nuqs/server";
import { paramKeys } from "../params-keys";

export const kitchenSearchParams = {
  [paramKeys.KITCHEN_ID]: parseAsString.withDefault(""),
};
export const loadKitchenSearchParams = createLoader(kitchenSearchParams);
