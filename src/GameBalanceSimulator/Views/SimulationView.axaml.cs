using Avalonia.Controls;
using GameBalanceSimulator.ViewModels.ViewModels;
using ScottPlot;

namespace GameBalanceSimulator.Views;

public partial class SimulationView : UserControl
{
    public SimulationView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is SimulationViewModel oldViewModel)
        {
            oldViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        if (DataContext is SimulationViewModel newViewModel)
        {
            newViewModel.PropertyChanged += OnViewModelPropertyChanged;
            RenderHistogram(newViewModel);
        }
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (DataContext is not SimulationViewModel viewModel)
        {
            return;
        }

        if (e.PropertyName == nameof(SimulationViewModel.Report))
        {
            RenderHistogram(viewModel);
        }
    }

    private void RenderHistogram(SimulationViewModel viewModel)
    {
        HistogramPlot.Plot.Clear();

        var report = viewModel.Report;
        if (report is null || report.HistogramBuckets.Count == 0)
        {
            HistogramPlot.Refresh();
            return;
        }

        var buckets = report.HistogramBuckets;
        var edges = report.HistogramBinEdges;
        var bars = new Bar[buckets.Count];

        for (var i = 0; i < buckets.Count; i++)
        {
            var center = (edges[i] + edges[i + 1]) / 2.0;
            var width = edges[i + 1] - edges[i];
            bars[i] = new Bar
            {
                Position = center,
                Value = buckets[i],
                Size = width
            };
        }

        HistogramPlot.Plot.Add.Bars(bars);
        HistogramPlot.Refresh();
    }
}
