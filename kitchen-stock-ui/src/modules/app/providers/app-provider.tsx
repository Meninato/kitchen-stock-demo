'use client'

import { useEffect, useState } from 'react'
import { usePathname, useSearchParams } from 'next/navigation'
import { APP_ROUTES } from '@/app-routes' 
import { useKitchenStore } from '@/modules/kitchen/store/kitchen-store'
import { kitchenEndpoints } from '@/modules/kitchen/api/endpoint';
import { Loader } from '@/components/loader'

export const AppProvider = ({ children }: { children: React.ReactNode })  => {
  const [isLoading, setIsLoading] = useState(true) 
  const pathname = usePathname()
  const searchParams = useSearchParams()
  
  const { selectedKitchen, setSelectedKitchen } = useKitchenStore();

  useEffect(() => {
    if (!pathname.startsWith("/app") || pathname === APP_ROUTES.APP.KITCHEN_SELECTION) {
      setIsLoading(false);
      return;
    }

    if (selectedKitchen) {
       setIsLoading(false);
       return;
    }
    
    if (!isLoading) {
        setIsLoading(true);
    }
    
    const kitchenId = searchParams.get('kitchen');

    if (kitchenId) {
      kitchenEndpoints.getKitchen(kitchenId).then((k) => setSelectedKitchen(k));
      setIsLoading(false);
    } 

  }, [pathname, searchParams, selectedKitchen, setSelectedKitchen, isLoading]) // Adicionado currentKitchenId como dependência
  
  if (isLoading) {
    return (
      <Loader />
    )
  }
  
  return <>{children}</>
}