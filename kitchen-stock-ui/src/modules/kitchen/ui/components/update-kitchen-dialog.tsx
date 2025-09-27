"use client";

import { ResponsiveDialog } from "@/components/responsive-dialog";

import { FormUpdateKitchenDto, Kitchen } from "@/modules/kitchen/api/types";
import { UpdateKitchenForm } from "./update-kitchen-form";

interface Props {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  kitchen: FormUpdateKitchenDto;
  onKitchenUpdated?: (kitchen: Kitchen) => Promise<void>;
};

export const UpdateKitchenDialog = ({
  open,
  onOpenChange,
  kitchen,
  onKitchenUpdated
}: Props) => {
  return (
    <ResponsiveDialog
      title="Atualizar Cozinha"
      description="Modifique as informações da cozinha"
      open={open}
      onOpenChange={onOpenChange}
    >
      <UpdateKitchenForm
        onSuccess={ async (k) => {
          onOpenChange(false);
          await onKitchenUpdated?.(k);
        }}
        onCancel={() => onOpenChange(false)}
        kitchen={kitchen}
      />
    </ResponsiveDialog>
  );
};