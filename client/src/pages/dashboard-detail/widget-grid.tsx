import type { WidgetResponse } from '@/types/api';
import { WidgetCard } from './widget-card';

interface WidgetGridProps {
  widgets: WidgetResponse[];
  onRemoveWidget: (widgetId: string) => void;
}

export function WidgetGrid({ widgets, onRemoveWidget }: WidgetGridProps) {
  if (widgets.length === 0) {
    return null;
  }

  const maxRow = Math.max(...widgets.map((w) => w.row + w.height));

  return (
    <div
      className="grid gap-4"
      style={{
        gridTemplateColumns: 'repeat(12, minmax(0, 1fr))',
        gridTemplateRows: `repeat(${maxRow}, minmax(120px, auto))`,
      }}
    >
      {widgets.map((widget) => (
        <WidgetCard key={widget.id} widget={widget} onRemove={onRemoveWidget} />
      ))}
    </div>
  );
}
