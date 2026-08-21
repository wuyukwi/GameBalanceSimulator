using GameBalanceSimulator.Core.Models;
using GameBalanceSimulator.Core.Services;

namespace GameBalanceSimulator.Core.Formulas;

public interface IDamageFormula
{
    string Name { get; }

    /// <summary>
    /// Calculates a single stochastic damage roll.
    /// </summary>
    DamageResult Calculate(StatBlock attacker, StatBlock defender, IRandomProvider random);

    /// <summary>
    /// Calculates the deterministic base damage before critical hit and dodge randomization.
    /// </summary>
    double CalculateBase(StatBlock attacker, StatBlock defender);
}
