import { ResponsiveDialog } from "@/components/responsive-dialog";
import { CreateKitchenForm } from "./create-kitchen-form";

interface Props {
  open: boolean;
  onOpenChange: (open: boolean) => void;
};

export const NewKitchenDialog = ({
  open,
  onOpenChange,
}: Props) => {
  return (
    <ResponsiveDialog
      title="Nova Cozinha"
      description="Crie uma nova cozinha"
      open={open}
      onOpenChange={onOpenChange}
    >
      <CreateKitchenForm
        onSuccess={() => onOpenChange(false)}
        onCancel={() => onOpenChange(false)}
      />
    </ResponsiveDialog>
  );
};