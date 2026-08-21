using System.Globalization;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GameBalanceSimulator.Core.Formulas;
using GameBalanceSimulator.Core.Persistence;
using GameBalanceSimulator.Core.Services;
using GameBalanceSimulator.Services;
using GameBalanceSimulator.ViewModels.Services;
using GameBalanceSimulator.ViewModels.ViewModels;
using GameBalanceSimulator.Views;
using Microsoft.Extensions.DependencyInjection;

namespace GameBalanceSimulator;

public partial class App : Application
{
    public IServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        Services = ConfigureServices(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var localizationService = Services.GetRequiredService<ILocalizationService>();
        if (localizationService is AvaloniaResourceLocalizationService avaloniaLocalization)
        {
            avaloniaLocalization.Initialize();
        }

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static IServiceProvider ConfigureServices(Application application)
    {
        var services = new ServiceCollection();

        services.AddSingleton<IDamageFormula, SubtractiveDamageFormula>();
        services.AddSingleton<IDamageFormula, MultiplicativeDamageFormula>();

        services.AddSingleton<IFormulaProvider>(provider =>
            new FormulaProvider(provider.GetServices<IDamageFormula>()));

        services.AddSingleton<ILocalizationService>(provider =>
            new AvaloniaResourceLocalizationService(
                application,
                new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("zh"),
                    new CultureInfo("ja")
                }));

        services.AddSingleton<IDialogService, AvaloniaDialogService>();
        services.AddSingleton<ISimulationConfigRepository, JsonConfigRepository>();

        services.AddSingleton<StatEditorViewModel>();
        services.AddSingleton<FormulaEditorViewModel>();
        services.AddSingleton<SimulationViewModel>();
        services.AddSingleton<SettingsViewModel>();
        services.AddSingleton<MainViewModel>();

        return services.BuildServiceProvider();
    }
}
