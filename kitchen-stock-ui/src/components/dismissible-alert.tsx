"use client";

import { useState } from "react";
import { Alert, AlertTitle, AlertDescription } from "@/components/ui/alert";
import { X } from "lucide-react";
import { Button } from "@/components/ui/button";

export function DismissibleAlert() {
  const [open, setOpen] = useState(true);

  if (!open) return null;

  return (
    <Alert className="relative">
      <div className="flex flex-col gap-1">
        <AlertTitle>Aviso</AlertTitle>
        <AlertDescription>
          Este é um alerta que você pode fechar manualmente.
        </AlertDescription>
      </div>

      {/* Close button */}
      <Button
        variant="ghost"
        size="icon"
        className="absolute right-2 top-2 h-5 w-5"
        onClick={() => setOpen(false)}
      >
        <X className="h-4 w-4" />
      </Button>
    </Alert>
  );
}