"use client"

import { format } from "date-fns";
import { ColumnDef } from "@tanstack/react-table"
import {
  CornerDownRightIcon,
  AlertTriangleIcon,
  ForkKnifeCrossedIcon,
  LoaderIcon,
  TrendingUp,
  DollarSign,
  Package,
  AlertTriangle,
  Calendar
} from "lucide-react"

import { Badge } from "@/components/ui/badge"

import { cn } from "@/lib/utils";
import { Ingredient } from "@/modules/ingredients/api/types";
import { Card } from "@/components/ui/card";

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

export const columns: ColumnDef<Ingredient>[] = 
[
  {
    accessorKey: "name",
    header: "Ingrediente",
    cell: ({ row }) => {
      const ingredient = row.original;

      return (
        <div className="flex flex-col gap-y-1">
          <span className="font-semibold capitalize">
            {row.original.name}
          </span>
          <div className="flex items-center gap-x-2 text-sm text-muted-foreground">
            <CornerDownRightIcon className="size-3" />
            <span className="truncate max-w-[140px] capitalize">
              {ingredient.description || "Sem descrição"}
            </span>
            {/* <ForkKnifeCrossedIcon className="size-3 opacity-70" />
            {ingredient.createdAt && (
              <span>{format(ingredient.createdAt, "MMM d")}</span>
            )} */}
          </div>
          {/* <div className="flex items-center gap-1.5 pt-3">
            <Calendar className="h-3 w-3 text-muted-foreground" />
            <span className="text-xs text-muted-foreground">
              Adicionado em {format(new Date(ingredient.createdAt), "dd/MM/yyyy")}
            </span>
          </div> */}
        </div>
      );
    },
  },
];

interface IngredientCardProps {
  ingredient: Ingredient;
  onClick?: (ingredient: Ingredient) => void;
  onEdit?: (ingredient: Ingredient) => void;
  onDelete?: (ingredient: Ingredient) => void;
}

export function IngredientCard({ 
  ingredient, 
  onClick, 
  onEdit, 
  onDelete 
}: IngredientCardProps) {
  return (
    <Card 
      className="p-4 space-y-3 hover:shadow-md transition-shadow cursor-pointer"
      onClick={() => onClick?.(ingredient)}
    >
      {/* Header: Nome + Status */}
      <div className="flex items-start justify-between gap-3">
        <div className="flex-1 min-w-0">
          <h3 className="font-semibold text-base leading-tight capitalize truncate">
            {ingredient.name}
          </h3>
          <p className="text-sm text-muted-foreground mt-1 line-clamp-2 capitalize">
            {ingredient.description}
          </p>
        </div>
        
        {/* Status Badge */}
        <Badge
          variant="outline"
          className={cn(
            "shrink-0 gap-1.5",
            ingredient.isLowStock 
              ? "bg-yellow-50 text-yellow-700 border-yellow-200" 
              : "bg-green-50 text-green-700 border-green-200"
          )}
        >
          {ingredient.isLowStock ? (
            <AlertTriangle className="h-3 w-3" />
          ) : (
            <Package className="h-3 w-3" />
          )}
          {ingredient.isLowStock ? "Baixo" : "OK"}
        </Badge>
      </div>

      {/* Estoque */}
      <div className="flex items-center justify-between py-2 px-3 bg-muted/30 rounded-md">
        <div className="flex items-center gap-2">
          <Package className="h-4 w-4 text-muted-foreground" />
          <span className="text-sm font-medium">Estoque</span>
        </div>
        <div className="text-right">
          <div className="font-semibold">
            {ingredient.currentStock.toLocaleString('pt-BR')} {ingredient.unitSymbol}
          </div>
          <div className="text-xs text-muted-foreground">
            Mín: {ingredient.minimumStock} {ingredient.unitSymbol}
          </div>
        </div>
      </div>

      {/* Informações adicionais em grid */}
      <div className="grid grid-cols-2 gap-3 pt-1">
        {/* Preços */}
        <div className="space-y-1">
          <div className="flex items-center gap-1.5">
            <DollarSign className="h-3 w-3 text-muted-foreground" />
            <span className="text-xs font-medium text-muted-foreground">ÚLTIMO PREÇO</span>
          </div>
          <div className="font-semibold text-sm">
            R$ {ingredient.lastUnitPrice.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
          </div>
        </div>

        <div className="space-y-1">
          <div className="flex items-center gap-1.5">
            <TrendingUp className="h-3 w-3 text-muted-foreground" />
            <span className="text-xs font-medium text-muted-foreground">PREÇO MÉDIO</span>
          </div>
          <div className="font-semibold text-sm">
            R$ {ingredient.averageUnitPrice.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
          </div>
        </div>
      </div>

      {/* Footer: Data de criação */}
      <div className="flex items-center gap-1.5 pt-2 border-t border-border/50">
        <Calendar className="h-3 w-3 text-muted-foreground" />
        <span className="text-xs text-muted-foreground">
          Adicionado em {format(new Date(ingredient.createdAt), "dd/MM/yyyy")}
        </span>
      </div>
    </Card>
  );
}

export function IngredientCardCompact({ 
  ingredient, 
  onClick 
}: IngredientCardProps) {
  return (
    <Card
      className="p-3 hover:shadow-sm transition cursor-pointer"
      onClick={() => onClick?.(ingredient)}
    >
      <div className="flex items-center justify-between gap-3">
        {/* Nome + descrição */}
        <div className="flex-1 min-w-0">
          <h3 className="font-medium text-sm truncate capitalize">
            {ingredient.name}
          </h3>
          <p className="text-xs text-muted-foreground truncate max-w-[180px] capitalize">
            {ingredient.description + 'lorem ipusm hiaodjf padfaf 2wuienqndqn nsaidnaind aisdnasindai'}
          </p>
        </div>

        {/* Status + Estoque resumido */}
        <Badge
          variant="outline"
          className={cn(
            "shrink-0 gap-1.5 text-xs px-2 py-0.5",
            ingredient.isLowStock 
              ? "bg-yellow-50 text-yellow-700 border-yellow-200" 
              : "bg-green-50 text-green-700 border-green-200"
          )}
        >
          {ingredient.isLowStock ? (
            <AlertTriangle className="h-3 w-3" />
          ) : (
            <Package className="h-3 w-3" />
          )}
          {ingredient.currentStock} {ingredient.unitSymbol}
        </Badge>
                <Badge
          variant="outline"
          className={cn(
            "shrink-0 gap-1.5 text-xs px-2 py-0.5",
            ingredient.isLowStock 
              ? "bg-yellow-50 text-yellow-700 border-yellow-200" 
              : "bg-green-50 text-green-700 border-green-200"
          )}
        >
          {ingredient.isLowStock ? (
            <AlertTriangle className="h-3 w-3" />
          ) : (
            <Package className="h-3 w-3" />
          )}
          {ingredient.currentStock} {ingredient.unitSymbol}
        </Badge>
      </div>

      {/* Linha secundária: preços + data */}
      <div className="mt-2 flex items-center justify-between text-xs text-muted-foreground">
        <span>
          Últ: R$ {ingredient.lastUnitPrice.toFixed(2)} · 
          Méd: R$ {ingredient.averageUnitPrice.toFixed(2)}
        </span>
        <span>{format(new Date(ingredient.createdAt), "dd/MM/yy")}</span>
      </div>
    </Card>
  );
}