"use client";

import Link from "next/link";
import { format } from "date-fns";
import { ptBR } from "date-fns/locale";
import {
  ChevronRightIcon,
  TrashIcon,
  PencilIcon,
  MoreVerticalIcon,
  AlertTriangle,
  Package,
  DollarSign,
  TrendingUp,
  Calendar,
  ArrowUpIcon,
  ArrowDownIcon,
  AlertCircleIcon,
  RotateCcwIcon,
  UndoIcon,
  ShoppingCartIcon,
  ChefHatIcon,
} from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuTrigger,
  DropdownMenuItem,
  DropdownMenuContent,
} from "@/components/ui/dropdown-menu";
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbSeparator,
} from "@/components/ui/breadcrumb";
import { Card, CardContent } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { APP_ROUTES } from "@/app-routes";
import { cn } from "@/lib/utils";

// Simulação: depois você pode buscar de API ou carregar via props
const ingredient = {
  id: "1",
  name: "Manteiga sem Sal",
  description: "Usada para bolos e massas",
  unitSymbol: "kg",
  currentStock: 12,
  minimumStock: 5,
  isLowStock: true,
  lastUnitPrice: 18.5,
  averageUnitPrice: 17.3,
  createdAt: "2024-07-12T12:00:00Z",
};

// Tipos de movimentação
enum StockMovementType {
  Purchase = 1,
  Usage = 2,
  Waste = 3,
  Adjustment = 4,
  Return = 5,
}

interface StockMovement {
  id: string;
  ingredientId: string;
  movementType: StockMovementType;
  quantity: number;
  unitPrice?: number;
  supplierId?: string;
  supplierName?: string;
  reason: string;
  movementDate: string;
  reference: string;
  totalValue?: number;
}

// Mock de histórico
const stockMovements: StockMovement[] = [
  {
    id: "1",
    ingredientId: "1",
    movementType: StockMovementType.Purchase,
    quantity: 20,
    unitPrice: 18.5,
    supplierName: "Fornecedor ABC",
    reason: "Reposição de estoque",
    movementDate: "2025-09-28T10:30:00Z",
    reference: "NF-12345",
    totalValue: 370,
  },
  {
    id: "2",
    ingredientId: "1",
    movementType: StockMovementType.Usage,
    quantity: -5,
    reason: "Usado em receita: Bolo de Chocolate",
    movementDate: "2025-09-27T14:20:00Z",
    reference: "PROD-456",
  },
  {
    id: "3",
    ingredientId: "1",
    movementType: StockMovementType.Waste,
    quantity: -2,
    reason: "Vencimento",
    movementDate: "2025-09-26T09:15:00Z",
    reference: "",
  },
  {
    id: "4",
    ingredientId: "1",
    movementType: StockMovementType.Adjustment,
    quantity: -1,
    reason: "Ajuste de inventário",
    movementDate: "2025-09-25T16:00:00Z",
    reference: "INV-001",
  },
  {
    id: "5",
    ingredientId: "1",
    movementType: StockMovementType.Purchase,
    quantity: 15,
    unitPrice: 17.8,
    supplierName: "Distribuidora XYZ",
    reason: "Compra programada",
    movementDate: "2025-09-20T11:00:00Z",
    reference: "NF-11223",
    totalValue: 267,
  },
];

// Configuração visual dos tipos de movimentação
const movementConfig = {
  [StockMovementType.Purchase]: {
    label: "Compra",
    icon: ShoppingCartIcon,
    color: "text-green-600",
    bgColor: "bg-green-50",
    borderColor: "border-green-200",
  },
  [StockMovementType.Usage]: {
    label: "Uso",
    icon: ChefHatIcon,
    color: "text-blue-600",
    bgColor: "bg-blue-50",
    borderColor: "border-blue-200",
  },
  [StockMovementType.Waste]: {
    label: "Desperdício",
    icon: AlertCircleIcon,
    color: "text-red-600",
    bgColor: "bg-red-50",
    borderColor: "border-red-200",
  },
  [StockMovementType.Adjustment]: {
    label: "Ajuste",
    icon: RotateCcwIcon,
    color: "text-orange-600",
    bgColor: "bg-orange-50",
    borderColor: "border-orange-200",
  },
  [StockMovementType.Return]: {
    label: "Devolução",
    icon: UndoIcon,
    color: "text-purple-600",
    bgColor: "bg-purple-50",
    borderColor: "border-purple-200",
  },
};

