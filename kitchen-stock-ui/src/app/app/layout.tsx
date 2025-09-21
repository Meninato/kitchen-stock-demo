import { APP_ROUTES } from "@/app-routes";
import { serverTokenStorage } from "@/lib/server/token-storage";
import { redirect } from "next/navigation";

interface Props {
  children: React.ReactNode;
}

export default async function AppLayout({ children }: Props) {

  const hasToken = await serverTokenStorage.hasRefreshToken();
  if(!hasToken) {
    redirect(APP_ROUTES.AUTH.SIGN_IN);
  }

  return children;
}
