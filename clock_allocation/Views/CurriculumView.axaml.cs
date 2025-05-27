using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using clock_allocation.ViewModels;

namespace clock_allocation.Views;

public partial class CurriculumView : UserControl
{
    public CurriculumView()
    {
        InitializeComponent();
        var vm = new CurriculumViewModel();
        DataContext = vm;
        SpecializationBox.ItemFilter = vm.SpecializationFilter;
    }
}