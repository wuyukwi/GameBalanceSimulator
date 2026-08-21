using GameBalanceSimulator.Core.Formulas;

namespace GameBalanceSimulator.Core.Services;

/// <summary>
/// Provides the set of available damage formulas and tracks the currently selected one.
/// </summary>
public interface IFormulaProvider
{
    IReadOnlyList<IDamageFormula> AvailableFormulas { get; }

    IDamageFormula CurrentFormula { get; set; }

    event EventHandler? CurrentFormulaChanged;
}
