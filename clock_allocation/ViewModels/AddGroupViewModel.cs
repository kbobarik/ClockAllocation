using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using clock_allocation.Models;
using clock_allocation.Views;
using ReactiveUI;

namespace clock_allocation.ViewModels;

public class AddGroupViewModel:ViewModelBase
{
    private ObservableCollection<string> _formOfEducations;
    private Specialization _idSpecialization;
    private ObservableCollection<Group> _groups;
    private string _year;
    private string _title;
    private string _errorMessage;
    private bool _readOnly;

    public bool ReadOnly
    {
        get => _readOnly;
        set => this.RaiseAndSetIfChanged(ref _readOnly, value);
    }
    public ObservableCollection<Group> Groups
    {
        get => _groups;
        set => this.RaiseAndSetIfChanged(ref _groups, value);
    }
    public ObservableCollection<string> FormOfEducations
    {
        get => _formOfEducations;
        set => this.RaiseAndSetIfChanged(ref _formOfEducations,value);
    }

    public string Year
    {
        get => _year;
        set => this.RaiseAndSetIfChanged(ref _year,value);
    }

    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title,value);
    }


    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage,value);
    }

    public AddGroupViewModel(Specialization idSpecialization)
    {
        Groups = new ObservableCollection<Group>();
        _idSpecialization = idSpecialization;
        FormOfEducations = new ObservableCollection<string>(["Бюджет","Внебюджет"]);
        Title = "Добавление групп";
        ReadOnly = false;

    }
    public AddGroupViewModel(Specialization idSpecialization, int year)
    {
        Groups = new ObservableCollection<Group>();
        _idSpecialization = idSpecialization;
        FormOfEducations = new ObservableCollection<string>(["Бюджет","Внебюджет"]);
        Title = "Добавление групп";
        Year = year.ToString();
        ReadOnly = true;

    }
    public void AddGroup()
    {
        Groups.Add(new Group(){IdSpecialization = _idSpecialization.IdSpecialization});
    }
    public void Save()
    {
        try
        {
            int year = Convert.ToInt32(Year);
    
            // Проверяем все группы перед добавлением
            foreach (var group in Groups)
            {
                group.YearOfAdmission = year;
        
                // Проверяем, существует ли уже группа с такими параметрами
                bool groupExists = db.Groups.Any(g => 
                    g.StreamNumber == group.StreamNumber &&
                    g.YearOfAdmission == year &&
                    g.FormOfEducation == group.FormOfEducation);
            
                if (groupExists)
                {
                    ErrorMessage = $"Группа {group}) за {year} год уже существует";
                    return;
                }
            }
    
            // Если все проверки пройдены - добавляем группы
            foreach (var group in Groups)
            {
                db.Groups.Add(group);
            }
    
            db.SaveChanges();
            MainViewModel.Instance.UserControl = new GroupView();
        }
        catch (FormatException)
        {
            ErrorMessage = "Год поступления должен быть числом";
        }
        catch (Exception e)
        {
            ErrorMessage = $"Ошибка: {e.Message}";
        }
    }

    public void DeleteGroup(Group group)
    {
        Groups.Remove(group);
    }
}