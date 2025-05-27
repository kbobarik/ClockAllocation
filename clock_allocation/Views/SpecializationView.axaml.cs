using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using clock_allocation.ViewModels;

namespace clock_allocation.Views;

public partial class SpecializationView : UserControl
{
    public SpecializationView()
    {
        InitializeComponent();
        DataContext = new SpecializationViewModel();
    }
}