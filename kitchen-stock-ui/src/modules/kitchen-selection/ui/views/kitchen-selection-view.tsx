"use client"

import { useEffect, useState } from "react";
import { PlusIcon, EditIcon } from "lucide-react";
import { useSearchParams, useRouter } from "next/navigation";
import { useQueryClient } from "@tanstack/react-query";

import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";

import { KitchenSwitcher } from "@/modules/kitchen/ui/components/kitchen-switcher";
import { NewKitchenDialog } from "@/modules/kitchen/ui/components/new-kitchen-dialog";
import { UpdateKitchenDialog } from "@/modules/kitchen/ui/components/update-kitchen-dialog";
import { useManyKitchens } from "@/modules/kitchen/hooks/queries/use-many-kitchens";
import { Loader } from "@/components/loader";
import { APP_ROUTES } from "@/app-routes";
import { useKitchenStore } from "@/modules/kitchen/store/kitchen-store";
import { Kitchen } from "@/modules/kitchen/api/types";
import { kitchenQueryKeys } from "@/modules/kitchen/hooks/queries/kitchen-query-keys";

export const KitchenSelectionView = () => {
  const [isNewDialogOpen, setIsNewDialogOpen] = useState(false);
  const [isEditDialogOpen, setIsEditDialogOpen] = useState(false);
  const { data: kitchens, isLoading: kitchensLoading, isError: kitchensError } = useManyKitchens();
  const { selectedKitchen, setSelectedKitchen } = useKitchenStore();
  const searchParams = useSearchParams();
  const router = useRouter();
  const queryClient = useQueryClient();

  useEffect(() => {
    if (!kitchens || kitchens.length === 0) return;

    if (!selectedKitchen) {
      setSelectedKitchen(kitchens[0]);
    }
  }, [kitchens, selectedKitchen, setSelectedKitchen]);

  if (kitchensLoading || kitchensError) {
    return (
      <Loader texts={['Preparando a sua cozinha...']} />
    );
  }

  if (!kitchens || kitchens.length === 0) {
    return (
      <>
        <NewKitchenDialog open={isNewDialogOpen} onOpenChange={setIsNewDialogOpen} />
        <div className="flex flex-col items-center justify-center h-screen gap-4">
          <h1 className="text-2xl font-semibold">Bem-vindo 👋</h1>
          <p className="text-muted-foreground text-center max-w-sm">
            Parece que você ainda não tem nenhuma cozinha configurada.  
            Crie sua primeira cozinha para começar a organizar suas receitas e ingredientes.
          </p>
          <Button size="lg" onClick={() => setIsNewDialogOpen(true)}>
            <PlusIcon className="mr-2 h-4 w-4" />
            Criar minha cozinha
          </Button>
        </div>
      </>
    );
  }

  const foundKitchen = kitchens.find(k => k.id === selectedKitchen?.id);

    if (!foundKitchen) {
    return <Loader texts={['Quase lá!']} />
  }

  const handleContinue = () => {
    const redirectPath = searchParams.get('redirect');
    const destinationPath = redirectPath || APP_ROUTES.APP.HOME; 
    
    router.push(destinationPath);
  };

  const handleOnKitchenChange = async (k: Kitchen) => {
    setSelectedKitchen(k);
    await queryClient.invalidateQueries({ queryKey: kitchenQueryKeys.lists() });
  }

  return (
    <>
      <NewKitchenDialog open={isNewDialogOpen} onOpenChange={setIsNewDialogOpen} onKitchenCreated={handleOnKitchenChange} />
      <UpdateKitchenDialog open={isEditDialogOpen} onOpenChange={setIsEditDialogOpen} onKitchenUpdated={handleOnKitchenChange} kitchen={selectedKitchen!} />

      <div className="flex items-center justify-center h-screen">
        <Card className="w-full max-w-lg shadow-lg rounded-2xl">
          <CardHeader>
            <CardTitle>Gerenciar Cozinhas</CardTitle>
            <CardDescription>
              Selecione, edite ou crie novas cozinhas para organizar seu trabalho.
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-6">
            <KitchenSwitcher 
              kitchens={kitchens}
              selectedKitchen={foundKitchen}
              onSelectKitchen={(k) => {
                setSelectedKitchen(k);
              }}
            />

            <Separator />

              <div className="flex items-center justify-between"> 
                <Button variant="destructive" onClick={handleContinue}>
                  Continuar
                </Button>
                
                {/* WRAP: Existing buttons are now grouped in a separate div */}
                <div className="flex items-center gap-2">
                  <Button onClick={() => setIsNewDialogOpen(true)}>
                    <PlusIcon className="mr-1 h-4 w-4" />
                    Criar
                  </Button>
                  <Button onClick={() => setIsEditDialogOpen(true)} disabled={!selectedKitchen}>
                    <EditIcon className="h-4 w-4" />
                  </Button>
                </div>
              </div>
          </CardContent>
        </Card>
      </div>
    </>
  );
}