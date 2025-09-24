import { create } from "zustand";

import { Kitchen } from "@/modules/kitchen/api/types";

interface KitchenStore {
  selectedKitchen: Kitchen | null;
  setSelectedKitchen: (kitchen: Kitchen) => void;
  clearSelectedKitchen: () => void;
}

export const useKitchenStore = create<KitchenStore>((set) => ({
  selectedKitchen: null,
  setSelectedKitchen: (kitchen) => set({ selectedKitchen: kitchen }),
  clearSelectedKitchen: () => set({ selectedKitchen: null }),
}));
