using GameBalanceSimulator.Core.Models;
using GameBalanceSimulator.Core.Services;

namespace GameBalanceSimulator.Core.Formulas;

/// <summary>
/// Percentage reduction armor model: damage scales down as effective defense grows.
/// </summary>
public sealed class PercentageReductionDamageFormula : IDamageFormula
{
    private const double DefaultReductionScale = 100.0;
    private const double MinimumDamageMultiplier = 0.1;

    public string Name => "PercentageReduction";

    public string Description => "Formula_PercentageReduction_Description";

    public IReadOnlyList<FormulaParameter> Parameters { get; }

    public PercentageReductionDamageFormula()
    {
        Parameters = new List<FormulaParameter>
        {
            new(
                "ReductionScale",
                DefaultReductionScale,
                1,
                10000,
                1,
                "FormulaParameter_ReductionScale")
        };
    }

    public DamageResult Calculate(StatBlock attacker, StatBlock defender, IRandomProvider random)
    {
        var isDodged = random.NextDouble() < defender.DodgeRate;
        if (isDodged)
        {
            return new DamageResult(0, false, true, Name);
        }

        var baseDamage = CalculateBase(attacker, defender);
        var isCritical = random.NextDouble() < attacker.CriticalRate;
        var finalDamage = baseDamage * (isCritical ? attacker.CriticalDamage : 1.0);

        return new DamageResult(finalDamage, isCritical, false, Name);
    }

    public double CalculateBase(StatBlock attacker, StatBlock defender)
    {
        var effectiveDefense = Math.Max(0, defender.Defense - attacker.ArmorPenetration);
        var reductionScale = Parameters[0].Value;
        var reduction = effectiveDefense / (effectiveDefense + reductionScale);
        var multiplier = Math.Max(MinimumDamageMultiplier, 1.0 - reduction);

        return Math.Max(1, attacker.BaseAttack * multiplier);
    }
}
