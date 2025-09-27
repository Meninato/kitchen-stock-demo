import { redirect } from "next/navigation";
import type { SearchParams } from 'nuqs/server'

import { APP_ROUTES } from "@/app-routes";
import { loadKitchenSearchParams } from "@/modules/kitchen/hooks/params/server/use-kitchen-qs";
import { getErrorStatusCode } from "@/lib/api-client";
import { kitchenEndpoints } from "@/modules/kitchen/api/endpoint";

interface Props {
  children: React.ReactNode;
  searchParams: Promise<SearchParams>;
}

export default async function KitchenGuard({
  children,
  searchParams
}: Props) {
  const { kitchen: kitchenQs } = await loadKitchenSearchParams(searchParams);
  
  console.log("TEM ALGUMA COISA?", kitchenQs);

  if(!kitchenQs) {
    redirect(APP_ROUTES.APP.KITCHEN_SELECTION);
  } 

  try {
    await kitchenEndpoints.getKitchen(kitchenQs);
  } catch (err) {
    const status = getErrorStatusCode(err);

    if (status === 404) redirect(APP_ROUTES.APP.KITCHEN_SELECTION);
    if (status === 401 || status === 403) redirect(APP_ROUTES.AUTH.SIGN_IN);
    // TODO: generic fallback (could be an error page route)
    redirect(APP_ROUTES.AUTH.SIGN_IN);
  }

  return children;
}