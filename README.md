# Game Balance Simulator

A cross-platform desktop tool for game developers and planners to simulate and visualize combat balance.

[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/)
[![Avalonia](https://img.shields.io/badge/Avalonia-12-purple)](https://avaloniaui.net/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

## Overview

Game Balance Simulator helps you explore how different stats and damage formulas affect combat outcomes. It provides an interactive way to tweak attacker/defender attributes, compare damage formulas, visualize damage curves, and run Monte Carlo battle simulations.

## Features

- **Stat Editor**: Configure attacker and defender attributes such as attack, defense, critical rate, dodge rate, armor penetration, attack interval, and health.
- **Damage Formula Presets**:
  - Subtractive armor model: `Damage = max(1, Attack - max(0, Defense - Penetration))`
  - Multiplicative armor model: `Damage = Attack * Attack / (Attack + max(0, Defense - Penetration))`
  - Percentage reduction model: `Damage = Attack * max(0.1, 1 - Defense / (Defense + 100))`
  - True damage model: ignores defense and armor penetration
- **Formula Visualization**: Plot expected damage and time-to-kill (TTK) curves as defense varies, with a localized formula description shown in the UI.
- **Monte Carlo Simulation**: Run thousands of combat iterations to obtain statistical results including average damage, max/min damage, average TTK, critical rate, and dodge rate. Results include a damage distribution histogram.
- **Save / Load Configuration**: Persist and restore simulation setups through the File menu.
- **Input Validation**: All numeric inputs use range validation via `ObservableValidator`.
- **Multilingual UI**: Supports English, Chinese, and Japanese with runtime language switching. New languages can be added by adding a resource dictionary.
- **MVVM Architecture**: Built with `CommunityToolkit.Mvvm` for clean separation between Model, ViewModel, and View.

## Tech Stack

| Layer | Technology |
| --- | --- |
| UI Framework | Avalonia UI 12 |
| Runtime | .NET 8 |
| Architecture | MVVM with CommunityToolkit.Mvvm |
| Charting | ScottPlot.Avalonia 5 |
| DI Container | Microsoft.Extensions.DependencyInjection |
| Testing | xUnit + FluentAssertions |

## Project Structure

```text
GameBalanceSimulator/
├── src/
│   ├── GameBalanceSimulator/              # Avalonia desktop application
│   │   ├── Assets/Strings/                # Localization resource dictionaries
│   │   ├── Services/                      # Avalonia-specific services (localization, dialogs)
│   │   └── Views/                         # Avalonia views (XAML + code-behind)
│   ├── GameBalanceSimulator.Core/         # Models, formulas, simulation, persistence
│   │   ├── Formulas/
│   │   ├── Models/
│   │   ├── Persistence/
│   │   ├── Services/
│   │   └── Simulation/
│   └── GameBalanceSimulator.ViewModels/   # ViewModels and UI abstractions
│       ├── Services/
│       └── ViewModels/
└── tests/
    └── GameBalanceSimulator.Core.Tests/   # Unit tests for core logic
```

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- (Optional) [PowerShell 7+](https://github.com/PowerShell/PowerShell) or a POSIX shell for convenience scripts
- (Optional) [Visual Studio Code](https://code.visualstudio.com/) with the C# Dev Kit extension

### Quick Start

The fastest way to run the application is using the provided convenience scripts:

**Windows (PowerShell):**
```powershell
.\scripts\run.ps1
```

**macOS / Linux:**
```bash
./scripts/run.sh
```

### Build

```bash
dotnet build
```

Or build only the desktop project:

```bash
dotnet build src/GameBalanceSimulator/GameBalanceSimulator.csproj
```

### Run

If you prefer to run manually:

```bash
dotnet run --project src/GameBalanceSimulator/GameBalanceSimulator.csproj
```

### Run Tests

Run all unit tests from the solution root:

```bash
dotnet test
```

If you want detailed per-test output:

```bash
dotnet test --verbosity normal
```

Or use the convenience script:

```powershell
.\scripts\test.ps1
```

```bash
./scripts/test.sh
```

### Publish a Standalone Executable

To create a single-file executable that does **not** require .NET runtime on the target machine:

**Windows:**
```powershell
.\scripts\publish.ps1 -Runtime win-x64 -Output publish
```

**macOS (Apple Silicon):**
```bash
./scripts/publish.sh osx-arm64 publish
```

**macOS (Intel):**
```bash
./scripts/publish.sh osx-x64 publish
```

**Linux:**
```bash
./scripts/publish.sh linux-x64 publish
```

The output executable will be located at:

- Windows: `publish/GameBalanceSimulator.exe`
- macOS / Linux: `publish/GameBalanceSimulator`

### Visual Studio Code

If you use VS Code, open the workspace and press `F5` to launch the application. Pre-configured tasks are provided in `.vscode/tasks.json` and `.vscode/launch.json` for build, test, and publish.

## Localization

Language files are located in `src/GameBalanceSimulator/Assets/Strings/`:

- `Strings.en.axaml`
- `Strings.zh.axaml`
- `Strings.ja.axaml`

To add a new language, create a new resource dictionary following the naming convention `Strings.{culture}.axaml`, register the culture in `App.axaml.cs`, and the UI will pick it up automatically.

## License

This project is licensed under the MIT License.
