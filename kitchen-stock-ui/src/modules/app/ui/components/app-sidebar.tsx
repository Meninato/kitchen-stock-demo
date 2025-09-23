"use client";

import { useState } from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { ChevronRight, EyeIcon, PlusIcon } from "lucide-react";
import { useRouter } from "next/navigation";

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
import { KitchenSwitcher, KitchenSwitcherSkeletonPulse } from "./kitchen-switcher";
import { Button } from "@/components/ui/button";
import { NavUser, NavUserSkeleton } from "./nav-user";
import { useKitchens } from "@/modules/kitchen/hooks/queries/use-kitchens";
import { useAuthMe } from "@/modules/auth/hooks/queries/use-auth-me";
import { useAuthLogout } from "@/modules/auth/hooks/mutations/use-auth-logout";
import { clearTokensAction } from "@/modules/auth/actions/clear-tokens-action";
import { NewKitchenDialog } from "@/modules/kitchen/ui/components/new-kitchen-dialog";

export const AppSidebar = () => {
  const router = useRouter();
  const pathname = usePathname();
  const [isDialogOpen, setIsDialogOpen] = useState(false);

  const ensureTokenIsDeleted = async () => {
    await clearTokensAction();
    router.push(APP_ROUTES.AUTH.SIGN_IN);
  };
  
  const { data: kitchens, isLoading: kichenIsLoading, isError: kitchenIsError } = useKitchens();
  const { data: user, isLoading: userIsLoading, isError: userIsError } = useAuthMe();
  const { isPending: isLoggingOut, mutateAsync: logoutAsync } = useAuthLogout({
    onSuccess: async () => await ensureTokenIsDeleted(),
    onError: async () => await ensureTokenIsDeleted()
  });

  function KitchenSwitcherWrapper() {
    if (kichenIsLoading || kitchenIsError) return <KitchenSwitcherSkeletonPulse />;

    if (!kitchens || kitchens.length === 0) {
      return (
        <div className="flex items-center justify-center h-full">
          <p className="">Seja bem-vindo</p>
        </div>
      );
    }

    return (
      <KitchenSwitcher
        kitchens={kitchens}
        defaultKitchen={kitchens[0]}
      />
    );
  }

  function NavUserWrapper() {
    if (userIsLoading || userIsError) return <NavUserSkeleton />;

    return (
      <NavUser 
        user={user!}
        logoutActions={{
          onLogout: logoutAsync,
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
          <KitchenSwitcherWrapper />
          <div className="flex items-center justify-center gap-2">
            <Button onClick={() => setIsDialogOpen(true)}>
              <PlusIcon />
              Criar
            </Button>
            <Button>
              <EyeIcon />
              Ver
            </Button>
          </div>
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

export function SidebarSkeletonPulse() {
  const mockGroups = [
    { title: "Menu", items: 3 },
    { title: "Cadastros", items: 4 },
  ];

  return (
    <div className="w-64 border-r bg-background h-screen">
      <div className="p-4 space-y-4">
        {/* Header */}
        <div className="space-y-3">
          <div className="h-12 bg-foreground/5 rounded-lg animate-pulse" />
          <div className="h-9 bg-foreground/5 rounded-md animate-pulse" />
        </div>
        
        {/* Menu Items */}
        <div className="space-y-6 pt-4">
          {mockGroups.map((group, index) => (
            <div key={index} className="space-y-2">
              <div className="h-4 w-20 bg-foreground/10 rounded animate-pulse" />
              <div className="space-y-1 pl-2">
                {Array.from({ length: group.items }).map((_, i) => (
                  <div key={i} className="h-8 bg-foreground/5 rounded animate-pulse" />
                ))}
              </div>
            </div>
          ))}
        </div>
      </div>
      
      {/* Footer */}
      <div className="absolute bottom-0 left-0 right-0 p-4">
        <div className="flex items-center gap-3">
          <div className="size-10 rounded-full bg-foreground/10 animate-pulse" />
          <div className="flex-1 space-y-1">
            <div className="h-3 w-20 bg-foreground/10 rounded animate-pulse" />
            <div className="h-3 w-32 bg-foreground/5 rounded animate-pulse" />
          </div>
        </div>
      </div>
    </div>
  );
}