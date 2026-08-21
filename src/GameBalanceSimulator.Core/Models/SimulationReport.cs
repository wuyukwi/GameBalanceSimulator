namespace GameBalanceSimulator.Core.Models;

/// <summary>
/// Report produced by a Monte Carlo simulation run.
/// </summary>
public sealed class SimulationReport
{
    public int IterationCount { get; init; }

    public double AverageDamage { get; init; }

    public double MaxDamage { get; init; }

    public double MinDamage { get; init; }

    /// <summary>
    /// Average number of attacks required to kill (time-to-kill). Only meaningful when SimulateUntilDeath is true.
    /// </summary>
    public double AverageTtk { get; init; }

    public long CriticalCount { get; init; }

    public long DodgeCount { get; init; }

    public double CriticalRate { get; init; }

    public double DodgeRate { get; init; }

    public IReadOnlyList<int> HistogramBuckets { get; init; } = Array.Empty<int>();

    public IReadOnlyList<double> HistogramBinEdges { get; init; } = Array.Empty<double>();
}
