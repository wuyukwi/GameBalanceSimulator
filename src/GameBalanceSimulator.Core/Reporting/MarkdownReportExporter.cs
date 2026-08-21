using System.Globalization;
using System.Text;
using ScottPlot;

namespace GameBalanceSimulator.Core.Reporting;

public sealed class MarkdownReportExporter : IReportExporter
{
    private const int ChartWidth = 800;
    private const int ChartHeight = 400;

    public string Format => "Markdown";

    public string Extension => ".md";

    public string Export(ReportData data, string outputDirectory)
    {
        Directory.CreateDirectory(outputDirectory);

        var damageCurveFileName = SaveDamageCurve(data, outputDirectory);
        var ttkCurveFileName = SaveTtkCurve(data, outputDirectory);
        var histogramFileName = data.SimulationReport?.HistogramBuckets.Count > 0
            ? SaveHistogram(data, outputDirectory)
            : null;

        var builder = new StringBuilder();
        var timestamp = data.Timestamp.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        builder.AppendLine($"# Game Balance Simulation Report");
        builder.AppendLine();
        builder.AppendLine($"- **Generated**: {timestamp}");
        builder.AppendLine($"- **Formula**: {data.Formula.Name}");
        builder.AppendLine();

        AppendParameters(builder, data);
        AppendStats(builder, "Attacker", data.Attacker);
        AppendStats(builder, "Defender", data.Defender);

        builder.AppendLine("## Damage Curve");
        builder.AppendLine();
        builder.AppendLine($"![Damage Curve]({damageCurveFileName})");
        builder.AppendLine();

        builder.AppendLine("## TTK Curve");
        builder.AppendLine();
        builder.AppendLine($"![TTK Curve]({ttkCurveFileName})");
        builder.AppendLine();

        AppendCurveData(builder, data);
        AppendSimulationResults(builder, data, histogramFileName);

        return builder.ToString();
    }

    private static string SaveDamageCurve(ReportData data, string outputDirectory)
    {
        var plot = new Plot();
        plot.Add.Scatter(data.DamageCurveX, data.DamageCurveY);
        var fileName = "damage_curve.png";
        plot.SavePng(Path.Combine(outputDirectory, fileName), ChartWidth, ChartHeight);
        return fileName;
    }

    private static string SaveTtkCurve(ReportData data, string outputDirectory)
    {
        var plot = new Plot();
        plot.Add.Scatter(data.TtkCurveX, data.TtkCurveY);
        var fileName = "ttk_curve.png";
        plot.SavePng(Path.Combine(outputDirectory, fileName), ChartWidth, ChartHeight);
        return fileName;
    }

    private static string SaveHistogram(ReportData data, string outputDirectory)
    {
        var report = data.SimulationReport!;
        var buckets = report.HistogramBuckets;
        var edges = report.HistogramBinEdges;
        var bars = new Bar[buckets.Count];

        for (var i = 0; i < buckets.Count; i++)
        {
            var center = (edges[i] + edges[i + 1]) / 2.0;
            var width = edges[i + 1] - edges[i];
            bars[i] = new Bar
            {
                Position = center,
                Value = buckets[i],
                Size = width
            };
        }

        var plot = new Plot();
        plot.Add.Bars(bars);
        var fileName = "damage_histogram.png";
        plot.SavePng(Path.Combine(outputDirectory, fileName), ChartWidth, ChartHeight);
        return fileName;
    }

    private static void AppendParameters(StringBuilder builder, ReportData data)
    {
        if (data.Formula.Parameters.Count == 0)
        {
            return;
        }

        builder.AppendLine("## Formula Parameters");
        builder.AppendLine();
        builder.AppendLine("| Parameter | Value |");
        builder.AppendLine("| --- | --- |");

        foreach (var parameter in data.Formula.Parameters)
        {
            builder.AppendLine($"| {parameter.Name} | {parameter.Value:F2} |");
        }

        builder.AppendLine();
    }

