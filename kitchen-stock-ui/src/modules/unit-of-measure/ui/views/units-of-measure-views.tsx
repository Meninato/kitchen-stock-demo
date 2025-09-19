"use client";

import { useUnitsOfMeasure } from "../../hooks/queries/useUnitsOfMeasure";

export const UnitsOfMeasureView = () => {
  const { data, isLoading, isError, error, refetch } = useUnitsOfMeasure();

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900"></div>
      </div>
    );
  }

  if (isError) {
    return (
      <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
        <p className="font-semibold">Error loading products</p>
        <p className="text-sm">{error?.message}</p>
        <button
          onClick={() => refetch()}
          className="mt-2 text-sm underline hover:no-underline"
        >
          Try again
        </button>
      </div>
    );
  }

  return (
    <div>
      {JSON.stringify(data, null, 2)}
      All units of measure
    </div>
  );
}