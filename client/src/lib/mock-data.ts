function seededRandom(seed: string) {
  let h = 0;
  for (let i = 0; i < seed.length; i++) {
    h = Math.imul(31, h) + seed.charCodeAt(i) | 0;
  }
  return function () {
    h = h ^ (h << 13);
    h = h ^ (h >> 17);
    h = h ^ (h << 5);
    return (h >>> 0) / 4294967296;
  };
}

export function generateLineData(widgetId: string) {
  const rng = seededRandom(widgetId);
  const months = ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'];
  let base = rng() * 500 + 100;

  return months.map((month) => {
    base += (rng() - 0.4) * 80;
    return {
      name: month,
      valor: Math.round(Math.max(0, base)),
      objetivo: Math.round(rng() * 200 + 400),
    };
  });
}

export function generateBarData(widgetId: string) {
  const rng = seededRandom(widgetId);
  const categories = ['Producto A', 'Producto B', 'Producto C', 'Producto D', 'Producto E'];

  return categories.map((name) => ({
    name,
    ventas: Math.round(rng() * 1000 + 200),
    retornos: Math.round(rng() * 200),
  }));
}

export function generatePieData(widgetId: string) {
  const rng = seededRandom(widgetId);
  const slices = ['Segmento A', 'Segmento B', 'Segmento C', 'Segmento D', 'Segmento E'];

  return slices.map((name) => ({
    name,
    value: Math.round(rng() * 500 + 100),
  }));
}

export function generateHeatmapData(widgetId: string) {
  const rng = seededRandom(widgetId);
  const days = ['Lun', 'Mar', 'Mie', 'Jue', 'Vie', 'Sab', 'Dom'];
  const hours = ['00-04', '04-08', '08-12', '12-16', '16-20', '20-24'];
  const data: Array<{ day: string; hour: string; value: number }> = [];

  for (const day of days) {
    for (const hour of hours) {
      data.push({ day, hour, value: Math.round(rng() * 100) });
    }
  }

  return data;
}

export function generateScatterData(widgetId: string) {
  const rng = seededRandom(widgetId);
  return Array.from({ length: 30 }, () => ({
    x: Math.round(rng() * 100),
    y: Math.round(rng() * 100),
    z: Math.round(rng() * 500 + 100),
  }));
}

export function generateGaugeData(widgetId: string) {
  const rng = seededRandom(widgetId);
  return {
    value: Math.round(rng() * 100),
    min: 0,
    max: 100,
    label: 'Rendimiento',
  };
}

export function generateTableData(widgetId: string) {
  const rng = seededRandom(widgetId);
  const names = ['Servidor Alpha', 'Servidor Beta', 'Servidor Gamma', 'Servidor Delta', 'Servidor Epsilon', 'Servidor Zeta'];
  const statuses = ['Activo', 'Advertencia', 'Critico'];

  return names.map((name) => ({
    name,
    cpu: Math.round(rng() * 100),
    memory: Math.round(rng() * 100),
    status: statuses[Math.floor(rng() * 3)],
    uptime: `${Math.round(rng() * 99 + 1)}%`,
  }));
}

export function generateKpiData(widgetId: string) {
  const rng = seededRandom(widgetId);
  const value = Math.round(rng() * 10000);
  const prev = Math.round(rng() * 10000);
  const change = ((value - prev) / prev) * 100;

  return {
    value,
    previousValue: prev,
    changePercent: Math.round(change * 10) / 10,
    label: 'Usuarios activos',
    isPositive: change >= 0,
  };
}

export function generateMapData(widgetId: string) {
  const rng = seededRandom(widgetId);
  const cities = [
    { name: 'Madrid', lat: 40.42, lng: -3.7 },
    { name: 'Barcelona', lat: 41.39, lng: 2.17 },
    { name: 'Bogota', lat: 4.71, lng: -74.07 },
    { name: 'CDMX', lat: 19.43, lng: -99.13 },
    { name: 'Buenos Aires', lat: -34.6, lng: -58.38 },
    { name: 'Lima', lat: -12.05, lng: -77.04 },
  ];

  return cities.map((city) => ({
    ...city,
    value: Math.round(rng() * 1000 + 100),
  }));
}
