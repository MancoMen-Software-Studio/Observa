using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using Observa.Domain.Enums;

namespace Observa.Infrastructure.Simulation;

/// <summary>
/// Generador estatico de datos simulados para cada tipo de widget.
/// Mantiene estado interno para producir variaciones graduales entre ticks.
/// </summary>
public static class WidgetDataGenerator
{
    private static readonly ConcurrentDictionary<Guid, double> State = new();
    private static readonly Random Rng = new();
    private static readonly object SyncLock = new();

    /// <summary>
    /// Genera datos simulados segun el tipo de widget.
    /// </summary>
    public static object Generate(Guid widgetId, WidgetType widgetType)
    {
        return widgetType switch
        {
            WidgetType.LineChart => GenerateLinePoint(widgetId),
            WidgetType.BarChart => GenerateBarData(widgetId),
            WidgetType.PieChart => GeneratePieData(widgetId),
            WidgetType.HeatMap => GenerateHeatmapData(widgetId),
            WidgetType.ScatterPlot => GenerateScatterData(widgetId),
            WidgetType.Gauge => GenerateGaugeData(widgetId),
            WidgetType.Table => GenerateTableData(widgetId),
            WidgetType.KpiCard => GenerateKpiData(widgetId),
            WidgetType.Map => GenerateMapData(widgetId),
            _ => new { }
        };
    }

    /// <summary>
    /// Genera un unico punto para LineChart. El frontend acumula en sliding window.
    /// </summary>
    private static object GenerateLinePoint(Guid widgetId)
    {
        var key = GetKey(widgetId, "line");
        var current = GetOrInit(key, 350.0);
        var next = RandomWalk(current, -12, 12, 50, 800);
        State[key] = next;

        var objKey = GetKey(widgetId, "line_obj");
        var objective = GetOrInit(objKey, 500.0);
        var nextObj = RandomWalk(objective, -4, 4, 300, 700);
        State[objKey] = nextObj;

        return new
        {
            name = DateTime.UtcNow.ToString("HH:mm:ss", CultureInfo.InvariantCulture),
            valor = (int)Math.Round(next),
            objetivo = (int)Math.Round(nextObj)
        };
    }

    /// <summary>
    /// Genera datos de barras con fluctuaciones graduales.
    /// </summary>
    private static List<object> GenerateBarData(Guid widgetId)
    {
        var categories = new[] { "Producto A", "Producto B", "Producto C", "Producto D", "Producto E" };
        var result = new List<object>(categories.Length);

        for (int i = 0; i < categories.Length; i++)
        {
            var salesKey = GetKey(widgetId, $"bar_s{i}");
            var retKey = GetKey(widgetId, $"bar_r{i}");

            var sales = GetOrInit(salesKey, 400.0 + i * 120);
            var returns = GetOrInit(retKey, 50.0 + i * 20);

            sales = RandomWalk(sales, -15, 15, 100, 1200);
            returns = RandomWalk(returns, -8, 8, 10, 300);

            State[salesKey] = sales;
            State[retKey] = returns;

            result.Add(new
            {
                name = categories[i],
                ventas = (int)Math.Round(sales),
                retornos = (int)Math.Round(returns)
            });
        }

        return result;
    }

    /// <summary>
    /// Genera datos de pastel con valores variando gradualmente.
    /// </summary>
    private static List<object> GeneratePieData(Guid widgetId)
    {
        var segments = new[] { "Segmento A", "Segmento B", "Segmento C", "Segmento D", "Segmento E" };
        var result = new List<object>(segments.Length);

        for (int i = 0; i < segments.Length; i++)
        {
            var key = GetKey(widgetId, $"pie{i}");
            var current = GetOrInit(key, 200.0 + i * 80);
            var next = RandomWalk(current, -8, 8, 50, 600);
            State[key] = next;

            result.Add(new { name = segments[i], value = (int)Math.Round(next) });
        }

        return result;
    }

    /// <summary>
    /// Genera datos de mapa de calor con variaciones leves.
    /// </summary>
    private static List<object> GenerateHeatmapData(Guid widgetId)
    {
        var days = new[] { "Lun", "Mar", "Mie", "Jue", "Vie", "Sab", "Dom" };
        var hours = new[] { "00-04", "04-08", "08-12", "12-16", "16-20", "20-24" };
        var result = new List<object>(days.Length * hours.Length);

        for (int d = 0; d < days.Length; d++)
        {
            for (int h = 0; h < hours.Length; h++)
            {
                var key = GetKey(widgetId, $"hm{d}_{h}");
                var current = GetOrInit(key, 50.0);
                var next = RandomWalk(current, -3, 3, 0, 100);
                State[key] = next;

                result.Add(new { day = days[d], hour = hours[h], value = (int)Math.Round(next) });
            }
        }

        return result;
    }

    /// <summary>
    /// Genera nube de puntos con desplazamiento gradual.
    /// </summary>
    private static List<object> GenerateScatterData(Guid widgetId)
    {
        var result = new List<object>(30);

        for (int i = 0; i < 30; i++)
        {
            var xKey = GetKey(widgetId, $"sc_x{i}");
            var yKey = GetKey(widgetId, $"sc_y{i}");
            var zKey = GetKey(widgetId, $"sc_z{i}");

            var x = GetOrInit(xKey, NextRandom(10, 90));
            var y = GetOrInit(yKey, NextRandom(10, 90));
            var z = GetOrInit(zKey, NextRandom(100, 500));

            x = RandomWalk(x, -2, 2, 0, 100);
            y = RandomWalk(y, -2, 2, 0, 100);
            z = RandomWalk(z, -10, 10, 50, 600);

            State[xKey] = x;
            State[yKey] = y;
            State[zKey] = z;

            result.Add(new { x = (int)Math.Round(x), y = (int)Math.Round(y), z = (int)Math.Round(z) });
        }

        return result;
    }

