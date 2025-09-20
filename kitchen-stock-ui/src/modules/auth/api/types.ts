import { z } from "zod";

export const authLoginSchema = z.object({
  email: z.email({ error: "Informe um email" }),
  password: z.string().min(1, { message: "Informe uma senha" }),
});

export type LoginDto = z.infer<typeof authLoginSchema>;
