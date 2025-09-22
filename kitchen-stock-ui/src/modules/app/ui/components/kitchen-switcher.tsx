"use client"

import * as React from "react"
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
import { Skeleton } from "@/components/ui/skeleton"

import { Kitchen } from "@/modules/kitchen/api/types"

interface Props {
  kitchens: Kitchen[];
  defaultKitchen: Kitchen;
}

export function KitchenSwitcher({
  kitchens,
  defaultKitchen,
}: Props) {
  const [selectedKitchen, setSelectedKitchen] = React.useState(defaultKitchen)

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
                <span className="">{selectedKitchen.name}</span>
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
                onSelect={() => setSelectedKitchen(kitchen)}
              >
                {kitchen.name}{" "}
                {kitchen.id === selectedKitchen.id && <Check className="ml-auto" />}
              </DropdownMenuItem>
            ))}
          </DropdownMenuContent>
        </DropdownMenu>
      </SidebarMenuItem>
    </SidebarMenu>
  )
}

export function KitchenSwitcherSkeleton() {
  return (
    <SidebarMenu>
      <SidebarMenuItem>
        <SidebarMenuButton
          size="lg"
          className="pointer-events-none"
          disabled
        >
          <div className="bg-sidebar-primary text-sidebar-primary-foreground flex aspect-square size-8 items-center justify-center rounded-lg">
            <GalleryVerticalEnd className="size-4" />
          </div>
          <div className="flex flex-col gap-0.5 leading-none">
            <Skeleton className="h-4 w-16" />
            <Skeleton className="h-3 w-24 mt-1" />
          </div>
          <Skeleton className="ml-auto h-4 w-4" />
        </SidebarMenuButton>
      </SidebarMenuItem>
    </SidebarMenu>
  )
}

export function KitchenSwitcherSkeletonPulse() {
  return (
    <SidebarMenu>
      <SidebarMenuItem>
        <div className="flex items-center gap-2 px-2 py-2">
          <div className="bg-sidebar-primary/20 flex aspect-square size-8 items-center justify-center rounded-lg animate-pulse">
            <div className="size-4 bg-sidebar-primary/30 rounded" />
          </div>
          <div className="flex flex-col gap-1.5 flex-1">
            <div className="h-4 w-16 bg-sidebar-primary/20 rounded animate-pulse" />
            <div className="h-3 w-24 bg-sidebar-primary/10 rounded animate-pulse" />
          </div>
          <div className="h-4 w-4 bg-sidebar-primary/20 rounded animate-pulse" />
        </div>
      </SidebarMenuItem>
    </SidebarMenu>
  )
}
