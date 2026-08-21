using CommunityToolkit.Mvvm.ComponentModel;
using GameBalanceSimulator.Core.Models;

namespace GameBalanceSimulator.ViewModels.ViewModels;

public sealed partial class StatEditorViewModel : ViewModelBase
{
    [ObservableProperty]
    private StatBlockViewModel _attacker;

    [ObservableProperty]
    private StatBlockViewModel _defender;

    public StatEditorViewModel()
    {
        _attacker = new StatBlockViewModel(StatBlock.DefaultAttacker);
        _defender = new StatBlockViewModel(StatBlock.DefaultDefender);
    }

    public void ResetToDefaults()
    {
        Attacker = new StatBlockViewModel(StatBlock.DefaultAttacker);
        Defender = new StatBlockViewModel(StatBlock.DefaultDefender);
    }
}
