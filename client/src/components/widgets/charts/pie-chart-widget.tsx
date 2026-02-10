import { PieChart, Pie, Cell, Tooltip, Legend } from 'recharts';
import { ChartContainer } from '../shared/chart-container';
import { useWidgetData } from '@/hooks/use-widget-data';
import { chartColors, chartTooltipStyle, chartTooltipItemStyle } from '@/lib/chart-theme';

interface PieChartWidgetProps {
  widgetId: string;
}

interface PieSlice {
  name: string;
  value: number;
}

export function PieChartWidget({ widgetId }: PieChartWidgetProps) {
  const data = useWidgetData<PieSlice[]>(widgetId, 'PieChart');

  return (
    <ChartContainer>
      <PieChart>
        <Pie
          data={data}
          cx="50%"
          cy="50%"
          innerRadius="40%"
          outerRadius="70%"
          dataKey="value"
          stroke="none"
          label={({ name, percent }) => `${name} ${(percent * 100).toFixed(0)}%`}
          labelLine={false}
          isAnimationActive={false}
        >
          {data.map((_, index) => (
            <Cell key={index} fill={chartColors[index % chartColors.length]} />
          ))}
        </Pie>
        <Tooltip contentStyle={chartTooltipStyle} itemStyle={chartTooltipItemStyle} />
        <Legend wrapperStyle={{ fontSize: 11 }} />
      </PieChart>
    </ChartContainer>
  );
}
