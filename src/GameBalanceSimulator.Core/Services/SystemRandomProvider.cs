namespace GameBalanceSimulator.Core.Services;

public sealed class SystemRandomProvider : IRandomProvider
{
    private readonly Random _random;

    public SystemRandomProvider()
    {
        _random = new Random();
    }

    public SystemRandomProvider(int seed)
    {
        _random = new Random(seed);
    }

    public double NextDouble() => _random.NextDouble();

    public int Next(int minValue, int maxValue) => _random.Next(minValue, maxValue);
}
