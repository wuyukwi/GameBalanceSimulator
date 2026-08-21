using GameBalanceSimulator.Core.Models;
using GameBalanceSimulator.Core.Services;

namespace GameBalanceSimulator.Core.Formulas;

/// <summary>
/// Multiplicative / division armor model: Damage = Attack * Attack / (Attack + max(0, Defense - Penetration))
/// </summary>
public sealed class MultiplicativeDamageFormula : IDamageFormula
{
    public string Name => "Multiplicative";

    public string Description => "Formula_Multiplicative_Description";

    public IReadOnlyList<FormulaParameter> Parameters { get; } = Array.Empty<FormulaParameter>();

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
        var denominator = attacker.BaseAttack + effectiveDefense;
        var baseDamage = denominator > 0
            ? attacker.BaseAttack * attacker.BaseAttack / denominator
            : attacker.BaseAttack;

        return Math.Max(1, baseDamage);
    }
}
