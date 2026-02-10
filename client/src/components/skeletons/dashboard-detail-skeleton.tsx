import { Skeleton } from '@/components/ui/skeleton';

export function DashboardDetailSkeleton() {
  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <Skeleton className="h-8 w-64" />
          <Skeleton className="h-6 w-20 rounded-md" />
        </div>
        <Skeleton className="h-8 w-28" />
      </div>

      <div className="flex items-center justify-between">
        <Skeleton className="h-4 w-80" />
        <Skeleton className="h-8 w-36" />
      </div>

      <div className="grid grid-cols-12 gap-4">
        <Skeleton className="col-span-4 h-48 rounded-xl" />
        <Skeleton className="col-span-4 h-48 rounded-xl" />
        <Skeleton className="col-span-4 h-48 rounded-xl" />
        <Skeleton className="col-span-6 h-48 rounded-xl" />
        <Skeleton className="col-span-6 h-48 rounded-xl" />
      </div>
    </div>
  );
}
