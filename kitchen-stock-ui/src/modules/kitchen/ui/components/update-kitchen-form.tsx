"use client";

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { toast } from "sonner";

import { getErrorMessage } from "@/lib/api-client";
import { FormUpdateKitchenDto, kitchenUpdateSchema } from "@/modules/kitchen/api/types";
import { useUpdateKitchen } from "@/modules/kitchen/hooks/mutations/use-update-kitchen";
import { useKitchenStore } from "@/modules/kitchen/store/kitchen-store";
import { KitchenForm } from "./kitchen-form";

interface Props {
  onSuccess?: () => void;
  onCancel?: () => void;
  kitchen: FormUpdateKitchenDto;
};

export const UpdateKitchenForm = ({
  onSuccess,
  onCancel,
  kitchen
}: Props) => {
  const { selectedKitchen, setSelectedKitchen } = useKitchenStore();
  const { isPending, mutateAsync: updateKitchenAsync } = useUpdateKitchen({
    onSuccess: () => {
      onSuccess?.();
    }
  });

  const form = useForm<FormUpdateKitchenDto>({
    resolver: zodResolver(kitchenUpdateSchema),
    defaultValues: {
      name: kitchen.name ?? "",
      description: kitchen.description ?? ""
    },
  });

  const handleSubmit = async (data: FormUpdateKitchenDto) => {
    try {
      const updatedKitchen = await updateKitchenAsync({id: selectedKitchen!.id, data});
      setSelectedKitchen(updatedKitchen);
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