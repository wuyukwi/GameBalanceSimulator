using GameBalanceSimulator.Core.Models;

namespace GameBalanceSimulator.Core.Persistence;

public interface ISimulationConfigRepository
{
    Task SaveAsync(SimulationConfig config, string filePath, CancellationToken cancellationToken = default);

    Task<SimulationConfig?> LoadAsync(string filePath, CancellationToken cancellationToken = default);
}
