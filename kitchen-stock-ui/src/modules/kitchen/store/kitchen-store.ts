import { create } from "zustand";
import { Kitchen } from "@/modules/kitchen/api/types";
import { persist } from "zustand/middleware";

interface KitchenState {
  selectedKitchen: Kitchen | null;
  setSelectedKitchen: (k: Kitchen) => void;
  hasSelectedKitchen: () => boolean;
}

export const useKitchenStore = create<KitchenState>()(
  persist(
    (set, get) => ({
      selectedKitchen: null,
      setSelectedKitchen: (selectedKitchen) => set({ selectedKitchen }),
      hasSelectedKitchen: () => get().selectedKitchen != null,
    }),
    {
      name: "kitchen-storage",
    }
  )
);
