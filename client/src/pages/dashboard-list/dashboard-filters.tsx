import { Search } from 'lucide-react';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import type { DashboardStatus } from '@/types/api';

interface DashboardFiltersProps {
  search: string;
  onSearchChange: (value: string) => void;
  statusFilter: DashboardStatus | 'all';
  onStatusFilterChange: (value: DashboardStatus | 'all') => void;
}

export function DashboardFilters({
  search,
  onSearchChange,
  statusFilter,
  onStatusFilterChange,
}: DashboardFiltersProps) {
  return (
    <div className="flex flex-col gap-3 sm:flex-row">
      <div className="relative flex-1">
        <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
        <Input
          placeholder="Buscar dashboards..."
          value={search}
          onChange={(e) => onSearchChange(e.target.value)}
          className="pl-9"
        />
      </div>
      <Select
        value={statusFilter}
        onValueChange={(v) => onStatusFilterChange(v as DashboardStatus | 'all')}
      >
        <SelectTrigger className="w-full sm:w-44">
          <SelectValue placeholder="Estado" />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value="all">Todos</SelectItem>
          <SelectItem value="Draft">Borrador</SelectItem>
          <SelectItem value="Published">Publicado</SelectItem>
          <SelectItem value="Archived">Archivado</SelectItem>
        </SelectContent>
      </Select>
    </div>
  );
}
