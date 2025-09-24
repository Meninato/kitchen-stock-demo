"use client";

import { ResponsiveDialog } from "@/components/responsive-dialog";
import { FormUpdateKitchenDto } from "@/modules/kitchen/api/types";
import { UpdateKitchenForm } from "./update-kitchen-form";

interface Props {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  kitchen: FormUpdateKitchenDto;
};

export const UpdateKitchenDialog = ({
  open,
  onOpenChange,
  kitchen
}: Props) => {
  return (
    <ResponsiveDialog
      title="Atualizar Cozinha"
      description="Modifique as informações da cozinha"
      open={open}
      onOpenChange={onOpenChange}
    >
      <UpdateKitchenForm
        onSuccess={() => onOpenChange(false)}
        onCancel={() => onOpenChange(false)}
        kitchen={kitchen}
      />
    </ResponsiveDialog>
  );
};