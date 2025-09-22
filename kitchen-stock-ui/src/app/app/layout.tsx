import { redirect } from "next/navigation";

import { SidebarProvider } from "@/components/ui/sidebar";

import { APP_ROUTES } from "@/app-routes";
import { serverTokenStorage } from "@/lib/server/token-storage";
import { AppNavbar, NavbarSkeletonPulse } from "@/modules/app/ui/components/app-navbar";
import { AppSidebar, SidebarSkeletonPulse } from "@/modules/app/ui/components/app-sidebar";

interface Props {
  children: React.ReactNode;
}

export default async function AppLayout({ children }: Props) {

  //check refresh token to spare a call
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

export function AppLayoutSkeleton({ children }: { children?: React.ReactNode }) {
  return (
    <SidebarProvider defaultOpen>
      <SidebarSkeletonPulse />
      <main className="flex flex-col h-screen w-screen bg-muted">
        <NavbarSkeletonPulse />
        <div className="flex-1 overflow-auto p-6">
          {children || <ContentSkeleton />}
        </div>
      </main>
    </SidebarProvider>
  );
}

function ContentSkeleton() {
  return (
    <div className="space-y-6 animate-pulse">
      {/* Page Title */}
      <div className="space-y-2">
        <div className="h-8 w-48 bg-foreground/10 rounded" />
        <div className="h-4 w-96 bg-foreground/5 rounded" />
      </div>
      
      {/* Cards Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {Array.from({ length: 6 }).map((_, i) => (
          <div key={i} className="p-6 rounded-lg border bg-card">
            <div className="space-y-3">
              <div className="h-5 w-24 bg-foreground/10 rounded" />
              <div className="h-4 w-full bg-foreground/5 rounded" />
              <div className="h-4 w-3/4 bg-foreground/5 rounded" />
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}