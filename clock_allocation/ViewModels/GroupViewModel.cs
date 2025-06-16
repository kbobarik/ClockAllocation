using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using clock_allocation.Models;
using clock_allocation.Views;
using ReactiveUI;

namespace clock_allocation.ViewModels;

public class GroupViewModel : ViewModelBase
{
    private ObservableCollection<Specialization> _specializations;
    private Specialization _currentSpecialization;
    private UserControl _userControl;
    private ObservableCollection<int> _years;
    private int currentYear;

    public UserControl UserControl
    {
        get=>_userControl;
        set=>this.RaiseAndSetIfChanged(ref _userControl, value);
    }
    public Specialization CurrentSpecialization
    {
        get => _currentSpecialization;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentSpecialization, value);
            UpdateCurrentGroup();
        }
    }
    public ObservableCollection<Specialization> Specializations
    {
        get=>_specializations;
        set=>this.RaiseAndSetIfChanged(ref _specializations, value);
    }

    public ObservableCollection<int> Years
    {
        get=>_years;
        set=>this.RaiseAndSetIfChanged(ref _years, value);
    }

    public int CurrentYear
    {
        get => currentYear;
        set=>this.RaiseAndSetIfChanged(ref currentYear, value);
    }
    public AutoCompleteFilterPredicate<object?> SpecializationFilter => (search, item) =>
    {
        if (item is Specialization spec && !string.IsNullOrEmpty(search))
        {
            return spec.Qualification.Contains(search, StringComparison.CurrentCultureIgnoreCase) || spec.FormOfEducation.Contains(search, StringComparison.CurrentCultureIgnoreCase);
        }

        return false;
    };
    public GroupViewModel()
    {
        Specializations = new ObservableCollection<Specialization>(db.Specializations.OrderBy(x=>x.IdSpecialization).ToList());
        CurrentSpecialization = Specializations.FirstOrDefault();
        var countCourses = CurrentSpecialization.DurationOfTraining switch
        {
            "3 года 10 месяцев" => 4,
            "2 года 10 месяцев" => 3,
            "1 год 10 месяцев" => 2,
            _ => 0
        };
        Years = new ObservableCollection<int>(db.Groups.Where(x=>x.IdSpecialization == CurrentSpecialization.IdSpecialization&&x.YearOfAdmission >= DateTime.Now.Year-countCourses).Select(x => x.YearOfAdmission).OrderBy(x=>x).Distinct().ToList());
        CurrentYear = Years.FirstOrDefault();
        UserControl = new CurrentGroupView(CurrentSpecialization.IdSpecialization,CurrentYear );
    }

    public void SelectByYear(int year)
    {
        CurrentYear = year;
        UserControl = new CurrentGroupView(CurrentSpecialization.IdSpecialization,CurrentYear );
    }
    private void UpdateCurrentGroup()
    {
        if (CurrentSpecialization != null)
        {
            
            var countCourses = CurrentSpecialization.DurationOfTraining switch
            {
                "3 года 10 месяцев" => 4,
                "2 года 10 месяцев" => 3,
                "1 год 10 месяцев" => 2,
                _ => 0
            };
            Years = new ObservableCollection<int>(db.Groups.Where(x=>x.IdSpecialization == CurrentSpecialization.IdSpecialization&&x.YearOfAdmission >= DateTime.Now.Year-countCourses).Select(x => x.YearOfAdmission).OrderBy(x=>x).Distinct().ToList());
            CurrentYear = Years.FirstOrDefault();
            UserControl = new CurrentGroupView(CurrentSpecialization.IdSpecialization,CurrentYear );
        }
    }

    public void AddGroup()
    {
        MainViewModel.Instance.UserControl = new AddGroupView(CurrentSpecialization);
    }
}