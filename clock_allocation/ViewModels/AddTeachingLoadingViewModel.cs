using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using clock_allocation.Models;
using clock_allocation.Views;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;

namespace clock_allocation.ViewModels;

public class AddTeachingLoadingViewModel : ViewModelBase
{
    private string _year;
    private bool _isLoading;
    private string _message;
    private ObservableCollection<Subject> _subjects;
    private ObservableCollection<Group> _groups;
    private Subject _selectedSubject;
    private Group _selectedGroup;
    private int _idTeacher;

    private ObservableCollection<NewTeachingLoad> _teachingLoads;

    public Group SelectedGroup
    {
        get => _selectedGroup;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedGroup, value);
            Debug.WriteLine(_selectedGroup);
        }
    }

    public ObservableCollection<Group> Groups
    {
        get => _groups;
        set => this.RaiseAndSetIfChanged(ref _groups, value);
    }

    public Subject SelectedSubject
    {
        get => _selectedSubject;
        set => this.RaiseAndSetIfChanged(ref _selectedSubject, value);
    }

    public string Year
    {
        get => _year;
        set => this.RaiseAndSetIfChanged(ref _year, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    public string Message
    {
        get => _message;
        set => this.RaiseAndSetIfChanged(ref _message, value);
    }


    public ObservableCollection<NewTeachingLoad> TeachingLoads
    {
        get => _teachingLoads;
        set => this.RaiseAndSetIfChanged(ref _teachingLoads, value);
    }

    public ObservableCollection<Subject> Subjects
    {
        get => _subjects;
        set => this.RaiseAndSetIfChanged(ref _subjects, value);
    }

    public AutoCompleteFilterPredicate<object?> SublectFilter => (search, item) =>
    {
        if (item is Subject spec && !string.IsNullOrEmpty(search))
        {
            return spec.Title.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                   spec.Code.Contains(search, StringComparison.CurrentCultureIgnoreCase);
        }

        return false;
    };

    public AutoCompleteFilterPredicate<object?> GroupFilter => (search, item) =>
    {
        if (item is Group spec && !string.IsNullOrEmpty(search))
        {
            return spec.ToString().Contains(search, StringComparison.CurrentCultureIgnoreCase);
        }

        return false;
    };

    public AddTeachingLoadingViewModel(int IdTeacher)
    {
        _idTeacher = IdTeacher;
        IsLoading = false;
        TeachingLoads = new ObservableCollection<NewTeachingLoad>();
        Subjects = new ObservableCollection<Subject>(db.Subjects.ToList());
        SelectedSubject = Subjects.FirstOrDefault();
        Groups = new ObservableCollection<Group>(db.Groups.Include(x => x.IdSpecializationNavigation).ToList());
        SelectedGroup = Groups.FirstOrDefault();
    }

    public void DeleteSubject(NewTeachingLoad teachingLoad)
    {
        TeachingLoads.Remove(teachingLoad);
    }

    public void AddSubject()
    {
        TeachingLoads.Add(new NewTeachingLoad()
            { Subject = SelectedSubject, Groups = new ObservableCollection<Group>() });
    }

    public void DeleteGroup(Group group)
    {
        foreach (var load in TeachingLoads)
        {
            if (load.Groups.Contains(group))
            {
                load.Groups.Remove(group);
                this.RaisePropertyChanged(nameof(TeachingLoads));
                break;
            }
        }
    }

    public void AddGroup(NewTeachingLoad subject)
    {
        if (SelectedGroup == null) return;
    
        // Создаем НОВЫЙ экземпляр группы на основе выбранной
        var newGroup = new Group 
        {
            IdGroup = SelectedGroup.IdGroup,
            StreamNumber = SelectedGroup.StreamNumber,
            YearOfAdmission = SelectedGroup.YearOfAdmission,
            IdSpecialization = SelectedGroup.IdSpecialization,
            FormOfEducation = SelectedGroup.FormOfEducation,
            IdSpecializationNavigation = SelectedGroup.IdSpecializationNavigation
        };
    
        subject.Groups.Add(newGroup);
    }
    
    

    public async Task SaveChanges()
    {
        try
        {
            var validateYear = int.TryParse(Year, out int year) && year is >= 2000 and <= 2100;
            if (!validateYear)
            {
                Message = "Укажите корректный год";
                IsLoading = true;
                await Task.Delay(3000);
                IsLoading = false;
                Message = string.Empty;
                return;
            }

            IsLoading = true;
            Message = "Идет сохранение...";
            await using var dbContext = new ClockAllocationContext();
            foreach (var subject in TeachingLoads)
            {
                foreach (var group in subject.Groups)
                {
                    // Получаем группу из базы, чтобы избежать конфликта отслеживания
                    var dbGroup = dbContext.Groups.Find(group.IdGroup);
                    if (dbGroup == null) continue;

                    // Определяем семестр по первому символу названия группы
                    var semester = group.ToString()[0] switch
                    {
                        '1' => 1,
                        '2' => 3,
                        '3' => 5,
                        '4' => 7,
                        _ => 1
                    };

                    // Находим или создаем семестры
                    var firstSemester = await dbContext.Semesters
                        .FirstOrDefaultAsync(x => x.Number == semester && x.IdSubject == subject.Subject.IdSubject);

                    var secondSemester = await dbContext.Semesters
                        .FirstOrDefaultAsync(x => x.Number == semester + 1 && x.IdSubject == subject.Subject.IdSubject);

                    if (firstSemester == null || secondSemester == null)
                    {
                        Debug.WriteLine("Не найдены соответствующие семестры");
                        continue;
                    }

                    // Создаем учебную нагрузку для первого семестра
                    var firstTeachingLoad = new TeachingLoad()
                    {
                        IdGroup = dbGroup.IdGroup,
                        IdSemester = firstSemester.IdSemester,
                        IdTeacher = _idTeacher,
                        Year = Convert.ToInt32(Year),
                        Consultations = firstSemester.Consultations,
                        IdSubject = subject.Subject.IdSubject,
                        MaximumWorkload = subject.Subject.TotalHours,
                    };
                    dbContext.TeachingLoads.Add(firstTeachingLoad);

                    // Создаем учебную нагрузку для второго семестра
                    var secondTeachingLoad = new TeachingLoad()
                    {
                        IdGroup = dbGroup.IdGroup,
                        IdSemester = secondSemester.IdSemester,
                        IdTeacher = _idTeacher,
                        Year = Convert.ToInt32(Year),
                        Consultations = secondSemester.Consultations,
                        IdSubject = subject.Subject.IdSubject
                    };
                   
                    dbContext.TeachingLoads.Add(secondTeachingLoad);
                    if (!group.Splitting) continue;
                    // Создаем учебную нагрузку для первого семестра
                    firstTeachingLoad = new TeachingLoad()
                    {
                        IdGroup = dbGroup.IdGroup,
                        IdSemester = firstSemester.IdSemester,
                        IdTeacher = _idTeacher,
                        Year = Convert.ToInt32(Year),
                        IdSubject = subject.Subject.IdSubject,
                        Split = true
                    };
                    dbContext.TeachingLoads.Add(firstTeachingLoad);

                    // Создаем учебную нагрузку для второго семестра
                    secondTeachingLoad = new TeachingLoad()
                    {
                        IdGroup = dbGroup.IdGroup,
                        IdSemester = secondSemester.IdSemester,
                        IdTeacher = _idTeacher,
                        Year = Convert.ToInt32(Year),
                        IdSubject = subject.Subject.IdSubject,
                        Split = true
                    };
                    dbContext.TeachingLoads.Add(secondTeachingLoad);
                }
            }

            await dbContext.SaveChangesAsync();
            MainViewModel.Instance.UserControl = new TeachingLoadingView();
        }
        catch (Exception e)
        {
            Message = e.Message;
            IsLoading = true;
            await Task.Delay(3000);
        }
        finally
        {
            IsLoading = false;
            Message  = string.Empty;
        }
    }
}