"use client";

import { useState } from "react";
import { Alert } from "@/components/ui/alert";
import { X } from "lucide-react";
import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";

interface Props {
  icon?: React.ElementType;
  iconClassName?: string;
  children: React.ReactNode;
  className?: string;
}

export function DismissibleAlert({
  children,
  className,
  icon: Icon,
  iconClassName
}: Props) {
  const [open, setOpen] = useState(true);

  if (!open) return null;

  return (
    <Alert className={cn("relative flex items-center gap-2", className)}>
      {Icon && <Icon className={cn("h-4 w-4 shrink-0", iconClassName)} />}
      <div>
        {children}
      </div>

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