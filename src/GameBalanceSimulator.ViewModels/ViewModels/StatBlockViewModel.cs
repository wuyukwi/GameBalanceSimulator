using CommunityToolkit.Mvvm.ComponentModel;
using GameBalanceSimulator.Core.Models;

namespace GameBalanceSimulator.ViewModels.ViewModels;

public sealed partial class StatBlockViewModel : ViewModelBase
{
    [ObservableProperty]
    private double _baseAttack;

    [ObservableProperty]
    private double _defense;

    [ObservableProperty]
    private double _criticalRate;

    [ObservableProperty]
    private double _criticalDamage;

    [ObservableProperty]
    private double _dodgeRate;

    [ObservableProperty]
    private double _armorPenetration;

    [ObservableProperty]
    private double _attackInterval;

    [ObservableProperty]
    private double _maxHealth;

    public StatBlockViewModel(StatBlock model)
    {
        BaseAttack = model.BaseAttack;
        Defense = model.Defense;
        CriticalRate = model.CriticalRate;
        CriticalDamage = model.CriticalDamage;
        DodgeRate = model.DodgeRate;
        ArmorPenetration = model.ArmorPenetration;
        AttackInterval = model.AttackInterval;
        MaxHealth = model.MaxHealth;
    }

    public StatBlock ToModel()
    {
        return new StatBlock
        {
            BaseAttack = BaseAttack,
            Defense = Defense,
            CriticalRate = CriticalRate,
            CriticalDamage = CriticalDamage,
            DodgeRate = DodgeRate,
            ArmorPenetration = ArmorPenetration,
            AttackInterval = AttackInterval,
            MaxHealth = MaxHealth
        };
    }
}
