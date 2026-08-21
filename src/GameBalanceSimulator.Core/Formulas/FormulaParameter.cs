using CommunityToolkit.Mvvm.ComponentModel;

namespace GameBalanceSimulator.Core.Formulas;

/// <summary>
/// A tunable parameter exposed by a damage formula.
/// </summary>
public sealed partial class FormulaParameter : ObservableObject
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private double _value;

    [ObservableProperty]
    private double _minimum;

    [ObservableProperty]
    private double _maximum;

    [ObservableProperty]
    private double _increment;

    [ObservableProperty]
    private string _descriptionKey;

    public FormulaParameter(
        string name,
        double value,
        double minimum,
        double maximum,
        double increment,
        string descriptionKey)
    {
        _name = name;
        _value = value;
        _minimum = minimum;
        _maximum = maximum;
        _increment = increment;
        _descriptionKey = descriptionKey;
    }
}
