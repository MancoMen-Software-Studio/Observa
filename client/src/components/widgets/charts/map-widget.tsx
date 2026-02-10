import { MapPin } from 'lucide-react';
import { useWidgetData } from '@/hooks/use-widget-data';
import { ScrollArea } from '@/components/ui/scroll-area';

interface MapWidgetProps {
  widgetId: string;
}

interface MapCity {
  name: string;
  lat: number;
  lng: number;
  value: number;
}

export function MapWidget({ widgetId }: MapWidgetProps) {
  const data = useWidgetData<MapCity[]>(widgetId, 'Map');
  const maxValue = Math.max(...data.map((d) => d.value));

  return (
    <ScrollArea className="h-full w-full">
      <div className="space-y-2 p-1">
        {data.map((city) => (
          <div key={city.name} className="flex items-center gap-3">
            <MapPin className="h-4 w-4 shrink-0 text-primary" />
            <div className="flex-1">
              <div className="flex items-center justify-between">
                <span className="text-sm font-medium">{city.name}</span>
                <span className="text-xs text-muted-foreground">
                  {city.value.toLocaleString('es-ES')}
                </span>
              </div>
              <div className="mt-1 h-1.5 w-full rounded-full bg-muted">
                <div
                  className="h-full rounded-full bg-primary transition-all"
                  style={{ width: `${(city.value / maxValue) * 100}%` }}
                />
              </div>
            </div>
          </div>
        ))}
      </div>
    </ScrollArea>
  );
}
