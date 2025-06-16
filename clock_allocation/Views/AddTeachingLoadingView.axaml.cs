using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using clock_allocation.Models;
using clock_allocation.ViewModels;

namespace clock_allocation.Views;

public partial class AddTeachingLoadingView : UserControl
{
    public AddTeachingLoadingView(int _idTeacher)
    {
        InitializeComponent();
        DataContext = new AddTeachingLoadingViewModel(_idTeacher);
    }
    private void OnGroupSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is AutoCompleteBox { DataContext: Group current, SelectedItem: Group SelectedGroup })
        {
            current.IdGroup = SelectedGroup.IdGroup;
            current.StreamNumber = SelectedGroup.StreamNumber;
            current.YearOfAdmission = SelectedGroup.YearOfAdmission;
            current.IdSpecialization = SelectedGroup.IdSpecialization;
            current.FormOfEducation = SelectedGroup.FormOfEducation;
            current.IdSpecializationNavigation = SelectedGroup.IdSpecializationNavigation;
        }
    }

}