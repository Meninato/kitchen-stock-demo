'use client';

import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { NuqsAdapter } from 'nuqs/adapters/next/app'
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { useState } from 'react';
import { Toaster } from 'sonner';
import { AppProvider } from '@/modules/app/providers/app-provider';

export function Providers({ children }: { children: React.ReactNode }) {
  const [queryClient] = useState(
    () =>
      new QueryClient({
        defaultOptions: {
          queries: {
            staleTime: 60 * 1000, // 1 minute
            gcTime: 5 * 60 * 1000, // 5 minutes
            retry: (failureCount, error: unknown) => {
              const err = error as { statusCode?: number };

              // Don't retry on 4xx errors except 429
              if (err?.statusCode && err?.statusCode >= 400 && err?.statusCode < 500 && err?.statusCode !== 429) {
                return false;
              }
              // Retry up to 3 times
              return failureCount < 3;
            },
            refetchOnWindowFocus: false,
          },
          mutations: {
            retry: false,
          },
        },
      })
  );

  return (
    <QueryClientProvider client={queryClient}>
        <NuqsAdapter>
          {children}
        </NuqsAdapter>
        <Toaster position="top-right" richColors />
        <ReactQueryDevtools initialIsOpen={false} />
      </QueryClientProvider>
  );
}