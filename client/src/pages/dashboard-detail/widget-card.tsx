import { Trash2 } from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { WidgetRenderer } from '@/components/widgets/widget-renderer';
import type { WidgetResponse } from '@/types/api';
import { WIDGET_TYPE_LABELS } from '@/types/enums';

interface WidgetCardProps {
  widget: WidgetResponse;
  onRemove: (widgetId: string) => void;
}

export function WidgetCard({ widget, onRemove }: WidgetCardProps) {
  return (
    <Card
      className="flex h-full flex-col"
      style={{
        gridColumn: `${widget.column + 1} / span ${widget.width}`,
        gridRow: `${widget.row + 1} / span ${widget.height}`,
      }}
    >
      <CardHeader className="flex flex-row items-center justify-between gap-2 pb-2">
        <div className="flex items-center gap-2 overflow-hidden">
          <CardTitle className="truncate text-sm">{widget.title}</CardTitle>
          <Badge variant="secondary" className="shrink-0 text-[10px]">
            {WIDGET_TYPE_LABELS[widget.type]}
          </Badge>
        </div>
        <Button
          variant="ghost"
          size="icon"
          className="h-7 w-7 shrink-0 text-muted-foreground hover:text-destructive"
          onClick={() => onRemove(widget.id)}
        >
          <Trash2 className="h-3.5 w-3.5" />
        </Button>
      </CardHeader>
      <CardContent className="flex-1 overflow-hidden">
        <div className="h-full w-full min-h-[120px]">
          <WidgetRenderer type={widget.type} widgetId={widget.id} />
        </div>
      </CardContent>
    </Card>
  );
}
