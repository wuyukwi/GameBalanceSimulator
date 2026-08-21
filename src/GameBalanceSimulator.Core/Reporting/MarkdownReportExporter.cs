using System.Globalization;
using System.Text;

namespace GameBalanceSimulator.Core.Reporting;

public sealed class MarkdownReportExporter : IReportExporter
{
    public string Format => "Markdown";

    public string Extension => ".md";

    public string Export(ReportData data)
    {
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
        AppendCurveData(builder, data);
        AppendSimulationResults(builder, data);

        return builder.ToString();
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
        builder.AppendLine("## Damage Curve");
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

    private static void AppendSimulationResults(StringBuilder builder, ReportData data)
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

        if (report.HistogramBuckets.Count > 0)
        {
            builder.AppendLine("## Damage Distribution Histogram");
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
