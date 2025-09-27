"use client";

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { toast } from "sonner";

import { getErrorMessage } from "@/lib/api-client";
import { FormCreateKitchenDto, kitchenCreateSchema } from "@/modules/kitchen/api/types";
import { useCreateKitchen } from "@/modules/kitchen/hooks/mutations/use-create-kitchen";
import { useKitchenQs } from "@/modules/kitchen/hooks/params/use-kitchen-qs";
import { KitchenForm } from "./kitchen-form";

interface Props {
  onSuccess?: () => void;
  onCancel?: () => void;
};

export const CreateKitchenForm = ({
  onSuccess,
  onCancel
}: Props) => {
  const [, setKitchenQs] = useKitchenQs();
  const { isPending, mutateAsync: createKitchenAsync } = useCreateKitchen({
    onSuccess: () => {
      onSuccess?.();
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
      const newKitchen = await createKitchenAsync(data);
      setKitchenQs(newKitchen.id);
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