using Avalonia.Controls;
using GameBalanceSimulator.ViewModels.ViewModels;

namespace GameBalanceSimulator.Views;

public partial class FormulaEditorView : UserControl
{
    public FormulaEditorView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is FormulaEditorViewModel oldViewModel)
        {
            oldViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        if (DataContext is FormulaEditorViewModel newViewModel)
        {
            newViewModel.PropertyChanged += OnViewModelPropertyChanged;
            RenderPlots(newViewModel);
        }
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (DataContext is not FormulaEditorViewModel viewModel)
        {
            return;
        }

        if (e.PropertyName is nameof(FormulaEditorViewModel.DamageCurveX) or nameof(FormulaEditorViewModel.DamageCurveY))
        {
            RenderDamagePlot(viewModel);
        }
        else if (e.PropertyName is nameof(FormulaEditorViewModel.TtkCurveX) or nameof(FormulaEditorViewModel.TtkCurveY))
        {
            RenderTtkPlot(viewModel);
        }
    }

    private void RenderPlots(FormulaEditorViewModel viewModel)
    {
        RenderDamagePlot(viewModel);
        RenderTtkPlot(viewModel);
    }

    private void RenderDamagePlot(FormulaEditorViewModel viewModel)
    {
        DamagePlot.Plot.Clear();
        if (viewModel.DamageCurveX.Length > 0)
        {
            DamagePlot.Plot.Add.Scatter(viewModel.DamageCurveX, viewModel.DamageCurveY);
        }
        DamagePlot.Refresh();
    }

    private void RenderTtkPlot(FormulaEditorViewModel viewModel)
    {
        TtkPlot.Plot.Clear();
        if (viewModel.TtkCurveX.Length > 0)
        {
            TtkPlot.Plot.Add.Scatter(viewModel.TtkCurveX, viewModel.TtkCurveY);
        }
        TtkPlot.Refresh();
    }
}
