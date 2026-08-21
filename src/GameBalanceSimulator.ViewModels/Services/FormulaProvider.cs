using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using GameBalanceSimulator.Core.Formulas;
using GameBalanceSimulator.Core.Services;

namespace GameBalanceSimulator.ViewModels.Services;

public sealed partial class FormulaProvider : ObservableObject, IFormulaProvider
{
    [ObservableProperty]
    private IDamageFormula _currentFormula;

    public IReadOnlyList<IDamageFormula> AvailableFormulas { get; }

    public event EventHandler? CurrentFormulaChanged;

    public FormulaProvider(IEnumerable<IDamageFormula> availableFormulas)
    {
        AvailableFormulas = new ReadOnlyCollection<IDamageFormula>(availableFormulas.ToList());
        _currentFormula = AvailableFormulas.FirstOrDefault()
                          ?? throw new ArgumentException("At least one formula must be provided.", nameof(availableFormulas));
    }

    partial void OnCurrentFormulaChanged(IDamageFormula value)
    {
        CurrentFormulaChanged?.Invoke(this, EventArgs.Empty);
    }
}
