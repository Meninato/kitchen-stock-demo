"use client";

import { ResponsiveDialog } from "@/components/responsive-dialog";
import { FormUpdateKitchenDto } from "@/modules/kitchen/api/types";
import { UpdateKitchenForm } from "./update-kitchen-form";

interface Props {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  initialValues: FormUpdateKitchenDto;
};

export const UpdateKitchenDialog = ({
  open,
  onOpenChange,
  initialValues
}: Props) => {
  return (
    <ResponsiveDialog
      title="Edit Agent"
      description="Edit the agent details"
      open={open}
      onOpenChange={onOpenChange}
    >
      <UpdateKitchenForm
        onSuccess={() => onOpenChange(false)}
        onCancel={() => onOpenChange(false)}
        initialValues={initialValues}
      />
    </ResponsiveDialog>
  );
};