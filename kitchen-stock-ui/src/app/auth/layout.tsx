import { APP_ROUTES } from "@/app-routes";
import { serverTokenStorage } from "@/lib/server/token-storage";
import { redirect } from "next/navigation";

interface Props {
  children: React.ReactNode;
};

const AuthLayout = async ({ children }: Props) => {

  const hasToken = await serverTokenStorage.hasRefreshToken();
  if(hasToken) {
    redirect(APP_ROUTES.APP.HOME);
  }

  return ( 
    <div className="bg-muted flex min-h-svh flex-col items-center justify-center p-6 md:p-10">
      <div className="w-full max-w-sm md:max-w-3xl">
        {children}
      </div>
    </div>
  );
};
 
export default AuthLayout;
