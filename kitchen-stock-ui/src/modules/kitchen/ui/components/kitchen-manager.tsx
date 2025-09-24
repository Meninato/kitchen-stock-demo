"use client";

import { useEffect, useState } from "react";
import { PlusIcon, EyeIcon, EditIcon } from "lucide-react";
import { Button } from "@/components/ui/button";
import { KitchenSwitcher, KitchenSwitcherSkeletonPulse } from "./kitchen-switcher";
import { NewKitchenDialog } from "@/modules/kitchen/ui/components/new-kitchen-dialog";
import { useKitchens } from "@/modules/kitchen/hooks/queries/use-kitchens";
import { useKitchenStore } from "@/modules/kitchen/store/kitchen-store"
import { UpdateKitchenDialog } from "./update-kitchen-dialog";

export function KitchenManager() {
  const [isNewDialogOpen, setIsNewDialogOpen] = useState(false);
  const [isEditDialogOpen, setIsEditDialogOpen] = useState(false);
  const { data: kitchens, isLoading: kitchensLoading, isError: kitchensError } = useKitchens();
  const { selectedKitchen, setSelectedKitchen } = useKitchenStore();

  useEffect(() => {
    if (kitchens && kitchens.length > 0 && !selectedKitchen) {
      setSelectedKitchen(kitchens[0]);
    }
  }, [kitchens, selectedKitchen, setSelectedKitchen]);

  if (kitchensLoading || kitchensError) {
    return (
      <KitchenSwitcherSkeletonPulse />
    );
  }

  if (!kitchens || kitchens.length === 0) {
    return (
      <>
        <NewKitchenDialog open={isNewDialogOpen} onOpenChange={setIsNewDialogOpen} />
        <div className="flex flex-col items-center justify-center h-full gap-2">
          <p className="text-center">Seja bem-vindo</p>
          <Button onClick={() => setIsNewDialogOpen(true)}>
            <PlusIcon className="mr-1" />
            Crie sua cozinha
          </Button>
        </div>
      </>
    );
  }

  return (
    <>
      <NewKitchenDialog open={isNewDialogOpen} onOpenChange={setIsNewDialogOpen} />
      <UpdateKitchenDialog open={isEditDialogOpen} onOpenChange={setIsEditDialogOpen} kitchen={selectedKitchen!} />
      <KitchenSwitcher 
        kitchens={kitchens}
        selectedKitchen={selectedKitchen}
        onSelectKitchen={setSelectedKitchen}
      />
      <div className="flex items-center justify-center gap-2">
        <Button onClick={() => setIsNewDialogOpen(true)}>
          <PlusIcon className="mr-1" />
          Criar
        </Button>
        <Button onClick={() => setIsEditDialogOpen(true)}>
          <EditIcon />
        </Button>
        <Button>
          <EyeIcon />
        </Button>
      </div>
    </>
  );
}
