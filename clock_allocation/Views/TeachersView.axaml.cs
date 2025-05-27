using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using clock_allocation.ViewModels;

namespace clock_allocation.Views;

public partial class TeachersView : UserControl
{
    public TeachersView()
    {
        InitializeComponent();
        DataContext = new TeachersViewModel();
    }
}