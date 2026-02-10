import { create } from 'zustand';

interface LineChartPoint {
  name: string;
  valor: number;
  objetivo: number;
}

interface WidgetDataStore {
  data: Record<string, unknown>;
  setBatchWidgetData: (widgets: Record<string, unknown>) => void;
  appendLineChartPoint: (widgetId: string, point: LineChartPoint) => void;
}

const MAX_LINE_POINTS = 30;

export const useWidgetDataStore = create<WidgetDataStore>()((set) => ({
  data: {},
  setBatchWidgetData: (widgets) =>
    set((state) => ({
      data: { ...state.data, ...widgets },
    })),
  appendLineChartPoint: (widgetId, point) =>
    set((state) => {
      const existing = (state.data[widgetId] as LineChartPoint[] | undefined) ?? [];
      const updated = [...existing, point].slice(-MAX_LINE_POINTS);
      return { data: { ...state.data, [widgetId]: updated } };
    }),
}));
