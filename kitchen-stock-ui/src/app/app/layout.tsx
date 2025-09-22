import { redirect } from "next/navigation";

import { SidebarProvider } from "@/components/ui/sidebar";

import { APP_ROUTES } from "@/app-routes";
import { serverTokenStorage } from "@/lib/server/token-storage";
import { AppNavbar } from "@/modules/app/ui/components/app-navbar";
import { AppSidebar } from "@/modules/app/ui/components/app-sidebar";

interface Props {
  children: React.ReactNode;
}

export default async function AppLayout({ children }: Props) {

  const hasToken = await serverTokenStorage.hasRefreshToken();
  if(!hasToken) {
    redirect(APP_ROUTES.AUTH.SIGN_IN);
  }

  return (
    <SidebarProvider>
      <AppSidebar />
      <main className="flex flex-col h-screen w-screen bg-muted">
        <AppNavbar />
        {children}
      </main>
    </SidebarProvider>
  );
}
