"use client";

import { useState } from "react";
import Link from "next/link";
import Image from "next/image";
import { useRouter } from "next/navigation";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Loader2Icon, OctagonAlertIcon } from "lucide-react";

import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { AlertDescription } from "@/components/ui/alert";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { PasswordInput } from "@/components/password-input";
import { DismissibleAlert } from "@/components/dismissible-alert";
import { getErrorMessage } from "@/lib/api-client";
import { authLoginSchema, FormLoginDto } from "@/modules/auth/api/types";
import { useAuthLogin } from "@/modules/auth/hooks/mutations/use-auth-login";

import { APP_ROUTES } from "@/app-routes";

export const SignInView = () => {
  const router = useRouter();
  const [error, setError] = useState<string | null>(null);
  const { isPending, mutateAsync } = useAuthLogin();

  const form = useForm<FormLoginDto>({
    resolver: zodResolver(authLoginSchema),
    defaultValues: {
      email: "",
      password: "",
    },
  });

  const onSubmit = async (data: FormLoginDto) => {
    setError(null);
    
    try {
      await mutateAsync(data);
      router.push(APP_ROUTES.APP.HOME);
    } catch(err) {
      const message = getErrorMessage(err);
      setError(message);
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
                            href={APP_ROUTES.AUTH.FORGOT_PASSWORD}
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
                {!!error && (
                  <DismissibleAlert 
                    className="bg-destructive/10 border-none"
                    icon={OctagonAlertIcon}
                    iconClassName="!text-destructive"
                  >
                    <AlertDescription>{error}</AlertDescription>
                  </DismissibleAlert>
                )}
                <Button type="submit" className="w-full" disabled={isPending}>
                  {isPending 
                    ? (<Loader2Icon className="animate-spin" />)
                    : "Entrar"}
                </Button>
                <div className="text-center text-sm">
                  Não tem uma conta?{" "}
                  <Link prefetch href={APP_ROUTES.AUTH.SIGN_UP} className="underline underline-offset-4">
                    Cadastre-se
                  </Link>
                </div>
              </div>
            </form>
          </Form>
          <div className="bg-muted relative hidden md:block">
            <Image
              src="/placeholder.svg"
              alt="Image"
              className="absolute inset-0 h-full w-full object-cover dark:brightness-[0.2] dark:grayscale"
              fill
              priority={true}
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