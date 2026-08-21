using FluentAssertions;
using GameBalanceSimulator.Core.Formulas;
using GameBalanceSimulator.Core.Models;
using GameBalanceSimulator.Core.Simulation;
using Xunit;

namespace GameBalanceSimulator.Core.Tests;

public class SimulatorTests
{
    private static readonly StatBlock Attacker = new()
    {
        BaseAttack = 50,
        CriticalRate = 0,
        CriticalDamage = 1.5,
        DodgeRate = 0
    };

    private static readonly StatBlock Defender = new()
    {
        Defense = 20,
        DodgeRate = 0,
        MaxHealth = 100
    };

    [Fact]
    public void MonteCarlo_SingleAttack_ReturnsExpectedIterationCount()
    {
        var simulator = new MonteCarloBattleSimulator(new SubtractiveDamageFormula());
        var config = new SimulationConfig(Attacker, Defender, 1000, "Subtractive", false, 42);

        var report = simulator.Run(config);

        report.IterationCount.Should().Be(1000);
        report.AverageDamage.Should().BeGreaterThan(0);
        report.MaxDamage.Should().BeGreaterThanOrEqualTo(report.AverageDamage);
    }

    [Fact]
    public void MonteCarlo_UntilDeath_ReturnsReasonableTtk()
    {
        var simulator = new MonteCarloBattleSimulator(new SubtractiveDamageFormula());
        var config = new SimulationConfig(Attacker, Defender, 1000, "Subtractive", true, 42);

        var report = simulator.Run(config);

        report.AverageTtk.Should().BeGreaterThan(0);
        report.AverageTtk.Should().BeApproximately(3.0, 1.0);
    }

    [Fact]
    public void MonteCarlo_Cancellation_StopsEarly()
    {
        var simulator = new MonteCarloBattleSimulator(new SubtractiveDamageFormula());
        var config = new SimulationConfig(Attacker, Defender, 10_000_000, "Subtractive", false, 42);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        Action act = () => simulator.Run(config, cts.Token);

        act.Should().Throw<OperationCanceledException>();
    }
}
