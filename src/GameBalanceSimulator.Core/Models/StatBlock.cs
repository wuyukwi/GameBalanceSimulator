namespace GameBalanceSimulator.Core.Models;

/// <summary>
/// Snapshot of combat statistics. All values are non-negative; semantic validation is the caller's responsibility.
/// </summary>
public sealed record StatBlock
{
    public double BaseAttack { get; init; }
    public double Defense { get; init; }
    public double CriticalRate { get; init; }
    public double CriticalDamage { get; init; }
    public double DodgeRate { get; init; }
    public double ArmorPenetration { get; init; }
    public double AttackInterval { get; init; }
    public double MaxHealth { get; init; }

    public static StatBlock DefaultAttacker => new()
    {
        BaseAttack = 100,
        Defense = 20,
        CriticalRate = 0.15,
        CriticalDamage = 2.0,
        DodgeRate = 0.05,
        ArmorPenetration = 0,
        AttackInterval = 1.0,
        MaxHealth = 500
    };

    public static StatBlock DefaultDefender => new()
    {
        BaseAttack = 40,
        Defense = 60,
        CriticalRate = 0.1,
        CriticalDamage = 1.5,
        DodgeRate = 0.1,
        ArmorPenetration = 0,
        AttackInterval = 1.5,
        MaxHealth = 800
    };
}
