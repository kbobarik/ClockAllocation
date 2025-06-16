using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using clock_allocation.Models;
using clock_allocation.ViewModels;

namespace clock_allocation.Views;

public partial class AddGroupView : UserControl
{
    public AddGroupView(Specialization specialization)
    {
        InitializeComponent();
        DataContext = new AddGroupViewModel(specialization);
    }
    public AddGroupView(Specialization specialization, int year)
    {
        InitializeComponent();
        DataContext = new AddGroupViewModel(specialization, year);
    }
}