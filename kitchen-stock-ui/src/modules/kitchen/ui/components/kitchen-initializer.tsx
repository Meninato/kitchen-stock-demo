'use client'

import { useEffect, useState } from "react"
import { usePathname, useRouter } from "next/navigation"
import { useQueryClient } from "@tanstack/react-query"

import { useKitchenStore } from "@/modules/kitchen/store/kitchen-store"
import { Loader } from "@/components/loader"
import { Kitchen } from "@/modules/kitchen/api/types"
import { useManyKitchens } from "@/modules/kitchen/hooks/queries/use-many-kitchens"
import { kitchenQueryKeys } from "@/modules/kitchen/hooks/queries/kitchen-query-keys"
import { APP_ROUTES } from "@/app-routes"

export default function KitchenInitializer({ children }: { children: React.ReactNode }) {
  const [initState, setInitState] = useState<'loading' | 'ready' | 'redirect'>('loading')
  const pathname = usePathname()
  const router = useRouter()
  const { selectedKitchen, setSelectedKitchen } = useKitchenStore()
  const queryClient = useQueryClient()

  const cachedKitchens = queryClient.getQueryData(kitchenQueryKeys.lists()) as Kitchen[] | undefined

  // Fetch kitchens se não estão no cache
  const { 
    data: kitchens, 
    isLoading: isKitchensLoading,
    isError: isKitchensError
  } = useManyKitchens({
    enabled: !cachedKitchens
  })

  useEffect(() => {

    // Determina as kitchens disponíveis (cache ou API)
    const availableKitchens = cachedKitchens || kitchens || [];

    console.log('Initializer state:', {
      pathname,
      selectedKitchen: selectedKitchen?.id,
      availableKitchens: availableKitchens.length,
      isKitchensLoading,
      cachedKitchens: !!cachedKitchens
    })

    // Se já está pronto, não faz nada
    if (initState === 'ready') return

    // Se está na página de seleção de kitchen, deixa passar
    if (pathname === APP_ROUTES.APP.KITCHEN_SELECTION) {
      setInitState('ready')
      return
    }

    // Se ainda está carregando dados, aguarda
    if (isKitchensLoading && !cachedKitchens) {
      setInitState('loading')
      return
    }

    // Se deu erro ao carregar e não tem cache, redireciona
    if (isKitchensError && !cachedKitchens) {
      console.log('Error loading kitchens, redirecting...')
      setInitState('redirect')
      router.replace(APP_ROUTES.APP.KITCHEN_SELECTION)
      return
    }

    // Se não tem kitchens disponíveis, redireciona para criação
    if (availableKitchens.length === 0) {
      console.log('No kitchens available, redirecting to kitchen-selection...')
      setInitState('redirect')
      router.replace(APP_ROUTES.APP.KITCHEN_SELECTION)
      return
    }

    // Se tem kitchen selecionada, está pronto
    if (selectedKitchen) {
      console.log('Kitchen already selected, ready!')
      setInitState('ready')
      return
    }

    // Se não tem kitchen selecionada mas tem kitchens disponíveis, seleciona a primeira
    if (availableKitchens.length > 0) {
      console.log('Auto-selecting first available kitchen...')
      setSelectedKitchen(availableKitchens[0])
      setInitState('ready')
      return
    }

    // Fallback - se chegou aqui, algo deu errado
    console.log('Fallback: redirecting to kitchen-selection...')
    setInitState('redirect')
    router.replace(APP_ROUTES.APP.KITCHEN_SELECTION)

  }, [
    pathname, 
    selectedKitchen, 
    isKitchensLoading, 
    isKitchensError,
    cachedKitchens,
    initState,
    kitchens,
    setSelectedKitchen, 
    router
  ])

  // Se está redirecionando ou carregando, mostra loader
  if (initState === 'loading' || initState === 'redirect') {
    return <Loader />
  }

  // Se chegou aqui, está pronto para renderizar
  return <>{children}</>
}