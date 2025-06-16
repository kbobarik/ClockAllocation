using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using clock_allocation.ViewModels;

namespace clock_allocation.Views;

public partial class CurrentTeacherLoadingView : UserControl
{
    public CurrentTeacherLoadingView(int year, int idTeacher)
    {
        InitializeComponent();
        DataContext = new CurrentTeacherLoadingViewModel(year, idTeacher);
    }
}