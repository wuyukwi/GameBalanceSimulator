using GameBalanceSimulator.Core.Models;
using GameBalanceSimulator.Core.Services;

namespace GameBalanceSimulator.Core.Formulas;

/// <summary>
/// True damage model: ignores defense and armor penetration entirely.
/// </summary>
public sealed class TrueDamageFormula : IDamageFormula
{
    public string Name => "TrueDamage";

    public string Description => "Formula_TrueDamage_Description";

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
        return Math.Max(1, attacker.BaseAttack);
    }
}
