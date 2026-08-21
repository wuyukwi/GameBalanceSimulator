using GameBalanceSimulator.Core.Models;
using GameBalanceSimulator.Core.Services;

namespace GameBalanceSimulator.Core.Formulas;

/// <summary>
/// Percentage reduction armor model: damage scales down as effective defense grows.
/// </summary>
public sealed class PercentageReductionDamageFormula : IDamageFormula
{
    public string Name => "PercentageReduction";

    public string Description => "Formula_PercentageReduction_Description";

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
        const double reductionScale = 100.0;
        var reduction = effectiveDefense / (effectiveDefense + reductionScale);
        var multiplier = Math.Max(0.1, 1.0 - reduction);

        return Math.Max(1, attacker.BaseAttack * multiplier);
    }
}
