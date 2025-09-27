import { SidebarProvider } from "@/components/ui/sidebar";

import { AppNavbar } from "@/modules/app/ui/components/app-navbar";
import { AppSidebar } from "@/modules/app/ui/components/app-sidebar";
import KitchenInitializer from "@/modules/kitchen/ui/components/kitchen-initializer";
import React from "react";

interface LayoutProps {
  children: React.ReactNode;
}

export default function AppLayout({
  children
}: LayoutProps){
  return (
    <KitchenInitializer>
      <SidebarProvider>
        <AppSidebar />
        <main className="flex flex-col h-screen w-screen bg-muted">
          <AppNavbar />
          {children}
        </main>
      </SidebarProvider>
    </KitchenInitializer>
  );
}