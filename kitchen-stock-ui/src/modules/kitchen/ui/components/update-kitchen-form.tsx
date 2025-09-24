"use client";

import { FormUpdateKitchenDto } from "@/modules/kitchen/api/types";

interface Props {
  onSuccess?: () => void;
  onCancel?: () => void;
  initialValues?: FormUpdateKitchenDto;
};

export const UpdateKitchenForm = ({
  onSuccess,
  onCancel,
  initialValues
}: Props) => {
  return (
    <div>Form</div>
  );
}