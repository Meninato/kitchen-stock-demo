"use client";

import { useRouter } from "next/navigation";
import { Dispatch, SetStateAction, useState } from "react";

import { 
  CommandResponsiveDialog, 
  CommandInput, 
  CommandItem, 
  CommandList,
  CommandGroup,
  CommandEmpty
} from "@/components/ui/command";

interface Props {
  open: boolean;
  setOpen: Dispatch<SetStateAction<boolean>>;
};

interface MockupData {
  id: string;
  name: string;
}

export const AppCommand = ({ open, setOpen }: Props) => {
  const router = useRouter();
  const [search, setSearch] = useState("");

  const ingredients: MockupData[] = []; // empty
  const recipes: MockupData[] = []; // empty
  const suppliers: MockupData[] = [
    { id: "a", name: "John Doe Varejão" },
    { id: "b", name: "Gisele Hortaliças" }
  ];

  return (
    <CommandResponsiveDialog shouldFilter={true} open={open} onOpenChange={setOpen}>
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
          {ingredients.map((ingredient) => (
            <CommandItem
              onSelect={() => {
                // router.push();
                setOpen(false);
              }}
              key={ingredient.id}
            >
              {ingredient.name}
            </CommandItem>
          ))}
        </CommandGroup>
        <CommandGroup heading="Receitas">
          <CommandEmpty>
            <span className="text-muted-foreground text-sm">
              Nenhuma receita
            </span>
          </CommandEmpty>
          {recipes.map((recipe) => (
            <CommandItem
              onSelect={() => {
                // router.push();
                setOpen(false);
              }}
              key={recipe.id}
            >
              {recipe.name}
            </CommandItem>
          ))}
        </CommandGroup>
        <CommandGroup heading="Fornecedores">
          <CommandEmpty>
            <span className="text-muted-foreground text-sm">
              Nenhum fornecedor
            </span>
          </CommandEmpty>
          {suppliers.map((supplier) => (
            <CommandItem
              onSelect={() => {
                // router.push();
                setOpen(false);
              }}
              key={supplier.id}
            >
              {supplier.name}
            </CommandItem>
          ))}
        </CommandGroup>
      </CommandList>
    </CommandResponsiveDialog>
  );
};
