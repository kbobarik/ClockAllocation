using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using clock_allocation.ViewModels;

namespace clock_allocation.Views;

public partial class GroupView : UserControl
{
    public GroupView()
    {
        InitializeComponent();
        DataContext = new GroupViewModel();
    }
}