using System.Text.Encodings.Web;
using System.Text.Json;
using GameBalanceSimulator.Core.Models;

namespace GameBalanceSimulator.Core.Persistence;

public sealed class JsonConfigRepository : ISimulationConfigRepository
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public async Task SaveAsync(SimulationConfig config, string filePath, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(config, Options);
        await File.WriteAllTextAsync(filePath, json, cancellationToken);
    }

    public async Task<SimulationConfig?> LoadAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
        {
            return null;
        }

        var json = await File.ReadAllTextAsync(filePath, cancellationToken);
        return JsonSerializer.Deserialize<SimulationConfig>(json, Options);
    }
}
