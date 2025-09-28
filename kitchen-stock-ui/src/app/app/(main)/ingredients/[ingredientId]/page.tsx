import { IngredientDetailView } from "@/modules/ingredients/ui/views/ingredient-detail-view";

interface Props {
  params: Promise<{
    ingredientId: string;
  }>;
}

export default async function IngredientDetailPage({
  params
}: Props) {
  const { ingredientId } = await params;

  return (
    <div className="flex-1 py-4 px-4 md:px-8 flex flex-col gap-y-4">
      <IngredientDetailView ingredientId={ingredientId} />
    </div>
  );
}