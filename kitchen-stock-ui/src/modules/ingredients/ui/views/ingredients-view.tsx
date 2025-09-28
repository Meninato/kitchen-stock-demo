"use client";

import { useRouter } from "next/navigation";

import { APP_ROUTES } from "@/app-routes";
import { DataTable } from "@/components/data-table";
import { DataPagination } from "@/components/data-table-pagination";
import { columns } from "@/modules/ingredients/ui/components/columns";
import { useIngredients } from "@/modules/ingredients/hooks/queries/use-ingredients";
import { useKitchenStore } from "@/modules/kitchen/store/kitchen-store";

export const IngredientsView = () => {
  const router = useRouter();
  const { selectedKitchen } = useKitchenStore();
  const { data: ingredients, isLoading } = useIngredients({
    kitchenId: selectedKitchen!.id
  });

  if(isLoading) {
    return <div>Loading ingredients table</div>
  }

  if(!ingredients) {
    return <div>Ingredients are not ready yet</div>
  }

  //TODO: empty state e loading state skeleton

  return (
    <div className="flex-1 pb-4 px-4 md:px-8 flex flex-col gap-y-4">
      <DataTable 
        data={ingredients.data} 
        columns={columns} 
        onRowClick={(row) => router.push(`${APP_ROUTES.APP.INGREDIENTS}/${row.id}`)}
      />
      {/* <div className="flex flex-col items-center justify-center">
        <div className="flex flex-col gap-y-6 max-w-md mx-auto text-center">
          <h6 className="text-lg font-medium">Nada pra se ver aqui bb</h6>
          <p className="text-sm text-muted-foreground">Talvez outra hora</p>
        </div>
      </div> */}
      <DataPagination
        page={1}
        totalPages={ingredients.pagination.totalPages}
        onPageChange={(page) => {}}
      />
      {/* {data.items.length === 0 && (
        <EmptyState
          title="Create your first meeting"
          description="Schedule a meeting to connect with others. Each meeting lets you collaborate, share ideas, and interact with participants in real time."
        />
      )} */}
    </div>
  );
}