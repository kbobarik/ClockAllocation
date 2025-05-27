using System;
using System.Collections.Generic;
using System.Linq;
using clock_allocation.Models;
using clock_allocation.Views;
using ReactiveUI;

namespace clock_allocation.ViewModels;

public class AddSpecializationViewModel : ViewModelBase
{
    private Specialization specialization;
    private List<Language> language;
    private List<string> durationOfTrainings;
    private List<string> formOfEducations;
    private List<string> educationLevels;
    private string errorMessage;
    private bool addSpecialization;
    private string title;

    public string Title
    {
        get => title;
        set => this.RaiseAndSetIfChanged(ref title, value);
    }
    public string ErrorMessage
    {
        get => errorMessage;
        set => this.RaiseAndSetIfChanged(ref errorMessage, value);
    }

    public List<string> EducationLevels
    {
        get => educationLevels;
        set => this.RaiseAndSetIfChanged(ref educationLevels, value);
    }

    public List<string> FormOfEducations
    {
        get => formOfEducations;
        set => this.RaiseAndSetIfChanged(ref formOfEducations, value);
    }

    public List<string> DurationOfTrainings
    {
        get => durationOfTrainings;
        set => this.RaiseAndSetIfChanged(ref durationOfTrainings, value);
    }

    public List<Language> Language
    {
        get => language;
        set => this.RaiseAndSetIfChanged(ref language, value);
    }

    public Specialization Specialization
    {
        get => specialization;
        private init => this.RaiseAndSetIfChanged(ref specialization, value);
    }

    public AddSpecializationViewModel()
    {
        Specialization = new Specialization();
        Title = "Добавить специальность";
        Language = db.Languages.ToList();
        DurationOfTrainings = ["3 года 10 месяцев", "2 года 10 месяцев", "1 год 10 месяцев"];
        FormOfEducations = ["Очная", "Заочная"];
        EducationLevels = ["Основное общее", "Среднее общее"];
        addSpecialization = true;
    }

    public AddSpecializationViewModel(Specialization specialization)
    {
        Title = "Отредактировать специальность";
        Specialization = specialization;
        db.Specializations.Attach(specialization);
        Language = db.Languages.ToList();
        DurationOfTrainings = ["3 года 10 месяцев", "2 года 10 месяцев", "1 год 10 месяцев"];
        FormOfEducations = ["Очная", "Заочная"];
        EducationLevels = ["Основное общее", "Среднее общее"];
        addSpecialization = false;
    }

    public void Save()
    {
        try
        {
            Specialization.IdLanguage = specialization.IdLanguageNavigation.IdLanguage;
            if (addSpecialization)
            {
                db.Specializations.Add(specialization);
            }
            else
            {
                db.Specializations.Update(specialization);
            }

            db.SaveChanges();
            MainViewModel.Instance.UserControl = new SpecializationView();
        }
        catch (Exception e)
        {
            ErrorMessage = e.Message;
            Console.WriteLine(e);
        }
    }
}