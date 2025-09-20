"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Loader2Icon, OctagonAlertIcon } from "lucide-react";

import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Alert, AlertTitle } from "@/components/ui/alert";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { PasswordInput } from "@/components/password-input";
import Link from "next/link";
import { authLoginSchema, LoginDto } from "@/modules/auth/api/types";
import { AUTH_API_ROUTES } from "@/modules/auth/api/api-routes";
import { useAuthLogin } from "@/modules/auth/hooks/mutations/useAuthLogin";
import { DismissibleAlert } from "@/components/dismissible-alert";

export const SignInView = () => {
  const router = useRouter();
  const [error, setError] = useState<string | null>(null);
  const { isPending, mutateAsync, mutate } = useAuthLogin();

  const form = useForm<LoginDto>({
    resolver: zodResolver(authLoginSchema),
    defaultValues: {
      email: "",
      password: "",
    },
  });

  const onSubmit = async (data: LoginDto) => {
    setError(null);
    
    try {
      await mutateAsync(data);
    } catch(err) {
      console.log(err);
      setError("failed");
    }   
  };
  
  return (
    <div className="flex flex-col gap-6">
      <Card className="overflow-hidden p-0">
        <CardContent className="grid p-0 md:grid-cols-2">
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="p-6 md:p-8">
              <div className="flex flex-col gap-6">
                <div className="flex flex-col items-center text-center">
                  <h1 className="text-2xl font-bold">Seja bem-vindo!</h1>
                  <p className="text-muted-foreground text-balance">
                    Acesse a sua conta
                  </p>
                </div>
                <div className="grid gap-3">
                  <FormField
                    control={form.control}
                    name="email"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Email</FormLabel>
                        <FormControl>
                          <Input
                            type="email"
                            placeholder="m@example.com"
                            {...field}
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>
                <div className="grid gap-3">
                  <FormField
                    control={form.control}
                    name="password"
                    render={({ field }) => (
                      <FormItem>
                        <div className="flex items-center">
                          <FormLabel>Senha</FormLabel>
                          <Link
                            href={AUTH_API_ROUTES.FORGOT_PASSWORD}
                            className="ml-auto text-sm underline-offset-2 hover:underline"
                          >
                            Esqueceu a senha?
                          </Link>
                        </div>
                        <FormControl>
                          <PasswordInput {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>
                {!!error && (<DismissibleAlert />
                  // <Alert className="bg-destructive/10 border-none">
                  //   <OctagonAlertIcon className="h-4 w-4 !text-destructive" />
                  //   <AlertTitle>{error}</AlertTitle>
                  // </Alert>
                )}
                <Button type="submit" className="w-full" disabled={isPending}>
                  {isPending 
                    ? (<Loader2Icon className="animate-spin" />)
                    : "Entrar"}
                </Button>
                <div className="text-center text-sm">
                  Não tem uma conta?{" "}
                  <Link href={AUTH_API_ROUTES.REGISTER} className="underline underline-offset-4">
                    Cadastre-se
                  </Link>
                </div>
              </div>
            </form>
          </Form>
          <div className="bg-muted relative hidden md:block">
            <img
              src="/placeholder.svg"
              alt="Image"
              className="absolute inset-0 h-full w-full object-cover dark:brightness-[0.2] dark:grayscale"
            />
          </div>
        </CardContent>
      </Card>
      <div className="text-muted-foreground *:[a]:hover:text-primary text-center text-xs text-balance *:[a]:underline *:[a]:underline-offset-4">
        Ao clicar no botão entrar, você concorda com nossos <a href="#">Termos de Serviço</a>{" "}
        e <a href="#">Política de Privacidade</a>.
      </div>
    </div>
  );
}