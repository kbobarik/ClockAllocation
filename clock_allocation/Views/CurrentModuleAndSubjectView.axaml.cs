using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using clock_allocation.Models;
using clock_allocation.ViewModels;

namespace clock_allocation.Views;

public partial class CurrentModuleAndSubjectView : UserControl
{
    public CurrentModuleAndSubjectView(Curriculum modules, Specialization specialization)
    {
        InitializeComponent();
        DataContext = new CurrentModuleAndSubjectViewModel(modules,specialization);
    }
}