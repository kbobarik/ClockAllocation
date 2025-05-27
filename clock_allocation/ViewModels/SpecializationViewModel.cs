using System;
using System.Collections.Generic;
using System.Linq;
using clock_allocation.Models;
using clock_allocation.Views;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;

namespace clock_allocation.ViewModels;

public class SpecializationViewModel : ViewModelBase
{
    List<Specialization> specializations0;
    List<Specialization> specializations;
    Specialization selectedSpecialization;
    private string query;
    private bool isDelete;


    public Specialization SelectedSpecialization
    {
        get => selectedSpecialization;
        set => this.RaiseAndSetIfChanged(ref selectedSpecialization, value);
    }

    public bool IsDelete
    {
        get => isDelete;
        set => this.RaiseAndSetIfChanged(ref isDelete, value);
    }

    public string Query
    {
        get => query;
        set
        {
            this.RaiseAndSetIfChanged(ref query, value);
            Search();
        }
    }

    public List<Specialization> Specializations0
    {
        get => specializations0;
        set => this.RaiseAndSetIfChanged(ref specializations0, value);
    }

    public List<Specialization> Specializations
    {
        get => specializations;
        set => this.RaiseAndSetIfChanged(ref specializations, value);
    }


    public SpecializationViewModel()
    {
        Specializations0 = db.Specializations.Include(x => x.IdLanguageNavigation).ToList();
        Specializations = Specializations0.Select(x => x).ToList();
        IsDelete = false;
    }

    private void Search()
    {
        Specializations = Specializations0.Where(x =>
            x.Code.Contains(Query, System.StringComparison.CurrentCultureIgnoreCase) ||
            x.Direction.Contains(Query, System.StringComparison.CurrentCultureIgnoreCase) ||
            x.Qualification.Contains(Query, System.StringComparison.CurrentCultureIgnoreCase) ||
            x.Title.Contains(Query, System.StringComparison.CurrentCultureIgnoreCase) ||
            x.EducationLevel.Contains(Query, System.StringComparison.CurrentCultureIgnoreCase) ||
            x.DurationOfTraining.Contains(Query, System.StringComparison.CurrentCultureIgnoreCase) ||
            x.FormOfEducation.Contains(Query, System.StringComparison.CurrentCultureIgnoreCase) ||
            x.IdLanguageNavigation.Title.Contains(Query, System.StringComparison.CurrentCultureIgnoreCase)).ToList();
    }

    public void AddSpecialization()
    {
        MainViewModel.Instance.UserControl = new AddSpecializationView();
    }

    public void UpdateSpecialization(Specialization specialization)
    {
        MainViewModel.Instance.UserControl = new AddSpecializationView(specialization);
    }

    public void ConfirmationOfDeletion(Specialization specialization)
    {
        SelectedSpecialization = specialization;
        IsDelete = true;
    }

    public void CancelDeleteCommand()
    {
        IsDelete = false;
    }

    public void DeleteSpecialization()
    {
        try
        {
            db.Specializations.Remove(SelectedSpecialization);
            db.SaveChanges();
            IsDelete = false;
            Specializations0 = db.Specializations.Include(x => x.IdLanguageNavigation).ToList();
            Specializations = Specializations0.Select(x => x).ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}