import { z } from "zod";

export interface AuthUser {
  id: string;
  name: string;
  email: string;
  plan: string;
  current_kitchens: string;
  created_at: string;
}

export const authLoginSchema = z.object({
  email: z.email({ error: "Informe um email" }),
  password: z.string().min(1, { message: "Informe uma senha" }),
});

export const authRegisterSchema = z
  .object({
    name: z.string().min(1, { message: "Informe o seu nome" }),
    email: z.email({ error: "Informe um email" }),
    password: z.string().min(1, { message: "Informe uma senha" }),
    confirmPassword: z.string().min(1, { message: "Confirme a senha" }),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: "As senhas são difrentes",
    path: ["confirmPassword"],
  });

export type FormLoginDto = z.infer<typeof authLoginSchema>;
export type FormRegisterDto = z.infer<typeof authRegisterSchema>;

export type ApiLoginDto = FormLoginDto;
export type ApiRegisterDto = Omit<FormRegisterDto, "confirmPassword">;
