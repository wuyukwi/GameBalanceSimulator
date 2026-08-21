using GameBalanceSimulator.Core.Formulas;
using GameBalanceSimulator.Core.Models;

namespace GameBalanceSimulator.Core.Reporting;

/// <summary>
/// Aggregates all data required to render a simulation report.
/// </summary>
public sealed record ReportData(
    DateTime Timestamp,
    StatBlock Attacker,
    StatBlock Defender,
    IDamageFormula Formula,
    double DefenseMin,
    double DefenseMax,
    double DefenseStep,
    double[] DamageCurveX,
    double[] DamageCurveY,
    double[] TtkCurveX,
    double[] TtkCurveY,
    SimulationConfig? SimulationConfig,
    SimulationReport? SimulationReport);
