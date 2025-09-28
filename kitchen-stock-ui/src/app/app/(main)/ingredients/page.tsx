import { IngredientsToolbar } from "@/modules/ingredients/ui/components/ingredients-toolbar";
import { IngredientsView } from "@/modules/ingredients/ui/views/ingredients-view";

export default function IngredientsPage() {
  return (
    <>
      <IngredientsToolbar />
      <IngredientsView />
    </>
  );
}