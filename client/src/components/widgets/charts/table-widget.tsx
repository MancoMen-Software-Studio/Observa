import { useWidgetData } from '@/hooks/use-widget-data';
import { Badge } from '@/components/ui/badge';
import { ScrollArea } from '@/components/ui/scroll-area';

interface TableWidgetProps {
  widgetId: string;
}

interface TableRow {
  name: string;
  cpu: number;
  memory: number;
  status: string;
  uptime: string;
}

function getStatusVariant(status: string): 'success' | 'warning' | 'destructive' {
  switch (status) {
    case 'Activo':
      return 'success';
    case 'Advertencia':
      return 'warning';
    default:
      return 'destructive';
  }
}

export function TableWidget({ widgetId }: TableWidgetProps) {
  const data = useWidgetData<TableRow[]>(widgetId, 'Table');

  return (
    <ScrollArea className="h-full w-full">
      <table className="w-full text-sm">
        <thead>
          <tr className="border-b text-left">
            <th className="px-2 py-1.5 font-medium text-muted-foreground">Servidor</th>
            <th className="px-2 py-1.5 font-medium text-muted-foreground">CPU</th>
            <th className="px-2 py-1.5 font-medium text-muted-foreground">Memoria</th>
            <th className="px-2 py-1.5 font-medium text-muted-foreground">Estado</th>
            <th className="px-2 py-1.5 font-medium text-muted-foreground">Uptime</th>
          </tr>
        </thead>
        <tbody>
          {data.map((row) => (
            <tr key={row.name} className="border-b border-border/50">
              <td className="px-2 py-1.5 font-medium">{row.name}</td>
              <td className="px-2 py-1.5">{row.cpu}%</td>
              <td className="px-2 py-1.5">{row.memory}%</td>
              <td className="px-2 py-1.5">
                <Badge variant={getStatusVariant(row.status)} className="text-[10px]">
                  {row.status}
                </Badge>
              </td>
              <td className="px-2 py-1.5">{row.uptime}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </ScrollArea>
  );
}
