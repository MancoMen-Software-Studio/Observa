import { ScatterChart, Scatter, XAxis, YAxis, ZAxis, Tooltip, Cell } from 'recharts';
import { ChartContainer } from '../shared/chart-container';
import { useWidgetData } from '@/hooks/use-widget-data';
import { chartTooltipStyle, chartAxisStyle } from '@/lib/chart-theme';

interface HeatmapWidgetProps {
  widgetId: string;
}

interface HeatmapPoint {
  day: string;
  hour: string;
  value: number;
}

function getHeatColor(value: number): string {
  if (value < 25) {
    return '#1e3a5f';
  }
  if (value < 50) {
    return '#3b82f6';
  }
  if (value < 75) {
    return '#f59e0b';
  }
  return '#ef4444';
}

export function HeatmapWidget({ widgetId }: HeatmapWidgetProps) {
  const rawData = useWidgetData<HeatmapPoint[]>(widgetId, 'HeatMap');

  const days = ['Lun', 'Mar', 'Mie', 'Jue', 'Vie', 'Sab', 'Dom'];
  const hours = ['00-04', '04-08', '08-12', '12-16', '16-20', '20-24'];

  const data = rawData.map((d) => ({
    x: hours.indexOf(d.hour),
    y: days.indexOf(d.day),
    z: d.value,
    day: d.day,
    hour: d.hour,
    value: d.value,
  }));

  return (
    <ChartContainer>
      <ScatterChart margin={{ top: 10, right: 20, bottom: 10, left: 30 }}>
        <XAxis
          type="number"
          dataKey="x"
          domain={[0, 5]}
          ticks={[0, 1, 2, 3, 4, 5]}
          tickFormatter={(v: number) => hours[v] ?? ''}
          {...chartAxisStyle}
        />
        <YAxis
          type="number"
          dataKey="y"
          domain={[0, 6]}
          ticks={[0, 1, 2, 3, 4, 5, 6]}
          tickFormatter={(v: number) => days[v] ?? ''}
          {...chartAxisStyle}
        />
        <ZAxis type="number" dataKey="z" range={[200, 600]} />
        <Tooltip
          contentStyle={chartTooltipStyle}
          content={({ payload }) => {
            if (!payload?.[0]) {
              return null;
            }
            const d = payload[0].payload as { day: string; hour: string; value: number };
            return (
              <div style={chartTooltipStyle} className="px-3 py-2">
                <p className="font-medium">{d.day} {d.hour}</p>
                <p>Valor: {d.value}</p>
              </div>
            );
          }}
        />
        <Scatter data={data} isAnimationActive={false}>
          {data.map((entry, index) => (
            <Cell key={index} fill={getHeatColor(entry.z)} />
          ))}
        </Scatter>
      </ScatterChart>
    </ChartContainer>
  );
}
