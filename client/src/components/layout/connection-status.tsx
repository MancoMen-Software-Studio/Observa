import { cn } from '@/lib/utils';
import { Tooltip, TooltipContent, TooltipTrigger } from '@/components/ui/tooltip';
import { useConnectionStore, type ConnectionState } from '@/stores/connection-store';

interface ConnectionStatusProps {
  compact?: boolean;
}

const colors: Record<ConnectionState, string> = {
  connected: 'bg-success',
  connecting: 'bg-warning animate-pulse',
  disconnected: 'bg-muted-foreground',
};

const labels: Record<ConnectionState, string> = {
  connected: 'Conectado',
  connecting: 'Conectando...',
  disconnected: 'Desconectado',
};

export function ConnectionStatus({ compact = false }: ConnectionStatusProps) {
  const state = useConnectionStore((s) => s.state);

  const dot = <span className={cn('h-2 w-2 rounded-full', colors[state])} />;

  if (compact) {
    return (
      <Tooltip>
        <TooltipTrigger asChild>
          <div className="flex items-center justify-center p-2">{dot}</div>
        </TooltipTrigger>
        <TooltipContent side="right">
          <p>{labels[state]}</p>
        </TooltipContent>
      </Tooltip>
    );
  }

  return (
    <div className="flex items-center gap-2 text-xs text-muted-foreground">
      {dot}
      <span>{labels[state]}</span>
    </div>
  );
}
