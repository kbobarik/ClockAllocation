using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using clock_allocation.Models;
using clock_allocation.ViewModels;

namespace clock_allocation.Views;

public partial class AddSpecializationView : UserControl
{
    public AddSpecializationView()
    {
        InitializeComponent();
        DataContext = new AddSpecializationViewModel();
    }

    public AddSpecializationView(Specialization specialization)
    {
        InitializeComponent();
        DataContext = new AddSpecializationViewModel(specialization);
    }
}