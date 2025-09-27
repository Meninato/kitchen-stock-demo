"use client";

import { ResponsiveDialog } from "@/components/responsive-dialog";
import { CreateKitchenForm } from "./create-kitchen-form";
import { Kitchen } from "../../api/types";

interface Props {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onKitchenCreated?: (kitchen: Kitchen) => Promise<void>;
};

export const NewKitchenDialog = ({
  open,
  onOpenChange,
  onKitchenCreated
}: Props) => {
  return (
    <ResponsiveDialog
      title="Nova Cozinha"
      description="Crie uma nova cozinha"
      open={open}
      onOpenChange={onOpenChange}
    >
      <CreateKitchenForm
        onSuccess={async (k) => {
          onOpenChange(false);
          await onKitchenCreated?.(k);
        }}
        onCancel={() => onOpenChange(false)}
      />
    </ResponsiveDialog>
  );
};