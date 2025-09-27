import { create } from "zustand";
import { Kitchen } from "@/modules/kitchen/api/types";
import { persist } from "zustand/middleware";

interface KitchenState {
  selectedKitchen: Kitchen | null;
  setSelectedKitchen: (k: Kitchen) => void;
}

export const useKitchenStore = create<KitchenState>()(
  persist(
    (set) => ({
      selectedKitchen: null,
      setSelectedKitchen: (selectedKitchen) => set({ selectedKitchen }),
    }),
    {
      name: "kitchen-storage",
    }
  )
);
