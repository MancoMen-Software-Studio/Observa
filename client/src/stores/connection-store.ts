import { create } from 'zustand';

export type ConnectionState = 'connected' | 'connecting' | 'disconnected';

interface ConnectionStore {
  state: ConnectionState;
  setState: (state: ConnectionState) => void;
}

export const useConnectionStore = create<ConnectionStore>()((set) => ({
  state: 'disconnected',
  setState: (state) => set({ state }),
}));
