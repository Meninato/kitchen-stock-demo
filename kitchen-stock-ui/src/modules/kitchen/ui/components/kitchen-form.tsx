import { UseFormReturn } from "react-hook-form";

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

interface KitchenFormProps {
  form: UseFormReturn<{name: string; description?: string;}>;
  isSubmitting: boolean;
  onSubmit: (data: {name: string; description?: string;}) => void;
  onCancel?: () => void;
  submitLabel?: string;
}

export function KitchenForm({
  form,
  isSubmitting,
  onSubmit,
  onCancel,
  submitLabel = "Salvar",
}: KitchenFormProps) {
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
              <FormLabel>Descrição</FormLabel>
              <FormControl>
                <Textarea {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <div className="flex justify-between gap-x-2">
          {onCancel && (
            <Button
              variant="ghost"
              disabled={isSubmitting}
              type="button"
              onClick={onCancel}
            >
              Cancelar
            </Button>
          )}
          <Button disabled={isSubmitting} type="submit">
            {submitLabel}
          </Button>
        </div>
      </form>
    </Form>
  );
}