    private static void AppendStats(StringBuilder builder, string label, Models.StatBlock stats)
    {
        builder.AppendLine($"## {label} Stats");
        builder.AppendLine();
        builder.AppendLine("| Attribute | Value |");
        builder.AppendLine("| --- | --- |");
        builder.AppendLine($"| Base Attack | {stats.BaseAttack:F2} |");
        builder.AppendLine($"| Defense | {stats.Defense:F2} |");
        builder.AppendLine($"| Critical Rate | {stats.CriticalRate:P2} |");
        builder.AppendLine($"| Critical Damage | {stats.CriticalDamage:F2}x |");
        builder.AppendLine($"| Dodge Rate | {stats.DodgeRate:P2} |");
        builder.AppendLine($"| Armor Penetration | {stats.ArmorPenetration:F2} |");
        builder.AppendLine($"| Attack Interval | {stats.AttackInterval:F2} |");
        builder.AppendLine($"| Max Health | {stats.MaxHealth:F2} |");
        builder.AppendLine();
    }

    private static void AppendCurveData(StringBuilder builder, ReportData data)
    {
        builder.AppendLine("## Curve Data");
        builder.AppendLine();
        builder.AppendLine($"Range: {data.DefenseMin:F2} ~ {data.DefenseMax:F2} (step {data.DefenseStep:F2})");
        builder.AppendLine();
        builder.AppendLine("| Defense | Expected Damage | TTK (Turns) |");
        builder.AppendLine("| --- | --- | --- |");

        for (var i = 0; i < data.DamageCurveX.Length; i++)
        {
            var defense = data.DamageCurveX[i];
            var damage = data.DamageCurveY[i];
            var ttk = data.TtkCurveY[i];
            builder.AppendLine($"| {defense:F2} | {damage:F2} | {ttk:F0} |");
        }

        builder.AppendLine();
    }

    private static void AppendSimulationResults(StringBuilder builder, ReportData data, string? histogramFileName)
    {
        if (data.SimulationReport is null)
        {
            return;
        }

        var report = data.SimulationReport;
        var config = data.SimulationConfig;

        builder.AppendLine("## Monte Carlo Simulation Results");
        builder.AppendLine();

        if (config is not null)
        {
            builder.AppendLine($"- **Iterations**: {config.IterationCount:N0}");
            builder.AppendLine($"- **Simulate Until Death**: {config.SimulateUntilDeath}");
            builder.AppendLine($"- **Seed**: {(config.Seed > 0 ? config.Seed.ToString(CultureInfo.InvariantCulture) : "Random")}");
            builder.AppendLine();
        }

        builder.AppendLine("| Metric | Value |");
        builder.AppendLine("| --- | --- |");
        builder.AppendLine($"| Average Damage | {report.AverageDamage:F2} |");
        builder.AppendLine($"| Max Damage | {report.MaxDamage:F2} |");
        builder.AppendLine($"| Min Damage | {report.MinDamage:F2} |");
        builder.AppendLine($"| Average TTK | {report.AverageTtk:F2} |");
        builder.AppendLine($"| Critical Rate | {report.CriticalRate:P2} |");
        builder.AppendLine($"| Dodge Rate | {report.DodgeRate:P2} |");
        builder.AppendLine();

        if (!string.IsNullOrEmpty(histogramFileName))
        {
            builder.AppendLine("## Damage Distribution Histogram");
            builder.AppendLine();
            builder.AppendLine($"![Damage Histogram]({histogramFileName})");
            builder.AppendLine();
            builder.AppendLine("| Range Start | Range End | Count |");
            builder.AppendLine("| --- | --- | --- |");

            for (var i = 0; i < report.HistogramBuckets.Count; i++)
            {
                var start = report.HistogramBinEdges[i];
                var end = report.HistogramBinEdges[i + 1];
                var count = report.HistogramBuckets[i];
                builder.AppendLine($"| {start:F2} | {end:F2} | {count} |");
            }

            builder.AppendLine();
        }
    }
}
