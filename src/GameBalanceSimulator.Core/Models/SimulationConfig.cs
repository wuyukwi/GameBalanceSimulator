namespace GameBalanceSimulator.Core.Models;

/// <summary>
/// Configuration for a Monte Carlo simulation run.
/// </summary>
public sealed record SimulationConfig(
    StatBlock Attacker,
    StatBlock Defender,
    int IterationCount,
    string FormulaName,
    bool SimulateUntilDeath,
    int Seed = 0,
    Dictionary<string, double>? FormulaParameters = null);
