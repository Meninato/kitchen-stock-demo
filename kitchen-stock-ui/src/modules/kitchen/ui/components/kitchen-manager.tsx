"use client";

import { useState } from "react";
import { PlusIcon, EyeIcon, EditIcon } from "lucide-react";
import { useQueryClient } from "@tanstack/react-query";

import { Button } from "@/components/ui/button";

import { KitchenSwitcher } from "./kitchen-switcher";
import { UpdateKitchenDialog } from "./update-kitchen-dialog";
import { NewKitchenDialog } from "@/modules/kitchen/ui/components/new-kitchen-dialog";
import { useKitchenStore } from "@/modules/kitchen/store/kitchen-store";
import { useManyKitchens } from "@/modules/kitchen/hooks/queries/use-many-kitchens";
import { Kitchen } from "@/modules/kitchen/api/types";
import { kitchenQueryKeys } from "@/modules/kitchen/hooks/queries/kitchen-query-keys";

export function KitchenManager() {
  const [isNewDialogOpen, setIsNewDialogOpen] = useState(false);
  const [isEditDialogOpen, setIsEditDialogOpen] = useState(false);
  const queryClient = useQueryClient();
  
  const { selectedKitchen, setSelectedKitchen } = useKitchenStore();
  
  // Check if we have cached kitchens
  const cachedKitchens = queryClient.getQueryData(['kitchens']) as Kitchen[] | undefined;
  
  // Only fetch if cache is stale/expired or we don't have cached data
  const { 
    data: freshKitchens, 
    isLoading: kitchensLoading, 
    isError: kitchensError,
  } = useManyKitchens({
    refetchOnWindowFocus: false // Prevent excessive refetching
  });

  // Determine which kitchens data to use
  const kitchens = freshKitchens || cachedKitchens;

  //TODO: create maybe skeleton for kitchen-switcher in case of failure and add button to refresh the page

  // Handle loading states
  if (kitchensLoading && !cachedKitchens) {
    return <div>Loading kitchens...</div>;
  }

  if (kitchensError && !cachedKitchens) {
    return <div>Error loading kitchens. Please try again.</div>;
  }

  // Handle kitchen selection
  const handleKitchenSelect = (kitchen: Kitchen) => {
    setSelectedKitchen(kitchen);
  };

  // Show loading indicator if data is being refreshed in background
  const isRefreshing = kitchensLoading && cachedKitchens;

  return (
    <>
      <NewKitchenDialog 
        open={isNewDialogOpen} 
        onOpenChange={setIsNewDialogOpen}
        onKitchenCreated={ async (newKitchen) => {
          setSelectedKitchen(newKitchen);
          await queryClient.invalidateQueries({ queryKey: kitchenQueryKeys.lists() });
        }}
      />
      
      {selectedKitchen && (
        <UpdateKitchenDialog 
          open={isEditDialogOpen} 
          onOpenChange={setIsEditDialogOpen} 
          kitchen={selectedKitchen}
          onKitchenUpdated={async (updatedKitchen) => {
            setSelectedKitchen(updatedKitchen);
            await queryClient.invalidateQueries({ queryKey: kitchenQueryKeys.lists() });
          }}
        />
      )}
      
      <div className="relative">
        {isRefreshing && (
          <div className="absolute top-0 right-0 w-2 h-2 bg-blue-500 rounded-full animate-pulse" />
        )}
        
        <KitchenSwitcher
          kitchens={kitchens!}
          selectedKitchen={selectedKitchen!}
          onSelectKitchen={handleKitchenSelect}
          // isLoading={isRefreshing}
        />
      </div>
      
      <div className="flex items-center justify-center gap-2">
        <Button onClick={() => setIsNewDialogOpen(true)}>
          <PlusIcon className="mr-1" />
          Criar
        </Button>
        
        <Button 
          onClick={() => setIsEditDialogOpen(true)}
          disabled={!selectedKitchen}
        >
          <EditIcon />
        </Button>
        
        <Button disabled={!selectedKitchen}>
          <EyeIcon />
        </Button>
      </div>
    </>
  );
}
