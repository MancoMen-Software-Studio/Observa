import { useCallback, useSyncExternalStore } from 'react';

export type ToastVariant = 'default' | 'destructive' | 'success';

export interface Toast {
  id: string;
  title: string;
  description?: string;
  variant?: ToastVariant;
}

let toasts: Toast[] = [];
let listeners: Array<() => void> = [];
let counter = 0;

function emitChange() {
  for (const listener of listeners) {
    listener();
  }
}

function subscribe(listener: () => void) {
  listeners = [...listeners, listener];
  return () => {
    listeners = listeners.filter((l) => l !== listener);
  };
}

function getSnapshot() {
  return toasts;
}

export function toast({ title, description, variant = 'default' }: Omit<Toast, 'id'>) {
  const id = String(++counter);
  toasts = [...toasts, { id, title, description, variant }];
  emitChange();

  setTimeout(() => {
    dismissToast(id);
  }, 5000);
}

export function dismissToast(id: string) {
  toasts = toasts.filter((t) => t.id !== id);
  emitChange();
}

export function useToasts() {
  const currentToasts = useSyncExternalStore(subscribe, getSnapshot);
  const dismiss = useCallback((id: string) => dismissToast(id), []);
  return { toasts: currentToasts, dismiss };
}
