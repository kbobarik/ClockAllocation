using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using clock_allocation.ViewModels;

namespace clock_allocation.Views;

public partial class ModuleAndSubjectView : UserControl
{
    public ModuleAndSubjectView()
    {
        InitializeComponent();
        DataContext = new ModuleAndSubjectViewModel();
    }
}