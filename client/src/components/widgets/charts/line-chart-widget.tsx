import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend } from 'recharts';
import { ChartContainer } from '../shared/chart-container';
import { useWidgetData } from '@/hooks/use-widget-data';
import { chartColors, chartTooltipStyle, chartTooltipItemStyle, chartGridStyle, chartAxisStyle } from '@/lib/chart-theme';

interface LineChartWidgetProps {
  widgetId: string;
}

interface LinePoint {
  name: string;
  valor: number;
  objetivo: number;
}

export function LineChartWidget({ widgetId }: LineChartWidgetProps) {
  const data = useWidgetData<LinePoint[]>(widgetId, 'LineChart');

  return (
    <ChartContainer>
      <LineChart data={data} margin={{ top: 5, right: 20, left: 0, bottom: 5 }}>
        <CartesianGrid {...chartGridStyle} />
        <XAxis dataKey="name" {...chartAxisStyle} />
        <YAxis {...chartAxisStyle} />
        <Tooltip contentStyle={chartTooltipStyle} itemStyle={chartTooltipItemStyle} />
        <Legend wrapperStyle={{ fontSize: 11 }} />
        <Line
          type="monotone"
          dataKey="valor"
          stroke={chartColors[0]}
          strokeWidth={2}
          dot={false}
          name="Valor"
          isAnimationActive={false}
        />
        <Line
          type="monotone"
          dataKey="objetivo"
          stroke={chartColors[2]}
          strokeWidth={2}
          strokeDasharray="5 5"
          dot={false}
          name="Objetivo"
          isAnimationActive={false}
        />
      </LineChart>
    </ChartContainer>
  );
}
