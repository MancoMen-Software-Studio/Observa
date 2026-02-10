import { BarChart3 } from 'lucide-react';

export function NoDataState() {
  return (
    <div className="flex h-full flex-col items-center justify-center gap-2 text-muted-foreground">
      <BarChart3 className="h-8 w-8" />
      <p className="text-xs">Sin datos disponibles</p>
    </div>
  );
}
