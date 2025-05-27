using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Threading;
using clock_allocation.Models;
using clock_allocation.Views;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;

namespace clock_allocation.ViewModels;

public class AddModuleAndSubjectViewModel : ViewModelBase
{
    private ObservableCollection<Module> _modules;
    private ObservableCollection<ModuleCurriculum> _rootModules;
    private Curriculum _curriculum;
    private bool _isUpdate;
    private string _message;
    private bool _isLoading;


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
    public ObservableCollection<ModuleCurriculum> RootModules
    {
        get => _rootModules;
        set => this.RaiseAndSetIfChanged(ref _rootModules, value);
    }


    public ObservableCollection<Module> Modules
    {
        get => _modules;
        set => this.RaiseAndSetIfChanged(ref _modules, value);
    }

    private Specialization _specialization;
    private int _year;

    public int Year
    {
        get => _year;
        set => this.RaiseAndSetIfChanged(ref _year, value);
    }

    public AddModuleAndSubjectViewModel(Specialization specialization)
    {
        RootModules = new ObservableCollection<ModuleCurriculum>();
        Modules = new ObservableCollection<Module>();
        _isUpdate = false;
        _specialization = specialization;
    }

    public AddModuleAndSubjectViewModel(Specialization specialization, ObservableCollection<Module> modules)
    {
        RootModules = ModulesToModuleCurriculum(modules);
        _isUpdate = false;
        _specialization = specialization;
    }

    public AddModuleAndSubjectViewModel(Specialization specialization, ObservableCollection<ModuleCurriculum> modules,
        Curriculum current)
    {
        RootModules = modules;
        _curriculum = current;
        _isUpdate = true;
        Year = current.Year;
        _specialization = specialization;
    }

    private ObservableCollection<ModuleCurriculum> ModulesToModuleCurriculum(ObservableCollection<Module> modules)
    {
        var rootModules = new ObservableCollection<ModuleCurriculum>();

        foreach (var module in modules)
        {
            var moduleCurriculum = new ModuleCurriculum()
            {
                Code = module.Code,
                Title = module.Title,
                Id = module.IdModule,
                OriginalModule = module // Сохраняем ссылку на оригинальный модуль
            };

            // Определяем тип модуля
            if (module.InverseIdHeadModuleNavigation.Count > 0)
            {
                moduleCurriculum.HasSubModules = true;
                moduleCurriculum.HasSubjects = false;

                // Рекурсивно конвертируем подмодули
                foreach (var subModule in module.InverseIdHeadModuleNavigation)
                {
                    var subModuleCurriculum = ConvertModuleToCurriculum(subModule);
                    moduleCurriculum.Items.Add(subModuleCurriculum);
                }
            }
            else if (module.Subjects.Count > 0)
            {
                moduleCurriculum.HasSubModules = false;
                moduleCurriculum.HasSubjects = true;

                // Конвертируем предметы
                foreach (var subject in module.Subjects)
                {
                    moduleCurriculum.Items.Add(new ModuleCurriculum()
                    {
                        Id = subject.IdSubject,
                        Code = subject.Code,
                        Title = subject.Title,
                        HasSubModules = false,
                        HasSubjects = false,
                        OriginalSubject = subject // Сохраняем ссылку на оригинальный предмет
                    });
                }
            }
            else
            {
                // Пустой модуль - по умолчанию считаем его модулем для подмодулей
                moduleCurriculum.HasSubModules = true;
                moduleCurriculum.HasSubjects = false;
            }

            rootModules.Add(moduleCurriculum);
        }

        return rootModules;
    }
    private ModuleCurriculum ConvertModuleToCurriculum(Module module)
    {
        var moduleCurriculum = new ModuleCurriculum()
        {
            Id = module.IdModule,
            Code = module.Code,
            Title = module.Title,
            OriginalModule = module
        };

        if (module.InverseIdHeadModuleNavigation.Count > 0)
        {
            moduleCurriculum.HasSubModules = true;
            moduleCurriculum.HasSubjects = false;

            foreach (var subModule in module.InverseIdHeadModuleNavigation)
            {
                var subModuleCurriculum = ConvertModuleToCurriculum(subModule);
                moduleCurriculum.Items.Add(subModuleCurriculum);
            }
        }
        else if (module.Subjects.Count > 0)
        {
            moduleCurriculum.HasSubModules = false;
            moduleCurriculum.HasSubjects = true;

            foreach (var subject in module.Subjects)
            {
                moduleCurriculum.Items.Add(new ModuleCurriculum()
                {
                    Id = subject.IdSubject,
                    Code = subject.Code,
                    Title = subject.Title,
                    HasSubModules = false,
                    HasSubjects = false,
                    OriginalSubject = subject
                });
            }
        }
        else
        {
            moduleCurriculum.HasSubModules = true;
            moduleCurriculum.HasSubjects = false;
        }

        return moduleCurriculum;
    }

