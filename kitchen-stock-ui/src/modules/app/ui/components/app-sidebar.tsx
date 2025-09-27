"use client";

import { useState } from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { ChevronRight } from "lucide-react";

import { 
  Sidebar, 
  SidebarContent, 
  SidebarFooter, 
  SidebarGroup, 
  SidebarGroupLabel, 
  SidebarHeader, 
  SidebarMenuSub,
  SidebarMenuSubButton,
  SidebarMenuSubItem,
  SidebarRail
} from "@/components/ui/sidebar";
import { 
  Collapsible, 
  CollapsibleContent, 
  CollapsibleTrigger 
} from "@/components/ui/collapsible";

import { cn } from "@/lib/utils";
import { APP_ROUTES, sidebarNav } from "@/app-routes";
import { NavUser, NavUserSkeleton } from "./nav-user";
import { useAuthMe } from "@/modules/auth/hooks/queries/use-auth-me";
import { useAuthLogout } from "@/modules/auth/hooks/mutations/use-auth-logout";
import { clearTokensAction } from "@/modules/auth/actions/clear-tokens-action";
import { NewKitchenDialog } from "@/modules/kitchen/ui/components/new-kitchen-dialog";
import { KitchenManager } from "@/modules/kitchen/ui/components/kitchen-manager";
import { useKitchenStore } from "@/modules/kitchen/store/kitchen-store";

export const AppSidebar = () => {
  const pathname = usePathname();
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const { data: user } = useAuthMe();
  const { isPending: isLoggingOut, mutateAsync: logoutAsync } = useAuthLogout();

  const endSession = async () => {
    await Promise.all([clearTokensAction(), logoutAsync()]);

    useKitchenStore.persist.clearStorage();

    // HARD NAVIGATION RESET
    window.location.href = APP_ROUTES.AUTH.SIGN_IN;

    //TODO: perhaps use soft reset clearing react queries and resettign selected kitchen
  }

  function NavUserWrapper() {
    if (!user) return <NavUserSkeleton />;

    return (
      <NavUser 
        user={user!}
        logoutActions={{
          onLogout: endSession,
          isLoggingOut 
        }}
      />
    );
  }

  return (
    <>
      <NewKitchenDialog open={isDialogOpen} onOpenChange={setIsDialogOpen} />
      <Sidebar>
        <SidebarHeader className="text-sidebar-accent-foreground">
          <KitchenManager />
        </SidebarHeader>
        <SidebarContent className="gap-0">
          {sidebarNav.map((item) => (
            <Collapsible
              key={item.title}
              title={item.title}
              defaultOpen
              className="group/collapsible"
            >
              <SidebarGroup>
                <SidebarGroupLabel
                  asChild
                  className="uppercase group/label text-sidebar-foreground hover:bg-sidebar-accent hover:text-sidebar-accent-foreground text-sm"
                >
                  <CollapsibleTrigger>
                      {item.title}{" "}
                      <ChevronRight className="ml-auto transition-transform group-data-[state=open]/collapsible:rotate-90" />
                  </CollapsibleTrigger>
                </SidebarGroupLabel>
                {item.items?.length ? (
                  <CollapsibleContent>
                    <SidebarMenuSub>
                      {item.items.map((item) => (
                        <SidebarMenuSubItem key={item.title}>
                          <SidebarMenuSubButton
                            asChild
                            isActive={pathname === item.href}
                            className={cn(
                              "h-10 hover:bg-linear-to-r/oklch border border-transparent hover:border-[#5D6B68]/10 from-sidebar-accent from-5% via-30% via-sidebar/50 to-sidebar/50",
                              pathname === item.href && "bg-linear-to-r/oklch border-[#5D6B68]/10"
                            )}
                          >
                            <Link href={item.href}>{item.title}</Link>
                          </SidebarMenuSubButton>
                        </SidebarMenuSubItem>
                      ))}
                    </SidebarMenuSub>
                  </CollapsibleContent>
                ) : null}
              </SidebarGroup>
            </Collapsible>
          ))}
        </SidebarContent>
        <SidebarFooter>
          <NavUserWrapper />
        </SidebarFooter>
        <SidebarRail />
      </Sidebar>
    </>
  );
}