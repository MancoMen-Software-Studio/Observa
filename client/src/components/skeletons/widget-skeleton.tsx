import { Skeleton } from '@/components/ui/skeleton';
import { Card, CardContent, CardHeader } from '@/components/ui/card';

export function WidgetSkeleton() {
  return (
    <Card className="flex h-full flex-col">
      <CardHeader className="flex flex-row items-center justify-between gap-2 pb-2">
        <div className="flex items-center gap-2">
          <Skeleton className="h-4 w-32" />
          <Skeleton className="h-5 w-16 rounded-md" />
        </div>
        <Skeleton className="h-7 w-7 rounded" />
      </CardHeader>
      <CardContent className="flex-1">
        <Skeleton className="h-full min-h-[120px] w-full rounded" />
      </CardContent>
    </Card>
  );
}
