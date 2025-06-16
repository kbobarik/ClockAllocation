using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using clock_allocation.ViewModels;

namespace clock_allocation.Views;

public partial class CurrentGroupView : UserControl
{
    public CurrentGroupView(int idSpecialisation, int year)
    {
        InitializeComponent();
        DataContext = new CurrentGroupViewModel(idSpecialisation, year);
    }
}