    /// <summary>
    /// Genera valor de gauge oscilando sinusoidalmente.
    /// </summary>
    private static object GenerateGaugeData(Guid widgetId)
    {
        var key = GetKey(widgetId, "gauge");
        var tickKey = GetKey(widgetId, "gauge_t");

        var tick = GetOrInit(tickKey, 0.0) + 1;
        State[tickKey] = tick;

        var sinValue = 57.5 + 37.5 * Math.Sin(tick * 0.15);
        var noise = NextRandom(-3, 3);
        var value = Math.Clamp(sinValue + noise, 0, 100);
        State[key] = value;

        return new
        {
            value = (int)Math.Round(value),
            min = 0,
            max = 100,
            label = "Rendimiento"
        };
    }

    /// <summary>
    /// Genera datos de tabla con metricas de servidores fluctuando.
    /// </summary>
    private static List<object> GenerateTableData(Guid widgetId)
    {
        var servers = new[] { "Servidor Alpha", "Servidor Beta", "Servidor Gamma", "Servidor Delta", "Servidor Epsilon", "Servidor Zeta" };
        var result = new List<object>(servers.Length);

        for (int i = 0; i < servers.Length; i++)
        {
            var cpuKey = GetKey(widgetId, $"tbl_cpu{i}");
            var memKey = GetKey(widgetId, $"tbl_mem{i}");

            var cpu = GetOrInit(cpuKey, 30.0 + i * 10);
            var mem = GetOrInit(memKey, 40.0 + i * 8);

            cpu = RandomWalk(cpu, -3, 3, 5, 99);
            mem = RandomWalk(mem, -2, 2, 10, 99);

            State[cpuKey] = cpu;
            State[memKey] = mem;

            var cpuVal = (int)Math.Round(cpu);
            var status = cpuVal > 85 ? "Critico" : cpuVal > 65 ? "Advertencia" : "Activo";

            result.Add(new
            {
                name = servers[i],
                cpu = cpuVal,
                memory = (int)Math.Round(mem),
                status,
                uptime = $"{Math.Max(90, 100 - cpuVal / 10)}%"
            });
        }

        return result;
    }

    /// <summary>
    /// Genera datos KPI con random walk gradual y tendencia.
    /// </summary>
    private static object GenerateKpiData(Guid widgetId)
    {
        var valKey = GetKey(widgetId, "kpi_val");
        var prevKey = GetKey(widgetId, "kpi_prev");

        var value = GetOrInit(valKey, 5000.0);
        var prev = GetOrInit(prevKey, 4800.0);

        State[prevKey] = value;

        value = RandomWalk(value, -50, 70, 500, 15000);
        State[valKey] = value;

        var valInt = (int)Math.Round(value);
        var prevInt = (int)Math.Round(prev);
        var change = prevInt == 0 ? 0.0 : ((value - prev) / prev) * 100;

        return new
        {
            value = valInt,
            previousValue = prevInt,
            changePercent = Math.Round(change, 1),
            label = "Usuarios activos",
            isPositive = change >= 0
        };
    }

    /// <summary>
    /// Genera datos de mapa con valores por ciudad variando.
    /// </summary>
    private static List<object> GenerateMapData(Guid widgetId)
    {
        var cities = new[]
        {
            (name: "Madrid", lat: 40.42, lng: -3.7),
            (name: "Barcelona", lat: 41.39, lng: 2.17),
            (name: "Bogota", lat: 4.71, lng: -74.07),
            (name: "CDMX", lat: 19.43, lng: -99.13),
            (name: "Buenos Aires", lat: -34.6, lng: -58.38),
            (name: "Lima", lat: -12.05, lng: -77.04)
        };

        var result = new List<object>(cities.Length);

        for (int i = 0; i < cities.Length; i++)
        {
            var key = GetKey(widgetId, $"map{i}");
            var current = GetOrInit(key, 500.0 + i * 100);
            var next = RandomWalk(current, -15, 15, 100, 1200);
            State[key] = next;

            result.Add(new
            {
                name = cities[i].name,
                lat = cities[i].lat,
                lng = cities[i].lng,
                value = (int)Math.Round(next)
            });
        }

        return result;
    }

    private static Guid GetKey(Guid widgetId, string suffix)
    {
        var hash = HashCode.Combine(widgetId, suffix);
        return new Guid(hash, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
    }

    private static double GetOrInit(Guid key, double defaultValue)
    {
        return State.GetOrAdd(key, defaultValue);
    }

    private static double RandomWalk(double current, double minDelta, double maxDelta, double floor, double ceiling)
    {
        double delta;
        lock (SyncLock)
        {
            delta = Rng.NextDouble() * (maxDelta - minDelta) + minDelta;
        }

        return Math.Clamp(current + delta, floor, ceiling);
    }

    private static double NextRandom(double min, double max)
    {
        lock (SyncLock)
        {
            return Rng.NextDouble() * (max - min) + min;
        }
    }
}
