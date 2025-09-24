"use client";

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { toast } from "sonner";

import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";

import { getErrorMessage } from "@/lib/api-client";
import { FormCreateKitchenDto, kitchenCreateSchema } from "@/modules/kitchen/api/types";
import { useCreateKitchen } from "@/modules/kitchen/hooks/mutations/use-create-kitchen";
import { useKitchenStore } from "@/modules/kitchen/store/kitchen-store";

interface Props {
  onSuccess?: () => void;
  onCancel?: () => void;
};

export const CreateKitchenForm = ({
  onSuccess,
  onCancel
}: Props) => {
  const { setSelectedKitchen } = useKitchenStore();
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

  const onSubmit = async (data: FormCreateKitchenDto) => {
    try {
      const newKitchen = await createKitchenAsync(data);
      setSelectedKitchen(newKitchen);
    } catch(err) {
      const message = getErrorMessage(err);
      toast.error(message)
    }
  };

  return (
    <Form {...form}>
      <form className="space-y-4" onSubmit={form.handleSubmit(onSubmit)}>
        <FormField
          name="name"
          control={form.control}
          render={({ field }) => (
            <FormItem>
              <FormLabel>Nome</FormLabel>
              <FormControl>
                <Input {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          name="description"
          control={form.control}
          render={({ field }) => (
            <FormItem>
              <FormLabel>Descriçao</FormLabel>
              <FormControl>
                <Textarea
                  {...field}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <div className="flex justify-between gap-x-2">
          {onCancel && (
            <Button
              variant="ghost"
              disabled={isPending}
              type="button"
              onClick={() => onCancel()}
            >
              Cancelar
            </Button>
          )}
          <Button disabled={isPending} type="submit">
            Criar
          </Button>
        </div>
      </form>
    </Form>
  );
}