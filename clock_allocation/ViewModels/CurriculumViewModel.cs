using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using clock_allocation.Models;
using clock_allocation.Views;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;

namespace clock_allocation.ViewModels;

public class CurriculumViewModel : ViewModelBase
{
    private UserControl _UserControl;
    private ObservableCollection<int> years;
    private Curriculum _Curriculum;
    private List<Specialization> _specializations;
    private List<Semester> _Semesters;
    private int? _currentYear;
    private Specialization _currentSpecialization;


    public int? CurrentYear
    {
        get => _currentYear;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentYear, value);
            UpdateCurriculum();
        }
    }

    public Specialization CurrentSpecialization
    {
        get => _currentSpecialization;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentSpecialization, value);
            UpdateCurriculum();
        }
    }

    public List<Specialization> Specializations
    {
        get => _specializations;
        set => this.RaiseAndSetIfChanged(ref _specializations, value);
    }

    public ObservableCollection<int> Years
    {
        get => years;
        set => this.RaiseAndSetIfChanged(ref years, value);
    }

    private List<Semester> Semesters
    {
        get => _Semesters;
        set => this.RaiseAndSetIfChanged(ref _Semesters, value);
    }

    private Curriculum Curriculum
    {
        get => _Curriculum;
        set => this.RaiseAndSetIfChanged(ref _Curriculum, value);
    }

    public UserControl UserControl
    {
        get => _UserControl;
        set => this.RaiseAndSetIfChanged(ref _UserControl, value);
    }

    public AutoCompleteFilterPredicate<object?> SpecializationFilter => (search, item) =>
    {
        if (item is Specialization spec && !string.IsNullOrEmpty(search))
        {
            return spec.Qualification.Contains(search, StringComparison.CurrentCultureIgnoreCase) || spec.FormOfEducation.Contains(search, StringComparison.CurrentCultureIgnoreCase);
        }

        return false;
    };

    public CurriculumViewModel()
    {
        Specializations = db.Specializations.OrderBy(x=>x.IdSpecialization).ToList();
        CurrentSpecialization = Specializations.FirstOrDefault();
        Years = [];
        GetYears();
        CurrentYear = Years.FirstOrDefault();
        if (Years == null) return;
        Curriculum = db.Curricula.FirstOrDefault(x => x.Year == Years.FirstOrDefault() && x.IdSpecialization == CurrentSpecialization.IdSpecialization);
        UserControl = new CertainCurriculumView(Curriculum);

    }
    

    private void GetYears()
    {
        try
        {
            var years = db.Curricula.Where(x=>x.IdSpecialization == CurrentSpecialization.IdSpecialization).Select(x => x.Year).Distinct().OrderBy(x => x).ToList();
            Years?.Clear();
            if (years.Count == 0)return;
            foreach (var year in years)
            {
                Years.Add(year);
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
        }
    }

    public void SelectByYear(int year)
    {
        CurrentYear = year;
    }


    private void UpdateCurriculum()
    {
      
        if (CurrentSpecialization == null) return;
        GetYears();
        Curriculum = db.Curricula.FirstOrDefault(x =>
            x.Year == CurrentYear && x.IdSpecialization == CurrentSpecialization.IdSpecialization);
        UserControl = new CertainCurriculumView(Curriculum);
    }

    public void ToAddCurriculum()
    {
        MainViewModel.Instance.UserControl = new AddCurriculumView(CurrentSpecialization);
    }


    
}