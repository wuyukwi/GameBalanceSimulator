using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using GameBalanceSimulator.Core.Models;

namespace GameBalanceSimulator.ViewModels.ViewModels;

public sealed partial class StatBlockViewModel : ViewModelBase
{
    [ObservableProperty]
    [Range(0, double.MaxValue)]
    private double _baseAttack;

    [ObservableProperty]
    [Range(0, double.MaxValue)]
    private double _defense;

    [ObservableProperty]
    [Range(0, 1)]
    private double _criticalRate;

    [ObservableProperty]
    [Range(1, double.MaxValue)]
    private double _criticalDamage;

    [ObservableProperty]
    [Range(0, 1)]
    private double _dodgeRate;

    [ObservableProperty]
    [Range(0, double.MaxValue)]
    private double _armorPenetration;

    [ObservableProperty]
    [Range(0.1, double.MaxValue)]
    private double _attackInterval;

    [ObservableProperty]
    [Range(1, double.MaxValue)]
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
