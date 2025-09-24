import { useEffect } from "react";

/**
 * Runs an effect only once (on mount).
 * Ignores React Hooks exhaustive-deps lint rule safely.
 */
export function useMount(effect: () => void | (() => void)) {
  // eslint-disable-next-line react-hooks/exhaustive-deps
  useEffect(effect, []);
}
