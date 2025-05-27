using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using clock_allocation.Models;
using clock_allocation.ViewModels;

namespace clock_allocation.Views;

public partial class AddModuleAndSubjectView : UserControl
{
    public AddModuleAndSubjectView(Specialization specialization)
    {
        InitializeComponent();
        DataContext = new AddModuleAndSubjectViewModel(specialization);
    }
    public AddModuleAndSubjectView(Specialization specialization, ObservableCollection<Module> existingModules)
    {
        InitializeComponent();
        DataContext = new AddModuleAndSubjectViewModel(specialization,existingModules);
    }
    public AddModuleAndSubjectView(Specialization specialization, ObservableCollection<ModuleCurriculum> existingModules,Curriculum curriculum)
    {
        InitializeComponent();
        DataContext = new AddModuleAndSubjectViewModel(specialization,existingModules,curriculum);
    }
}