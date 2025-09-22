"use client";

import Image from "next/image";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { ChevronRight } from "lucide-react";

import { 
  Sidebar, 
  SidebarContent, 
  SidebarFooter, 
  SidebarGroup, 
  SidebarGroupContent, 
  SidebarGroupLabel, 
  SidebarHeader, 
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
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

import { sidebarNav } from "@/app-routes";
import { KitchenSwitcher } from "./kitchen-switcher";
import { Button } from "@/components/ui/button";


export const AppSidebar = () => {
  const pathname = usePathname();

  return (
    <Sidebar>
      <SidebarHeader className="text-sidebar-accent-foreground">
        <KitchenSwitcher 
          kitchens={["Hot dog cabuloso", "Delicia de açúcar"]}
          defaultKitchen="Hot dog cabuloso"
        />
        <Button>Nova cozinha</Button>
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
                className="group/label text-sidebar-foreground hover:bg-sidebar-accent hover:text-sidebar-accent-foreground text-sm"
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
                        >
                          <a href={item.href}>{item.title}</a>
                        </SidebarMenuSubButton>
                      </SidebarMenuSubItem>
                    ))}
                  </SidebarMenuSub>
                </CollapsibleContent>
              ) : null}

              {/* <CollapsibleContent>
                <SidebarGroupContent>
                  <SidebarMenu>
                    {item.items?.map((item) => (
                      <SidebarMenuItem key={item.title}>
                        <SidebarMenuButton asChild>
                          <a href={item.href}>{item.title}</a>
                        </SidebarMenuButton>
                      </SidebarMenuItem>
                    ))}
                  </SidebarMenu>
                </SidebarGroupContent>
              </CollapsibleContent> */}

            </SidebarGroup>
          </Collapsible>
        ))}
      </SidebarContent>
      <SidebarRail />
      <SidebarFooter>

      </SidebarFooter>
    </Sidebar>
  );
}