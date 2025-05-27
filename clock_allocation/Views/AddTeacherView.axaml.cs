using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using clock_allocation.Models;
using clock_allocation.ViewModels;

namespace clock_allocation.Views;

public partial class AddTeacherView : UserControl
{
    public AddTeacherView()
    {
        InitializeComponent();
        DataContext = new AddTeacherViewModel();
    }
    
    public AddTeacherView(Teacher teacher)
    {
        InitializeComponent();
        DataContext = new AddTeacherViewModel(teacher);
    }
}