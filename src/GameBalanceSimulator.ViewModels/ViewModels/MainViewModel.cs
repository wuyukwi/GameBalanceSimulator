using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameBalanceSimulator.Core.Formulas;
using GameBalanceSimulator.Core.Models;
using GameBalanceSimulator.Core.Persistence;
using GameBalanceSimulator.Core.Reporting;
using GameBalanceSimulator.Core.Services;
using GameBalanceSimulator.ViewModels.Services;

namespace GameBalanceSimulator.ViewModels.ViewModels;

public sealed partial class MainViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly IFormulaProvider _formulaProvider;
    private readonly IReportExporter _reportExporter;
    private readonly ISimulationConfigRepository _repository;

    [ObservableProperty]
    private StatEditorViewModel _statEditor;

    [ObservableProperty]
    private FormulaEditorViewModel _formulaEditor;

    [ObservableProperty]
    private SimulationViewModel _simulation;

    [ObservableProperty]
    private SettingsViewModel _settings;

    public MainViewModel(
        IDialogService dialogService,
        IFormulaProvider formulaProvider,
        IReportExporter reportExporter,
        ISimulationConfigRepository repository,
        StatEditorViewModel statEditor,
        FormulaEditorViewModel formulaEditor,
        SimulationViewModel simulation,
        SettingsViewModel settings)
    {
        _dialogService = dialogService;
        _formulaProvider = formulaProvider;
        _reportExporter = reportExporter;
        _repository = repository;
        _statEditor = statEditor;
        _formulaEditor = formulaEditor;
        _simulation = simulation;
        _settings = settings;

        WireAutoCurveUpdate();
    }

    [RelayCommand]
    private async Task SaveConfigAsync()
    {
        var path = await _dialogService.ShowSaveFileAsync("Save Configuration", "JSON files (*.json)|*.json");
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        if (!path.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            path += ".json";
        }

        var formulaParameters = _formulaProvider.CurrentFormula.Parameters
            .ToDictionary(p => p.Name, p => p.Value);

        var config = new SimulationConfig(
            StatEditor.Attacker.ToModel(),
            StatEditor.Defender.ToModel(),
            Simulation.IterationCount,
            _formulaProvider.CurrentFormula.Name,
            Simulation.SimulateUntilDeath,
            Simulation.Seed,
            formulaParameters);

        await _repository.SaveAsync(config, path);
        await _dialogService.ShowInfoAsync($"Configuration saved to:\n{path}", "Save Configuration");
    }

    [RelayCommand]
    private async Task LoadConfigAsync()
    {
        var path = await _dialogService.ShowOpenFileAsync("Load Configuration", "JSON files (*.json)|*.json");
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        var config = await _repository.LoadAsync(path);
        if (config is null)
        {
            await _dialogService.ShowInfoAsync("Failed to load configuration.", "Load Configuration");
            return;
        }

        ApplyConfig(config);
        await _dialogService.ShowInfoAsync("Configuration loaded successfully.", "Load Configuration");
    }

    [RelayCommand]
    private async Task ExportReportAsync()
    {
        var path = await _dialogService.ShowSaveFileAsync("Export Report", "Markdown files (*.md)|*.md");
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        if (!path.EndsWith(_reportExporter.Extension, StringComparison.OrdinalIgnoreCase))
        {
            path += _reportExporter.Extension;
        }

        var outputDirectory = Path.GetDirectoryName(path);
        if (string.IsNullOrWhiteSpace(outputDirectory))
        {
            outputDirectory = Directory.GetCurrentDirectory();
        }

        var formulaParameters = _formulaProvider.CurrentFormula.Parameters
            .ToDictionary(p => p.Name, p => p.Value);

        var reportData = new ReportData(
            DateTime.Now,
            StatEditor.Attacker.ToModel(),
            StatEditor.Defender.ToModel(),
            _formulaProvider.CurrentFormula,
            FormulaEditor.DefenseMin,
            FormulaEditor.DefenseMax,
            FormulaEditor.DefenseStep,
            FormulaEditor.DamageCurveX,
            FormulaEditor.DamageCurveY,
            FormulaEditor.TtkCurveX,
            FormulaEditor.TtkCurveY,
            new SimulationConfig(
                StatEditor.Attacker.ToModel(),
                StatEditor.Defender.ToModel(),
                Simulation.IterationCount,
                _formulaProvider.CurrentFormula.Name,
                Simulation.SimulateUntilDeath,
                Simulation.Seed,
                formulaParameters),
            Simulation.Report);

        var content = _reportExporter.Export(reportData, outputDirectory);
        await File.WriteAllTextAsync(path, content);
        await _dialogService.ShowInfoAsync($"Report exported to:\n{path}", "Export Report");
    }

    private void ApplyConfig(SimulationConfig config)
    {
        StatEditor.Attacker = new StatBlockViewModel(config.Attacker);
        StatEditor.Defender = new StatBlockViewModel(config.Defender);

        var formula = _formulaProvider.AvailableFormulas.FirstOrDefault(f => f.Name == config.FormulaName);
        if (formula is not null)
        {
            _formulaProvider.CurrentFormula = formula;
        }

        var parameters = config.FormulaParameters ?? new Dictionary<string, double>();
        foreach (var parameter in _formulaProvider.CurrentFormula.Parameters)
        {
            if (parameters.TryGetValue(parameter.Name, out var value))
            {
                parameter.Value = value;
            }
        }

        Simulation.IterationCount = config.IterationCount;
        Simulation.Seed = config.Seed;
        Simulation.SimulateUntilDeath = config.SimulateUntilDeath;
    }

    private StatBlockViewModel? _lastAttacker;
    private StatBlockViewModel? _lastDefender;

    private void WireAutoCurveUpdate()
    {
        void OnStatBlockChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) =>
            FormulaEditor.GenerateCurvesCommand.Execute(null);

        void OnStatEditorChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StatEditorViewModel.Attacker))
            {
                if (_lastAttacker is not null)
                {
                    _lastAttacker.PropertyChanged -= OnStatBlockChanged;
                }

                _lastAttacker = StatEditor.Attacker;
                _lastAttacker.PropertyChanged += OnStatBlockChanged;
                FormulaEditor.GenerateCurvesCommand.Execute(null);
            }
            else if (e.PropertyName == nameof(StatEditorViewModel.Defender))
            {
                if (_lastDefender is not null)
                {
                    _lastDefender.PropertyChanged -= OnStatBlockChanged;
                }

                _lastDefender = StatEditor.Defender;
                _lastDefender.PropertyChanged += OnStatBlockChanged;
                FormulaEditor.GenerateCurvesCommand.Execute(null);
            }
        }

        _lastAttacker = StatEditor.Attacker;
        _lastDefender = StatEditor.Defender;
        _lastAttacker.PropertyChanged += OnStatBlockChanged;
        _lastDefender.PropertyChanged += OnStatBlockChanged;
        StatEditor.PropertyChanged += OnStatEditorChanged;
    }
}
