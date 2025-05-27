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
    }

    private void UpdateCurrentGroup()
    {
        UserControl = new CurrentGroupView(CurrentSpecialization.IdSpecialization);
    }
}