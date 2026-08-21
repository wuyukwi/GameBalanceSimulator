using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameBalanceSimulator.Core.Models;
using GameBalanceSimulator.Core.Services;
using GameBalanceSimulator.Core.Simulation;

namespace GameBalanceSimulator.ViewModels.ViewModels;

public sealed partial class SimulationViewModel : ViewModelBase
{
    private readonly IFormulaProvider _formulaProvider;
    private readonly StatEditorViewModel _statEditor;
    private IBattleSimulator _simulator;
    private CancellationTokenSource? _cancellationTokenSource;

    [ObservableProperty]
    [Range(1, int.MaxValue)]
    private int _iterationCount = 10000;

    [ObservableProperty]
    [Range(0, int.MaxValue)]
    private int _seed;

    [ObservableProperty]
    private bool _simulateUntilDeath = true;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StartSimulationCommand))]
    private bool _isRunning;

    [ObservableProperty]
    private double _progressValue;

    [ObservableProperty]
    private double _progressMaximum = 100;

    [ObservableProperty]
    private SimulationReport? _report;

    public SimulationViewModel(IFormulaProvider formulaProvider, StatEditorViewModel statEditor)
    {
        _formulaProvider = formulaProvider;
        _statEditor = statEditor;
        _simulator = new MonteCarloBattleSimulator(_formulaProvider.CurrentFormula);
        _formulaProvider.CurrentFormulaChanged += (_, _) =>
            _simulator = new MonteCarloBattleSimulator(_formulaProvider.CurrentFormula);
    }

    [RelayCommand(CanExecute = nameof(CanStartSimulation))]
    private async Task StartSimulationAsync()
    {
        IsRunning = true;
        ProgressValue = 0;
        _cancellationTokenSource = new CancellationTokenSource();
        var progress = new Progress<double>(value => ProgressValue = value * ProgressMaximum);

        var config = new SimulationConfig(
            _statEditor.Attacker.ToModel(),
            _statEditor.Defender.ToModel(),
            IterationCount,
            _formulaProvider.CurrentFormula.Name,
            SimulateUntilDeath,
            Seed);

        try
        {
            Report = await Task.Run(() => _simulator.Run(config, progress, _cancellationTokenSource.Token), _cancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            // User cancelled the run; leave the previous report intact.
        }
        finally
        {
            IsRunning = false;
            ProgressValue = 0;
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
    }

    private bool CanStartSimulation() => !IsRunning && IterationCount > 0;

    [RelayCommand]
    private void CancelSimulation()
    {
        _cancellationTokenSource?.Cancel();
    }
}
