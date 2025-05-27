using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.Controls;
using clock_allocation.Models;
using clock_allocation.Views;
using ReactiveUI;

namespace clock_allocation.ViewModels;

public class CurrentModuleAndSubjectViewModel:ViewModelBase
{
    private ObservableCollection<Module> _modules;
    private ObservableCollection<ModuleCurriculum> _rootModules;
    private Curriculum _curriculum;

    public ObservableCollection<ModuleCurriculum> RootModules
    {
        get => _rootModules;
        set => this.RaiseAndSetIfChanged(ref _rootModules, value);
    }

    public ObservableCollection<Module> Modules
    {
        get => _modules;
        set=>this.RaiseAndSetIfChanged(ref _modules, value);
    }
    private Specialization _specialization;
    public CurrentModuleAndSubjectViewModel(Curriculum curriculum, Specialization specialization)
    {
        _curriculum = curriculum;
        Modules = new ObservableCollection<Module>(_curriculum.Modules.Where(x=> x.IdHeadModule == null).ToList());
        _specialization = specialization;
        RootModules = ModulesToModuleCurriculum(Modules);
    }


    private ObservableCollection<ModuleCurriculum> ModulesToModuleCurriculum( ObservableCollection<Module> modules, bool isRootLevel = true)
    {
        var result = new ObservableCollection<ModuleCurriculum>();
    
        if (modules == null)
            return result;

        foreach (var module in modules)
        {
            var moduleCurriculum = new ModuleCurriculum
            {
                OriginalModule = module,
                Code = module.Code,
                Title = module.Title,
                Id = module.IdModule,
                HasSubModules = module.InverseIdHeadModuleNavigation?.Count > 0,
                HasSubjects = module.Subjects?.Count > 0
            };

            // Рекурсивно конвертируем подмодули в Items
            if (module.InverseIdHeadModuleNavigation != null && 
                module.InverseIdHeadModuleNavigation.Count > 0)
            {
                var childModules = ModulesToModuleCurriculum(
                    module.InverseIdHeadModuleNavigation);
                
                foreach (var child in childModules)
                {
                    moduleCurriculum.Items.Add(child);
                }
            }

            // Добавляем предметы в Items
            if (module.Subjects != null)
            {
                foreach (var subject in module.Subjects)
                {
                    moduleCurriculum.Items.Add(new ModuleCurriculum
                    {
                        OriginalSubject = subject,
                        Code = subject.Code,
                        Title = subject.Title,
                        Id = subject.IdSubject,
                        HasSubjects = false,
                        HasSubModules = false
                    });
                }
            }

            result.Add(moduleCurriculum);
        }

        return result;
    }

    public void CopyModules()
    {
        MainViewModel.Instance.UserControl = new AddModuleAndSubjectView(_specialization,Modules);
    }
    public void UpdateNodules()
    {
        MainViewModel.Instance.UserControl = new AddModuleAndSubjectView(_specialization,RootModules, _curriculum);
    }
}