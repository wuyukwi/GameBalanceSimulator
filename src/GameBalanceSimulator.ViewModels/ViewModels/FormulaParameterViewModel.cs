using CommunityToolkit.Mvvm.ComponentModel;
using GameBalanceSimulator.Core.Formulas;
using GameBalanceSimulator.ViewModels.Services;

namespace GameBalanceSimulator.ViewModels.ViewModels;

/// <summary>
/// ViewModel wrapper for a <see cref="FormulaParameter"/> that provides localized display names.
/// </summary>
public sealed partial class FormulaParameterViewModel : ObservableObject
{
    private readonly FormulaParameter _parameter;
    private readonly ILocalizationService _localizationService;

    [ObservableProperty]
    private string _displayName = string.Empty;

    public string Name => _parameter.Name;

    public double Value
    {
        get => _parameter.Value;
        set => _parameter.Value = value;
    }

    public double Minimum => _parameter.Minimum;

    public double Maximum => _parameter.Maximum;

    public double Increment => _parameter.Increment;

    public FormulaParameterViewModel(FormulaParameter parameter, ILocalizationService localizationService)
    {
        _parameter = parameter;
        _localizationService = localizationService;
        _parameter.PropertyChanged += (_, e) => OnPropertyChanged(e.PropertyName);
        UpdateDisplayName();
    }

    public void UpdateDisplayName()
    {
        DisplayName = _localizationService.GetString(_parameter.DescriptionKey);
    }
}
