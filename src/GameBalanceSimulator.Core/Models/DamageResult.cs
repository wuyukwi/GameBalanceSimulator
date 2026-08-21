namespace GameBalanceSimulator.Core.Models;

/// <summary>
/// 单次伤害计算结果。
/// </summary>
public sealed record DamageResult(
    double FinalDamage,
    bool IsCritical,
    bool IsDodged,
    string FormulaName);
