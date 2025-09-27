"use client"

import { Kitchen } from "@/modules/kitchen/api/types"
import { CommandSelect } from "@/components/command-select"
import { CheckIcon } from "lucide-react";

interface Props {
  kitchens: Kitchen[];
  selectedKitchen: Kitchen;
  onSelectKitchen: (kitchen: Kitchen) => void;
}

export function KitchenSwitcher({
  kitchens,
  selectedKitchen,
  onSelectKitchen
}: Props) {

  const handleOnSelect = (kitchenId: string) => {
    const kitchen = kitchens.find(k => k.id == kitchenId)!;
    onSelectKitchen(kitchen);
  } 

  return (
    <div className="p-2"> 
      <CommandSelect className="w-full"
        options={(kitchens ?? []).map((kitchen) => ({
          id: kitchen.id,
          value: kitchen.id,
          children: (
          <div className="flex items-center justify-between w-full">
            <span>{kitchen.name}</span>
            {selectedKitchen.id === kitchen.id && (
              <CheckIcon className="h-4 w-4 text-gray-600" />
            )}
          </div>
          )
        }))}
        onSelect={handleOnSelect}
        // onSearch={setAgentSearch}
        value={selectedKitchen.id}
        placeholder="Seleciona uma cozinha"
      />
    </div>
  )
}