import { ScatterChart, Scatter, XAxis, YAxis, ZAxis, CartesianGrid, Tooltip } from 'recharts';
import { ChartContainer } from '../shared/chart-container';
import { useWidgetData } from '@/hooks/use-widget-data';
import { chartColors, chartTooltipStyle, chartTooltipItemStyle, chartGridStyle, chartAxisStyle } from '@/lib/chart-theme';

interface ScatterPlotWidgetProps {
  widgetId: string;
}

interface ScatterPoint {
  x: number;
  y: number;
  z: number;
}

export function ScatterPlotWidget({ widgetId }: ScatterPlotWidgetProps) {
  const data = useWidgetData<ScatterPoint[]>(widgetId, 'ScatterPlot');

  return (
    <ChartContainer>
      <ScatterChart margin={{ top: 10, right: 20, bottom: 10, left: 0 }}>
        <CartesianGrid {...chartGridStyle} />
        <XAxis type="number" dataKey="x" name="X" {...chartAxisStyle} />
        <YAxis type="number" dataKey="y" name="Y" {...chartAxisStyle} />
        <ZAxis type="number" dataKey="z" range={[40, 300]} />
        <Tooltip
          contentStyle={chartTooltipStyle}
          itemStyle={chartTooltipItemStyle}
          cursor={{ strokeDasharray: '3 3' }}
        />
        <Scatter data={data} fill={chartColors[2]} opacity={0.7} isAnimationActive={false} />
      </ScatterChart>
    </ChartContainer>
  );
}
