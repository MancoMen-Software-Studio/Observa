import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend } from 'recharts';
import { ChartContainer } from '../shared/chart-container';
import { useWidgetData } from '@/hooks/use-widget-data';
import { chartColors, chartTooltipStyle, chartTooltipItemStyle, chartGridStyle, chartAxisStyle } from '@/lib/chart-theme';

interface BarChartWidgetProps {
  widgetId: string;
}

interface BarDataPoint {
  name: string;
  ventas: number;
  retornos: number;
}

export function BarChartWidget({ widgetId }: BarChartWidgetProps) {
  const data = useWidgetData<BarDataPoint[]>(widgetId, 'BarChart');

  return (
    <ChartContainer>
      <BarChart data={data} margin={{ top: 5, right: 20, left: 0, bottom: 5 }}>
        <CartesianGrid {...chartGridStyle} />
        <XAxis dataKey="name" {...chartAxisStyle} />
        <YAxis {...chartAxisStyle} />
        <Tooltip contentStyle={chartTooltipStyle} itemStyle={chartTooltipItemStyle} />
        <Legend wrapperStyle={{ fontSize: 11 }} />
        <Bar dataKey="ventas" fill={chartColors[0]} radius={[4, 4, 0, 0]} name="Ventas" isAnimationActive={false} />
        <Bar dataKey="retornos" fill={chartColors[5]} radius={[4, 4, 0, 0]} name="Retornos" isAnimationActive={false} />
      </BarChart>
    </ChartContainer>
  );
}
