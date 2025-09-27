"use client";

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { toast } from "sonner";

import { getErrorMessage } from "@/lib/api-client";
import { FormCreateKitchenDto, Kitchen, kitchenCreateSchema } from "@/modules/kitchen/api/types";
import { useCreateKitchen } from "@/modules/kitchen/hooks/mutations/use-create-kitchen";
import { KitchenForm } from "./kitchen-form";

interface Props {
  onSuccess?: (kitchen: Kitchen) => void;
  onCancel?: () => void;
};

export const CreateKitchenForm = ({
  onSuccess,
  onCancel
}: Props) => {
  const { isPending, mutateAsync: createKitchenAsync } = useCreateKitchen({
    onSuccess: (data) => {
      onSuccess?.(data);
    }
  });


  const form = useForm<FormCreateKitchenDto>({
    resolver: zodResolver(kitchenCreateSchema),
    defaultValues: {
      name: "",
      description: ""
    },
  });

  const handleSubmit = async (data: FormCreateKitchenDto) => {
    try {
      await createKitchenAsync(data);
    } catch(err) {
      const message = getErrorMessage(err);
      toast.error(message)
    }
  };

  return (
    <KitchenForm
      form={form}
      isSubmitting={isPending}
      onSubmit={handleSubmit}
      onCancel={onCancel}
      submitLabel="Salvar"
    />
  );
}