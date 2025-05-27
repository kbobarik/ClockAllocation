using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using clock_allocation.Models;
using clock_allocation.ViewModels;

namespace clock_allocation.Views;

public partial class AddCurriculumView : UserControl
{
    public AddCurriculumView(Specialization specialization, Curriculum existingCurriculum = null)
    {
        InitializeComponent();
        DataContext = new AddCurriculumViewModel(specialization, existingCurriculum);
    }
    
}