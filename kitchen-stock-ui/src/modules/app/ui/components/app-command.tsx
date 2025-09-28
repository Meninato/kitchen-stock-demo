"use client";

import { useRouter } from "next/navigation";
import { Dispatch, SetStateAction, useState } from "react";
import { keepPreviousData } from "@tanstack/react-query";

import { 
  CommandResponsiveDialog, 
  CommandInput, 
  CommandItem, 
  CommandList,
  CommandGroup,
  CommandEmpty
} from "@/components/ui/command";

import { useIngredients } from "@/modules/ingredients/hooks/queries/use-ingredients";
import { useKitchenStore } from "@/modules/kitchen/store/kitchen-store";
import { useDebounce } from "@/hooks/use-debounce";

interface Props {
  open: boolean;
  setOpen: Dispatch<SetStateAction<boolean>>;
};

export const AppCommand = ({ open, setOpen }: Props) => {
  const router = useRouter();
  const [search, setSearch] = useState("");
  const debouncedSearch = useDebounce(search, 300);
  const { selectedKitchen } = useKitchenStore();

  const { data: ingredients } = useIngredients({
    kitchenId: selectedKitchen!.id,
    config: {
      filter: {
        searchTerm: debouncedSearch
      },
      pagination: {
        page: 1,
        pageSize: 100,
      }
    },
  }, {
    placeholderData: keepPreviousData
  });

  return (
    <CommandResponsiveDialog open={open} onOpenChange={setOpen}>
      <CommandInput
        placeholder="O que você procura?"
        value={search}
        onValueChange={(value) => setSearch(value)}
      />
      <CommandList>
        <CommandGroup heading="Ingredientes">
          <CommandEmpty>
            <span className="text-muted-foreground text-sm">
              Nenhum ingrediente
            </span>
          </CommandEmpty>
          {ingredients?.data.map((ingredient) => (
            <CommandItem
              onSelect={() => {
                router.push(`/app/ingredients/${ingredient.id}`);
                setOpen(false);
              }}
              key={ingredient.id}
            >
              {ingredient.name}
            </CommandItem>
          ))}
        </CommandGroup>
      </CommandList>
    </CommandResponsiveDialog>
  );
};
