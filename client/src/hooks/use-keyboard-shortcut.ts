import { useEffect } from 'react';

interface ShortcutOptions {
  key: string;
  ctrl?: boolean;
  shift?: boolean;
  handler: () => void;
  enabled?: boolean;
}

export function useKeyboardShortcut({
  key,
  ctrl = false,
  shift = false,
  handler,
  enabled = true,
}: ShortcutOptions) {
  useEffect(() => {
    if (!enabled) {
      return;
    }

    function handleKeyDown(event: KeyboardEvent) {
      const ctrlMatch = ctrl ? event.ctrlKey || event.metaKey : true;
      const shiftMatch = shift ? event.shiftKey : true;

      if (ctrlMatch && shiftMatch && event.key.toLowerCase() === key.toLowerCase()) {
        event.preventDefault();
        handler();
      }
    }

    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, [key, ctrl, shift, handler, enabled]);
}