    public void AddModule()
    {
        Dispatcher.UIThread.Post(() =>
        {
            var module = new ModuleCurriculum()
            {
                HasSubModules = true,
                HasSubjects = true,
            };
            RootModules.Add(module);
        });
    }

    public void AddSubject(ModuleCurriculum module)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (module.Items.Count != 0)
            {
                if (!module.HasSubjects || module.HasSubModules) return;
            }

            module.HasSubjects = true;
            module.HasSubModules = false;
            var newSubject = new ModuleCurriculum()
            {
                HasSubjects = false,
                HasSubModules = false
            };
            module.Items.Add(newSubject);
        });
    }

    public void AddSubModule(ModuleCurriculum module)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (module.Items.Count != 0)
            {
                if (!module.HasSubModules || module.HasSubjects) return;
            }

            module.HasSubModules = true;
            module.HasSubjects = false;
            var submodule = new ModuleCurriculum()
            {
                HasSubModules = true,
                HasSubjects = true
            };
            module.Items.Add(submodule);
        });
    }

    public void DeleteItem(ModuleCurriculum itemToDelete)
    {
        if (itemToDelete == null) return;

        // Ищем родительский элемент
        var parent = FindParent(RootModules, itemToDelete);

        if (parent != null)
        {
            // Удаляем из коллекции родителя
            parent.Items.Remove(itemToDelete);
        }
        else
        {
            // Если не нашли родителя, значит это корневой модуль
            RootModules.Remove(itemToDelete);
        }
    }

    private CurriculumItem FindParent(IEnumerable<CurriculumItem> items, CurriculumItem child)
    {
        foreach (var item in items)
        {
            if (item.Items.Contains(child))
                return item;

            var foundParent = FindParent(item.Items, child);
            if (foundParent != null)
                return foundParent;
        }

        return null;
    }
    private bool ValidateCurriculum()
    {
        if (Year is >= 2000 and <= 2100)
        {
            if (RootModules.Count == 0)
            {
                Message = "Создайте хотя бы один модуль";
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            Message = "Укажите корректный год (2000-2100)";
            return false;
        }
       
    }

    public async Task SaveAsync()
{
    try
    {
        if (!ValidateCurriculum())
        {
            IsLoading = true;
            await Task.Delay(3000);
            IsLoading = false;
            return;
        }
        IsLoading = true;
        Message = "Идет сохранение";
        await using var dbContext = new ClockAllocationContext();

        if (!_isUpdate)
        {
            // Создаем новый учебный план
            _curriculum = new Curriculum
            {
                IdSpecialization = _specialization.IdSpecialization,
                Year = _year,
                Modules = new ObservableCollection<Module>()
            };
            dbContext.Curricula.Add(_curriculum);
        }
        else
        {
            // Загружаем существующий учебный план с полной иерархией
            _curriculum = await dbContext.Curricula
                .Include(c => c.Modules)
                    .ThenInclude(m => m.InverseIdHeadModuleNavigation)
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Subjects)
                        .ThenInclude(s => s.Semesters)
                .FirstOrDefaultAsync(c => c.IdCurriculum == _curriculum.IdCurriculum);

            if (_curriculum == null)
            {
                Debug.WriteLine("Учебный план не найден");
                return;
            }

            // Удаляем предметы и семестры вручную перед удалением модулей
            foreach (var module in _curriculum.Modules.ToList())
            {
                // Удаляем семестры предметов
                foreach (var subject in module.Subjects.ToList())
                {
                    dbContext.Semesters.RemoveRange(subject.Semesters);
                    dbContext.Subjects.Remove(subject);
                }
                
                // Удаляем подмодули
                foreach (var subModule in module.InverseIdHeadModuleNavigation.ToList())
                {
                    // Удаляем предметы подмодулей
                    foreach (var subject in subModule.Subjects.ToList())
                    {
                        dbContext.Semesters.RemoveRange(subject.Semesters);
                        dbContext.Subjects.Remove(subject);
                    }
                    dbContext.Modules.Remove(subModule);
                }
                
                dbContext.Modules.Remove(module);
            }
            
            await dbContext.SaveChangesAsync();
            _curriculum.Modules.Clear();
        }

        // Строим новую иерархию модулей и предметов
        foreach (var moduleCurriculum in RootModules)
        {
            var module = new Module
            {
                Code = moduleCurriculum.Code,
                Title = moduleCurriculum.Title,
                Subjects = new ObservableCollection<Subject>(),
                InverseIdHeadModuleNavigation = new ObservableCollection<Module>(),
                IdCurriculum = _curriculum.IdCurriculum
            };

            ProcessModuleItems(module, moduleCurriculum, _specialization.DurationOfTraining);
            _curriculum.Modules.Add(module);
        }

        await dbContext.SaveChangesAsync();
        IsLoading = false;
        MainViewModel.Instance.UserControl = new ModuleAndSubjectView();
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"Ошибка при сохранении: {ex.Message}");
        Message = ex.Message;
        await Task.Delay(3000);
        IsLoading = false;
    }
}

    private void ProcessModuleItems(Module parentModule,
        ModuleCurriculum parentModuleCurriculum,
        string durationOfTraining)
    {
        foreach (var item in parentModuleCurriculum.Items)
        {
            if (parentModuleCurriculum.HasSubModules) // Это подмодуль
            {
                var subModule = new Module
                {
                    Code = item.Code,
                    Title = item.Title,
                    Subjects = new ObservableCollection<Subject>(),
                    IdHeadModuleNavigation = parentModule,
                    InverseIdHeadModuleNavigation = new ObservableCollection<Module>(),
                    IdCurriculumNavigation = parentModule.IdCurriculumNavigation // Наследуем связь с учебным планом
                };

                if (item.Items.Any())
                {
                    ProcessModuleItems(subModule, item, durationOfTraining);
                }

                parentModule.InverseIdHeadModuleNavigation.Add(subModule);
            }
            else if (parentModuleCurriculum.HasSubjects) // Это предмет
            {
                var subject = new Subject
                {
                    Code = item.Code,
                    Title = item.Title,
                    IdModuleNavigation = parentModule,
                    Semesters = new ObservableCollection<Semester>()
                };

                if (item.OriginalSubject != null)
                {
                    subject.LaboratoryAndPracticalExercises = item.OriginalSubject.LaboratoryAndPracticalExercises;
                    subject.Lessons = item.OriginalSubject.Lessons;
                    subject.CourseDesign = item.OriginalSubject.CourseDesign;
                    AddSemestersForSubject(subject, item.OriginalSubject.Semesters, durationOfTraining);
                }
                else
                {
                    AddSemestersForSubject(subject, new ObservableCollection<Semester>(), durationOfTraining);
                }

                parentModule.Subjects.Add(subject);
            }
        }
    }

    private void AddSemestersForSubject(Subject subject,
        ObservableCollection<Semester> semesters,
        string durationOfTraining)
    {
        if (semesters.Any())
        {
            // Копируем существующие семестры
            foreach (var semester in semesters)
            {
                subject.Semesters.Add(new Semester
                {
                    Number = semester.Number,
                    IdFormOfIntermediateCertification = semester.IdFormOfIntermediateCertification,
                    Lessons = semester.Lessons,
                    Practice = semester.Practice,
                    IndependentWork = semester.IndependentWork,
                    Consultations = semester.Consultations,
                    IntermediateCertification = semester.IntermediateCertification
                });
            }
        }
        else
        {
            // Создаем пустые семестры, если их нет
            var countSemester = GetSemesterCount(durationOfTraining);
            for (var i = 1; i <= countSemester; i++)
            {
                subject.Semesters.Add(new Semester { Number = i });
            }
        }
    }

    private int GetSemesterCount(string durationOfTraining)
    {
        return durationOfTraining switch
        {
            "3 года 10 месяцев" => 8,
            "2 года 10 месяцев" => 6,
            "1 год 10 месяцев" => 4,
            _ => 0
        };
    }
}