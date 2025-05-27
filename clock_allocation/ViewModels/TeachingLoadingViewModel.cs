using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using clock_allocation.Models;
using ReactiveUI;

namespace clock_allocation.ViewModels;

public class TeachingLoadingViewModel : ViewModelBase
{
    private ObservableCollection<int> _years;
    private int _selectedYear;
    private ObservableCollection<Teacher> _teachers;
    private Teacher _selectedTeacher;
    private UserControl _teachingLoadingControl;

    public ObservableCollection<int> Years
    {
        get => _years;
        set => this.RaiseAndSetIfChanged(ref _years, value);
    }

    public ObservableCollection<Teacher> Teachers
    {
        get => _teachers;
        set => this.RaiseAndSetIfChanged(ref _teachers, value);
    }

    public Teacher SelectedTeacher
    {
        get => _selectedTeacher;
        set => this.RaiseAndSetIfChanged(ref _selectedTeacher, value);
    }

    public int SelectedYear
    {
        get => _selectedYear;
        set => this.RaiseAndSetIfChanged(ref _selectedYear, value);
    }

    public UserControl TeachingLoadingControl
    {
        get => _teachingLoadingControl;
        set => this.RaiseAndSetIfChanged(ref _teachingLoadingControl, value);
    }

    public TeachingLoadingViewModel()
    {
        Teachers = new ObservableCollection<Teacher>(db.Teachers.OrderBy(x=>x.Surname).ToList());
        SelectedTeacher = Teachers.FirstOrDefault();
        Years = new ObservableCollection<int>(db.TeachingLoads.Where(x=>x.IdTeacher == SelectedTeacher.IdTeacher).Select(x=>x.Year).OrderBy(x=>x).Distinct().ToList());
        SelectedYear = Years.FirstOrDefault();
        TeachingLoadingControl = new UserControl();
        
    }
    public AutoCompleteFilterPredicate<object?> TeacherFilter => (search, item) =>
    {
        if (item is Teacher spec && !string.IsNullOrEmpty(search))
        {
            return spec.Surname.Contains(search, StringComparison.CurrentCultureIgnoreCase) || spec.Name.Contains(search, StringComparison.CurrentCultureIgnoreCase) || spec.Patronymic.Contains(search, StringComparison.CurrentCultureIgnoreCase);
        }

        return false;
    };

    public void ToAddTeachingLoading()
    {
        
    }

    public void SelectByYear()
    {
        
    }
}