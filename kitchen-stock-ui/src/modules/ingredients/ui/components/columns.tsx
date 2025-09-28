"use client"

import { format } from "date-fns";
import { ColumnDef } from "@tanstack/react-table"
import {
  CornerDownRightIcon,
  AlertTriangleIcon,
  ForkKnifeCrossedIcon,
  LoaderIcon
} from "lucide-react"

import { Badge } from "@/components/ui/badge"

import { cn } from "@/lib/utils";
import { Ingredient } from "@/modules/ingredients/api/types";

const statusIconMap = {
  lowStock: AlertTriangleIcon,
};

const statusColorMap = {
  lowStock: "bg-yellow-500/20 text-yellow-800 border-yellow-800/5",
  // active: "bg-blue-500/20 text-blue-800 border-blue-800/5",
  // completed: "bg-emerald-500/20 text-emerald-800 border-emerald-800/5",
  // cancelled: "bg-rose-500/20 text-rose-800 border-rose-800/5",
  // processing: "bg-gray-300/20 text-gray-800 border-gray-800/5",
}

export const columns: ColumnDef<Ingredient>[] = [
  {
    accessorKey: "name",
    header: "Nome do Ingrediente",
    cell: ({ row }) => (
      <div className="flex flex-col gap-y-1">
        <span className="font-semibold capitalize">{row.original.name}</span>
        <div className="flex items-center gap-x-2">
          <div className="flex items-center gap-x-1">
            <CornerDownRightIcon className="size-3 text-muted-foreground" />
            <span className="text-sm text-muted-foreground max-w-[200px] truncate capitalize">
              {row.original.description}
            </span>
          </div>
          <ForkKnifeCrossedIcon className="size-4" />
          <span className="text-sm text-muted-foreground">
            {row.original.createdAt ? format(row.original.createdAt, "MMM d") : ""}
          </span>
        </div>
      </div>
    )
  },
  {
    accessorKey: "currentStock",
    header: "Saldo Atual",
    cell: ({row}) => {
      const Icon = row.original.isLowStock ? statusIconMap.lowStock : LoaderIcon;
      const styles = row.original.isLowStock ? statusColorMap.lowStock : "";

      return (
        <Badge
          variant="outline"
          className={cn(
            "capitalize [&>svg]:size-4 text-muted-foreground",
            styles
          )}
        >
          <Icon />
          {row.original.currentStock}
        </Badge>
      )
    }
  }
];
