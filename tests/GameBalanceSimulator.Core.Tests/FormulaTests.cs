using FluentAssertions;
using GameBalanceSimulator.Core.Formulas;
using GameBalanceSimulator.Core.Models;
using GameBalanceSimulator.Core.Services;
using Xunit;

namespace GameBalanceSimulator.Core.Tests;

public class FormulaTests
{
    private static readonly StatBlock Attacker = new()
    {
        BaseAttack = 100,
        ArmorPenetration = 10,
        CriticalRate = 0,
        CriticalDamage = 1.5
    };

    [Fact]
    public void Subtractive_BaseDamage_HonorsArmorAndPenetration()
    {
        var formula = new SubtractiveDamageFormula();
        var defender = new StatBlock { Defense = 80 };

        var baseDamage = formula.CalculateBase(Attacker, defender);

        baseDamage.Should().Be(30);
    }

    [Fact]
    public void Subtractive_BaseDamage_IsAtLeastOne()
    {
        var formula = new SubtractiveDamageFormula();
        var defender = new StatBlock { Defense = 200 };

        var baseDamage = formula.CalculateBase(Attacker, defender);

        baseDamage.Should().Be(1);
    }

    [Fact]
    public void Subtractive_DodgedAttack_ReturnsZeroDamage()
    {
        var formula = new SubtractiveDamageFormula();
        var attacker = Attacker with { CriticalRate = 0 };
        var defender = new StatBlock { DodgeRate = 1.0 };
        var random = new FixedRandomProvider(0.0);

        var result = formula.Calculate(attacker, defender, random);

        result.IsDodged.Should().BeTrue();
        result.FinalDamage.Should().Be(0);
    }

    [Fact]
    public void Multiplicative_BaseDamage_ComputesCorrectly()
    {
        var formula = new MultiplicativeDamageFormula();
        var attacker = new StatBlock { BaseAttack = 100 };
        var defender = new StatBlock { Defense = 100 };

        var baseDamage = formula.CalculateBase(attacker, defender);

        baseDamage.Should().Be(50);
    }

    [Fact]
    public void Multiplicative_BaseDamage_IsAtLeastOne()
    {
        var formula = new MultiplicativeDamageFormula();
        var attacker = new StatBlock { BaseAttack = 10 };
        var defender = new StatBlock { Defense = 1000 };

        var baseDamage = formula.CalculateBase(attacker, defender);

        baseDamage.Should().BeGreaterThanOrEqualTo(1);
    }

    private sealed class FixedRandomProvider : IRandomProvider
    {
        private readonly double _value;

        public FixedRandomProvider(double value)
        {
            _value = value;
        }

        public double NextDouble() => _value;

        public int Next(int minValue, int maxValue) => minValue;
    }
}
