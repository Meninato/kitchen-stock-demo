import { redirect } from "next/navigation";

import { APP_ROUTES } from "@/app-routes";
import { serverTokenStorage } from "@/lib/server/token-storage";
import { getErrorStatusCode } from "@/lib/api-client";
import { kitchenEndpoints } from "@/modules/kitchen/api/endpoint";

export async function validateAccess(
  kitchen: string | undefined,
  currentPath: string
) {
  const loginRedirectUrl = `${
    APP_ROUTES.AUTH.SIGN_IN
  }?callbackUrl=${encodeURIComponent(currentPath)}`;

  const hasToken = await serverTokenStorage.hasRefreshToken();
  if (!hasToken) {
    redirect(loginRedirectUrl);
  }

  const kitchenRedirectUrl = `${
    APP_ROUTES.APP.KITCHEN_SELECTION
  }?redirect=${encodeURIComponent(currentPath)}`;

  if (!kitchen) {
    redirect(kitchenRedirectUrl);
  }

  try {
    await kitchenEndpoints.getKitchen(kitchen);
  } catch (err) {
    const status = getErrorStatusCode(err);

    if (status === 404) redirect(kitchenRedirectUrl);
    redirect(loginRedirectUrl);
  }

  return kitchen;
}
