using GameBalanceSimulator.Core.Formulas;
using GameBalanceSimulator.Core.Models;
using GameBalanceSimulator.Core.Services;

namespace GameBalanceSimulator.Core.Simulation;

public sealed class MonteCarloBattleSimulator : IBattleSimulator
{
    private readonly IDamageFormula _formula;

    public MonteCarloBattleSimulator(IDamageFormula formula)
    {
        _formula = formula ?? throw new ArgumentNullException(nameof(formula));
    }

    public SimulationReport Run(SimulationConfig config, CancellationToken cancellationToken = default)
        => Run(config, null, cancellationToken);

    public SimulationReport Run(SimulationConfig config, IProgress<double>? progress, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(config.IterationCount);

        var random = config.Seed.HasValue
            ? new SystemRandomProvider(config.Seed.Value)
            : new SystemRandomProvider();

        var iterationCount = config.IterationCount;
        var progressInterval = Math.Max(1, iterationCount / 100);

        double totalDamage = 0;
        double maxDamage = double.MinValue;
        double minDamage = double.MaxValue;
        long totalTtk = 0;
        long criticalCount = 0;
        long dodgeCount = 0;

        var damages = new double[iterationCount];

        for (var i = 0; i < iterationCount; i++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            var result = _formula.Calculate(config.Attacker, config.Defender, random);
            var damage = result.FinalDamage;

            double iterationDamage;

            if (config.SimulateUntilDeath)
            {
                var remainingHealth = config.Defender.MaxHealth;
                var attacks = 0;
                var battleTotalDamage = 0.0;

                while (remainingHealth > 0)
                {
                    if (attacks > 0)
                    {
                        result = _formula.Calculate(config.Attacker, config.Defender, random);
                        damage = result.FinalDamage;
                    }

                    remainingHealth -= damage;
                    battleTotalDamage += damage;
                    attacks++;

                    // Guard against pathological cases such as zero damage per attack.
                    if (attacks > 10_000)
                    {
                        break;
                    }
                }

                totalTtk += attacks;
                iterationDamage = battleTotalDamage;
            }
            else
            {
                iterationDamage = damage;
            }

            damages[i] = iterationDamage;
            totalDamage += iterationDamage;

            if (iterationDamage > maxDamage) maxDamage = iterationDamage;
            if (iterationDamage < minDamage) minDamage = iterationDamage;
            if (result.IsCritical) criticalCount++;
            if (result.IsDodged) dodgeCount++;

            if (progress is not null && i % progressInterval == 0)
            {
                progress.Report((double)i / iterationCount);
            }
        }

        progress?.Report(1.0);

        var histogram = BuildHistogram(damages, minDamage, maxDamage);

        return new SimulationReport
        {
            IterationCount = iterationCount,
            AverageDamage = totalDamage / iterationCount,
            MaxDamage = maxDamage == double.MinValue ? 0 : maxDamage,
            MinDamage = minDamage == double.MaxValue ? 0 : minDamage,
            AverageTtk = config.SimulateUntilDeath ? (double)totalTtk / iterationCount : 0,
            CriticalCount = criticalCount,
            DodgeCount = dodgeCount,
            CriticalRate = (double)criticalCount / iterationCount,
            DodgeRate = (double)dodgeCount / iterationCount,
            HistogramBuckets = histogram.Buckets,
            HistogramBinEdges = histogram.BinEdges
        };
    }

    private static (IReadOnlyList<int> Buckets, IReadOnlyList<double> BinEdges) BuildHistogram(double[] damages, double min, double max)
    {
        const int bucketCount = 20;

        if (damages.Length == 0 || Math.Abs(max - min) < double.Epsilon)
        {
            return (Array.Empty<int>(), Array.Empty<double>());
        }

        var buckets = new int[bucketCount];
        var binEdges = new double[bucketCount + 1];
        var step = (max - min) / bucketCount;

        for (var i = 0; i <= bucketCount; i++)
        {
            binEdges[i] = min + step * i;
        }

        foreach (var damage in damages)
        {
            var index = (int)((damage - min) / step);
            if (index >= bucketCount) index = bucketCount - 1;
            if (index < 0) index = 0;
            buckets[index]++;
        }

        return (buckets, binEdges);
    }
}
