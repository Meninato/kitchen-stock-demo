"use client"

import { Check, ChevronsUpDown, GalleryVerticalEnd } from "lucide-react"

import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import {
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from "@/components/ui/sidebar"

import { Kitchen } from "@/modules/kitchen/api/types"

interface Props {
  kitchens: Kitchen[];
  selectedKitchen: Kitchen | null;
  onSelectKitchen: (kitchen: Kitchen) => void;
}

export function KitchenSwitcher({
  kitchens,
  selectedKitchen,
  onSelectKitchen
}: Props) {

  return (
    <SidebarMenu>
      <SidebarMenuItem>
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <SidebarMenuButton
              size="lg"
              className="data-[state=open]:bg-sidebar-accent data-[state=open]:text-sidebar-accent-foreground"
            >
              <div className="bg-sidebar-primary text-sidebar-primary-foreground flex aspect-square size-8 items-center justify-center rounded-lg">
                <GalleryVerticalEnd className="size-4" />
              </div>
              <div className="flex flex-col gap-0.5 leading-none">
                <span className="font-medium">Cozinha</span>
                <span className="">{selectedKitchen?.name ?? "Nenhuma"}</span>
              </div>
              <ChevronsUpDown className="ml-auto" />
            </SidebarMenuButton>
          </DropdownMenuTrigger>
          <DropdownMenuContent
            className="w-(--radix-dropdown-menu-trigger-width)"
            align="start"
          >
            {kitchens.map((kitchen) => (
              <DropdownMenuItem
                key={kitchen.id}
                onSelect={() => onSelectKitchen(kitchen)}
              >
                {kitchen.name}{" "}
                {kitchen.id === selectedKitchen?.id && <Check className="ml-auto" />}
              </DropdownMenuItem>
            ))}
          </DropdownMenuContent>
        </DropdownMenu>
      </SidebarMenuItem>
    </SidebarMenu>
  )
}

export function KitchenSwitcherSkeletonPulse() {
  return (
    <div className="space-y-2">
      {/* Kitchen Switcher Skeleton */}
      <SidebarMenu>
        <SidebarMenuItem>
          <SidebarMenuButton
            size="lg"
            className="pointer-events-none"
            disabled
          >
            <div className="bg-sidebar-primary/20 flex aspect-square size-8 items-center justify-center rounded-lg animate-pulse">
              <div className="size-4 bg-sidebar-primary/30 rounded" />
            </div>
            <div className="flex flex-col gap-1.5 flex-1">
              <div className="h-4 w-16 bg-sidebar-primary/20 rounded animate-pulse" />
              <div className="h-3 w-24 bg-sidebar-primary/10 rounded animate-pulse" />
            </div>
            <div className="h-4 w-4 bg-sidebar-primary/20 rounded animate-pulse ml-auto" />
          </SidebarMenuButton>
        </SidebarMenuItem>
      </SidebarMenu>
      
      {/* Button Skeletons */}
      <div className="flex gap-2 px-2">
        <div className="h-8 flex-1 bg-sidebar-primary/15 rounded-md animate-pulse" />
        <div className="h-8 flex-1 bg-sidebar-primary/15 rounded-md animate-pulse" />
      </div>
    </div>
  );
}
