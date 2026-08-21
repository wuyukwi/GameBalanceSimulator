using GameBalanceSimulator.Core.Models;

namespace GameBalanceSimulator.Core.Simulation;

public interface IBattleSimulator
{
    SimulationReport Run(SimulationConfig config, CancellationToken cancellationToken = default);

    SimulationReport Run(SimulationConfig config, IProgress<double> progress, CancellationToken cancellationToken = default);
}
