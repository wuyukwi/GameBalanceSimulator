using CommunityToolkit.Mvvm.ComponentModel;

namespace GameBalanceSimulator.ViewModels.ViewModels;

public sealed partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private StatEditorViewModel _statEditor;

    [ObservableProperty]
    private FormulaEditorViewModel _formulaEditor;

    [ObservableProperty]
    private SimulationViewModel _simulation;

    [ObservableProperty]
    private SettingsViewModel _settings;

    public MainViewModel(
        StatEditorViewModel statEditor,
        FormulaEditorViewModel formulaEditor,
        SimulationViewModel simulation,
        SettingsViewModel settings)
    {
        _statEditor = statEditor;
        _formulaEditor = formulaEditor;
        _simulation = simulation;
        _settings = settings;

        WireAutoCurveUpdate();
    }

    private void WireAutoCurveUpdate()
    {
        void OnStatChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) =>
            FormulaEditor.GenerateCurvesCommand.Execute(null);

        StatEditor.Attacker.PropertyChanged += OnStatChanged;
        StatEditor.Defender.PropertyChanged += OnStatChanged;
    }
}
