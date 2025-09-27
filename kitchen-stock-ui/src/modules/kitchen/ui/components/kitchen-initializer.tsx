'use client'

import { useEffect, useState } from "react"
import { usePathname } from "next/navigation"
import { useQueryClient } from "@tanstack/react-query"

import { useKitchenStore } from "@/modules/kitchen/store/kitchen-store"
import { Loader } from "@/components/loader"
import { Kitchen } from "@/modules/kitchen/api/types"
import { useManyKitchens } from "@/modules/kitchen/hooks/queries/use-many-kitchens"
import { kitchenQueryKeys } from "../../hooks/queries/kitchen-query-keys"

export default function KitchenInitializer({ children }: { children: React.ReactNode }) {
  const [isReady, setIsReady] = useState(false)
  const pathname = usePathname()
  const { selectedKitchen, setSelectedKitchen } = useKitchenStore()
  const queryClient = useQueryClient();

  const cachedKitchens = queryClient.getQueryData(kitchenQueryKeys.lists()) as Kitchen[] | undefined;
  
  // Only fetch kitchens if not in cache and we need them
  const { 
    data: kitchens, 
    isLoading: isKitchensLoading 
  } = useManyKitchens({
    enabled: !cachedKitchens
  });

  useEffect(() => {
    if(isReady && selectedKitchen) return;

    console.log("SELECTED KITCHEN", selectedKitchen);
    console.log("CACHED KITCHENS", cachedKitchens);

    // Case 1: Coming from kitchen-selection - both store and cache should be ready
    if (selectedKitchen && cachedKitchens) {
      console.log("Already initialized with cache - ready!")
      setIsReady(true);
      return;
    }

    // Case 2: selectedKitchen exists but no cached kitchens data
    if (selectedKitchen && !cachedKitchens) {
      console.log("Store ready but cache empty - waiting for cache...")
      setIsReady(false);
      return;
    }

    // Case 3: no selectedKitchen - need to initialize store
    if (!selectedKitchen) {
      console.log("Initializing store from URL...")

      // Try to find kitchen in cache first
      if (cachedKitchens && cachedKitchens.length > 0) {
        setSelectedKitchen(cachedKitchens[0]);
        setIsReady(true);
        return;
      }

      // If not in cache, wait for API call or set partial kitchen
      if (kitchens) {
        setSelectedKitchen(kitchens[0]);
        setIsReady(true);
        return;
      }
    }

    // Default case - not ready yet
    setIsReady(false);
  }, [pathname, selectedKitchen, setSelectedKitchen, cachedKitchens, kitchens, isReady]);

  // Show loader while waiting for initialization or API calls
  if (!isReady || isKitchensLoading) {
    return <Loader texts={["wtf"]} />;
  }

  return <>{children}</>;
}