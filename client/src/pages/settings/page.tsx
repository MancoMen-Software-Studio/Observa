import { PageHeader } from '@/components/shared/page-header';
import { PageTransition } from '@/components/shared/page-transition';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Separator } from '@/components/ui/separator';
import { useHealth } from '@/hooks/queries/use-health';

export function Component() {
  const { data: health, isLoading } = useHealth();

  return (
    <PageTransition>
      <div className="space-y-6">
        <PageHeader title="Configuracion" description="Informacion y estado del sistema" />

        <div className="grid gap-4 sm:grid-cols-2">
          <Card>
            <CardHeader>
              <CardTitle className="text-base">Estado del backend</CardTitle>
              <CardDescription>Conexion con la API de Observa</CardDescription>
            </CardHeader>
            <CardContent>
              {isLoading ? (
                <Badge variant="secondary">Verificando...</Badge>
              ) : health ? (
                <Badge variant="success">{health.status}</Badge>
              ) : (
                <Badge variant="destructive">Sin conexion</Badge>
              )}
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle className="text-base">Informacion del sistema</CardTitle>
              <CardDescription>Detalles tecnicos</CardDescription>
            </CardHeader>
            <CardContent className="space-y-2 text-sm">
              <div className="flex justify-between">
                <span className="text-muted-foreground">Frontend</span>
                <span>React 19 + TypeScript</span>
              </div>
              <Separator />
              <div className="flex justify-between">
                <span className="text-muted-foreground">Backend</span>
                <span>.NET 9.0</span>
              </div>
              <Separator />
              <div className="flex justify-between">
                <span className="text-muted-foreground">Tiempo real</span>
                <span>SignalR</span>
              </div>
              <Separator />
              <div className="flex justify-between">
                <span className="text-muted-foreground">Atajos de teclado</span>
                <span>Ctrl+N, Ctrl+W</span>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </PageTransition>
  );
}
