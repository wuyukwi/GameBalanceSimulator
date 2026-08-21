using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameBalanceSimulator.Core.Formulas;
using GameBalanceSimulator.Core.Models;
using GameBalanceSimulator.Core.Services;
using GameBalanceSimulator.ViewModels.Services;

namespace GameBalanceSimulator.ViewModels.ViewModels;

public sealed partial class FormulaEditorViewModel : ViewModelBase
{
    private readonly IFormulaProvider _formulaProvider;
    private readonly ILocalizationService _localizationService;
    private readonly StatEditorViewModel _statEditor;

    [ObservableProperty]
    [Range(0, double.MaxValue)]
    private double _defenseMin;

    [ObservableProperty]
    [Range(0, double.MaxValue)]
    private double _defenseMax = 200;

    [ObservableProperty]
    [Range(0.1, double.MaxValue)]
    private double _defenseStep = 1;

    [ObservableProperty]
    private double[] _damageCurveX = Array.Empty<double>();

    [ObservableProperty]
    private double[] _damageCurveY = Array.Empty<double>();

    [ObservableProperty]
    private double[] _ttkCurveX = Array.Empty<double>();

    [ObservableProperty]
    private double[] _ttkCurveY = Array.Empty<double>();

    [ObservableProperty]
    private string _selectedFormulaDescription = string.Empty;

    public IReadOnlyList<IDamageFormula> AvailableFormulas => _formulaProvider.AvailableFormulas;

    public IDamageFormula SelectedFormula
    {
        get => _formulaProvider.CurrentFormula;
        set => _formulaProvider.CurrentFormula = value;
    }

    public FormulaEditorViewModel(
        IFormulaProvider formulaProvider,
        ILocalizationService localizationService,
        StatEditorViewModel statEditor)
    {
        _formulaProvider = formulaProvider;
        _localizationService = localizationService;
        _statEditor = statEditor;

        _formulaProvider.CurrentFormulaChanged += (_, _) =>
        {
            UpdateDescription();
            GenerateCurves();
        };
        _localizationService.CultureChanged += (_, _) => UpdateDescription();

        UpdateDescription();
        GenerateCurves();
    }

    [RelayCommand]
    private void GenerateCurves()
    {
        var attacker = _statEditor.Attacker.ToModel();
        var defenderTemplate = _statEditor.Defender.ToModel();
        var formula = _formulaProvider.CurrentFormula;

        var pointCount = Math.Max(2, (int)Math.Ceiling((DefenseMax - DefenseMin) / DefenseStep)) + 1;
        var damageX = new double[pointCount];
        var damageY = new double[pointCount];
        var ttkX = new double[pointCount];
        var ttkY = new double[pointCount];

        for (var i = 0; i < pointCount; i++)
        {
            var defense = DefenseMin + DefenseStep * i;
            if (defense > DefenseMax)
            {
                defense = DefenseMax;
            }

            var defender = defenderTemplate with { Defense = defense };
            var baseDamage = formula.CalculateBase(attacker, defender);
            var expectedDamage = baseDamage *
                                 (1.0 - defender.DodgeRate) *
                                 (1.0 - attacker.CriticalRate + attacker.CriticalRate * attacker.CriticalDamage);

            damageX[i] = defense;
            damageY[i] = expectedDamage;

            ttkX[i] = defense;
            ttkY[i] = expectedDamage > 0 ? Math.Ceiling(defender.MaxHealth / expectedDamage) : 0;
        }

        DamageCurveX = damageX;
        DamageCurveY = damageY;
        TtkCurveX = ttkX;
        TtkCurveY = ttkY;
    }

    private void UpdateDescription()
    {
        SelectedFormulaDescription = _localizationService.GetString(_formulaProvider.CurrentFormula.Description);
    }
}
