"use server";

import { serverTokenStorage } from "@/lib/server/token-storage";

export async function clearTokensAction() {
  await serverTokenStorage.destroyTokens();
}