interface Props {
  ingredientId: string;
}

export const IngredientDetailView = ({ ingredientId }: Props) => {
  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <Breadcrumb>
          <BreadcrumbList>
            <BreadcrumbItem>
              <BreadcrumbLink asChild className="font-medium">
                <Link href={APP_ROUTES.APP.INGREDIENTS}>Meus Ingredientes</Link>
              </BreadcrumbLink>
            </BreadcrumbItem>
            <BreadcrumbSeparator>
              <ChevronRightIcon className="size-4" />
            </BreadcrumbSeparator>
            <BreadcrumbItem>
              <BreadcrumbLink asChild className="font-medium text-foreground">
                <Link href={`${APP_ROUTES.APP.INGREDIENTS}/${ingredientId}`}>
                  {ingredient.name}
                </Link>
              </BreadcrumbLink>
            </BreadcrumbItem>
          </BreadcrumbList>
        </Breadcrumb>

        {/* Menu de ações */}
        <DropdownMenu modal={false}>
          <DropdownMenuTrigger asChild>
            <Button variant="ghost" size="icon">
              <MoreVerticalIcon className="size-4" />
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end">
            <DropdownMenuItem onClick={() => {}}>
              <PencilIcon className="size-4 mr-2" />
              Editar
            </DropdownMenuItem>
            <DropdownMenuItem onClick={() => {}}>
              <TrashIcon className="size-4 mr-2" />
              Remover
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>

      {/* Nome + descrição */}
      <div>
        <h1 className="text-2xl font-bold capitalize">{ingredient.name}</h1>
        <p className="text-muted-foreground mt-1 capitalize">
          {ingredient.description}
        </p>
      </div>

      {/* Cards de informações principais */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
        {/* Estoque - Card colorido com status */}
        <Card
          className={cn(
            "border-2",
            ingredient.isLowStock
              ? "bg-yellow-50 border-yellow-200"
              : "bg-green-50 border-green-200"
          )}
        >
          <CardContent className="p-4 h-full flex items-center">
            <div className="flex items-center justify-between gap-4 w-full">
              <div className="flex items-center gap-2 min-w-0">
                {ingredient.isLowStock ? (
                  <AlertTriangle className="h-5 w-5 text-yellow-700 shrink-0" />
                ) : (
                  <Package className="h-5 w-5 text-green-700 shrink-0" />
                )}
                <span
                  className={cn(
                    "text-sm font-semibold whitespace-nowrap",
                    ingredient.isLowStock ? "text-yellow-900" : "text-green-900"
                  )}
                >
                  Estoque {ingredient.isLowStock ? "Baixo" : "OK"}
                </span>
              </div>
              <div className="text-right shrink-0">
                <div
                  className={cn(
                    "font-bold text-base whitespace-nowrap",
                    ingredient.isLowStock ? "text-yellow-900" : "text-green-900"
                  )}
                >
                  {ingredient.currentStock} {ingredient.unitSymbol}
                </div>
                <div
                  className={cn(
                    "text-xs font-medium whitespace-nowrap",
                    ingredient.isLowStock ? "text-yellow-700" : "text-green-700"
                  )}
                >
                  Mín: {ingredient.minimumStock} {ingredient.unitSymbol}
                </div>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Último preço */}
        <Card>
          <CardContent className="p-4 h-full flex items-center">
            <div className="flex items-center justify-between gap-4 w-full">
              <div className="flex items-center gap-2 min-w-0">
                <DollarSign className="h-5 w-5 text-muted-foreground shrink-0" />
                <span className="text-sm font-medium whitespace-nowrap">Último Preço</span>
              </div>
              <div className="font-semibold shrink-0">
                R$ {ingredient.lastUnitPrice.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Preço médio */}
        <Card>
          <CardContent className="p-4 h-full flex items-center">
            <div className="flex items-center justify-between gap-4 w-full">
              <div className="flex items-center gap-2 min-w-0">
                <TrendingUp className="h-5 w-5 text-muted-foreground shrink-0" />
                <span className="text-sm font-medium whitespace-nowrap">Preço Médio</span>
              </div>
              <div className="font-semibold shrink-0">
                R$ {ingredient.averageUnitPrice.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Status + metadados */}
      <div className="flex items-center justify-end gap-2 text-sm text-muted-foreground">
        <Calendar className="h-4 w-4" />
        <span>
          Adicionado em{" "}
          {format(new Date(ingredient.createdAt), "dd/MM/yyyy", { locale: ptBR })}
        </span>
      </div>

      {/* Histórico de Movimentações */}
      <div className="space-y-4">
        <div className="flex items-center justify-between">
          <div>
            <h2 className="text-lg font-semibold">Histórico de Movimentações</h2>
            <p className="text-sm text-muted-foreground">
              Últimas {stockMovements.length} movimentações de estoque
            </p>
          </div>
          <Button variant="outline" size="sm">
            Ver todas
          </Button>
        </div>

        {/* Lista de movimentações */}
        <div className="space-y-3">
          {stockMovements.map((movement) => {
            const config = movementConfig[movement.movementType];
            const Icon = config.icon;
            const isPositive = movement.quantity > 0;

            return (
              <Card key={movement.id}>
                <CardContent className="p-4">
                  <div className="flex items-start gap-4">
                    {/* Ícone do tipo de movimentação */}
                    <div
                      className={cn(
                        "p-2 rounded-lg shrink-0",
                        config.bgColor,
                        config.borderColor,
                        "border"
                      )}
                    >
                      <Icon className={cn("h-5 w-5", config.color)} />
                    </div>

                    {/* Conteúdo principal */}
                    <div className="flex-1 min-w-0 space-y-2">
                      {/* Header: Tipo + Data */}
                      <div className="flex items-start justify-between gap-3">
                        <div className="flex-1 min-w-0">
                          <div className="flex items-center gap-2 flex-wrap">
                            <span className="font-semibold text-sm">
                              {config.label}
                            </span>
                            {movement.reference && (
                              <Badge variant="outline" className="text-xs">
                                {movement.reference}
                              </Badge>
                            )}
                          </div>
                          <p className="text-sm text-muted-foreground mt-0.5">
                            {movement.reason}
                          </p>
                        </div>

                        {/* Quantidade */}
                        <div className="text-right shrink-0">
                          <div
                            className={cn(
                              "font-semibold text-sm flex items-center gap-1 justify-end",
                              isPositive ? "text-green-600" : "text-red-600"
                            )}
                          >
                            {isPositive ? (
                              <ArrowUpIcon className="h-3 w-3" />
                            ) : (
                              <ArrowDownIcon className="h-3 w-3" />
                            )}
                            {Math.abs(movement.quantity)} {ingredient.unitSymbol}
                          </div>
                        </div>
                      </div>

                      {/* Footer: Fornecedor, Preço, Data */}
                      <div className="flex items-center justify-between gap-3 text-xs text-muted-foreground flex-wrap">
                        <div className="flex items-center gap-3">
                          {movement.supplierName && (
                            <span>📦 {movement.supplierName}</span>
                          )}
                          {movement.unitPrice && (
                            <span>
                              💰 R$ {movement.unitPrice.toLocaleString('pt-BR', { minimumFractionDigits: 2 })} / {ingredient.unitSymbol}
                            </span>
                          )}
                          {movement.totalValue && (
                            <span className="font-medium">
                              Total: R$ {movement.totalValue.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                            </span>
                          )}
                        </div>
                        <span>
                          {format(new Date(movement.movementDate), "dd/MM/yyyy 'às' HH:mm", { locale: ptBR })}
                        </span>
                      </div>
                    </div>
                  </div>
                </CardContent>
              </Card>
            );
          })}
        </div>

        {/* Empty state (caso não tenha movimentações) */}
        {stockMovements.length === 0 && (
          <Card>
            <CardContent className="p-8 text-center">
              <Package className="h-12 w-12 text-muted-foreground mx-auto mb-3 opacity-50" />
              <h3 className="font-semibold mb-1">Nenhuma movimentação ainda</h3>
              <p className="text-sm text-muted-foreground">
                O histórico de movimentações aparecerá aqui assim que houver entradas ou saídas de estoque.
              </p>
            </CardContent>
          </Card>
        )}
      </div>
    </div>
  );
};