import { TrendingUp, TrendingDown } from 'lucide-react';
import { useWidgetData } from '@/hooks/use-widget-data';

interface KpiCardWidgetProps {
  widgetId: string;
}

interface KpiData {
  value: number;
  previousValue: number;
  changePercent: number;
  label: string;
  isPositive: boolean;
}

export function KpiCardWidget({ widgetId }: KpiCardWidgetProps) {
  const kpi = useWidgetData<KpiData>(widgetId, 'KpiCard');

  return (
    <div className="flex h-full flex-col items-center justify-center gap-2">
      <p className="text-xs font-medium uppercase tracking-wider text-muted-foreground">
        {kpi.label}
      </p>
      <p className="text-4xl font-bold">{kpi.value.toLocaleString('es-ES')}</p>
      <div
        className={`flex items-center gap-1 text-sm ${kpi.isPositive ? 'text-success' : 'text-destructive'}`}
      >
        {kpi.isPositive ? (
          <TrendingUp className="h-4 w-4" />
        ) : (
          <TrendingDown className="h-4 w-4" />
        )}
        <span>{kpi.isPositive ? '+' : ''}{kpi.changePercent}%</span>
        <span className="text-muted-foreground">vs anterior</span>
      </div>
    </div>
  );
}
