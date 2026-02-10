import { PieChart, Pie, Cell } from 'recharts';
import { ChartContainer } from '../shared/chart-container';
import { useWidgetData } from '@/hooks/use-widget-data';

interface GaugeWidgetProps {
  widgetId: string;
}

interface GaugeData {
  value: number;
  min: number;
  max: number;
  label: string;
}

function getGaugeColor(value: number): string {
  if (value < 40) {
    return '#ef4444';
  }
  if (value < 70) {
    return '#f59e0b';
  }
  return '#22c55e';
}

export function GaugeWidget({ widgetId }: GaugeWidgetProps) {
  const gauge = useWidgetData<GaugeData>(widgetId, 'Gauge');
  const color = getGaugeColor(gauge.value);

  const data = [
    { value: gauge.value },
    { value: gauge.max - gauge.value },
  ];

  return (
    <div className="flex h-full flex-col items-center justify-center">
      <div className="relative h-[80%] w-full">
        <ChartContainer>
          <PieChart>
            <Pie
              data={data}
              cx="50%"
              cy="60%"
              startAngle={180}
              endAngle={0}
              innerRadius="60%"
              outerRadius="85%"
              dataKey="value"
              stroke="none"
              isAnimationActive={false}
            >
              <Cell fill={color} />
              <Cell fill="#27272a" />
            </Pie>
          </PieChart>
        </ChartContainer>
        <div className="absolute inset-0 flex flex-col items-center justify-center pt-4">
          <span className="text-3xl font-bold" style={{ color }}>
            {gauge.value}%
          </span>
          <span className="text-xs text-muted-foreground">{gauge.label}</span>
        </div>
      </div>
    </div>
  );
}
