namespace GameBalanceSimulator.Core.Services;

/// <summary>
/// 随机数抽象，便于单元测试时注入固定种子。
/// </summary>
public interface IRandomProvider
{
    double NextDouble();

    int Next(int minValue, int maxValue);
}
