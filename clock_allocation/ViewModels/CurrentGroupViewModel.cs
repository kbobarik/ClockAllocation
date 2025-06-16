using System.Collections.ObjectModel;
using System.Linq;
using clock_allocation.Models;
using clock_allocation.Views;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;

namespace clock_allocation.ViewModels;

public class CurrentGroupViewModel : ViewModelBase
{
    private ObservableCollection<Group> _groups;
    private int _idSpecailization;
    private int yaer;

    public ObservableCollection<Group> Groups
    {
        get => _groups;
        set => this.RaiseAndSetIfChanged(ref _groups, value);
    }

    public CurrentGroupViewModel(int idSpecialisation,int Year)
    {
        _idSpecailization = idSpecialisation;
        yaer = Year;
        var result = db.Groups
            .Where(g => g.IdSpecialization == idSpecialisation && g.YearOfAdmission == Year)
            .Include(x=>x.IdSpecializationNavigation)
            .ToList();
        Groups = new ObservableCollection<Group>(result);
    }

    public void AddNewGroup()
    {
        var specialization = db.Specializations.FirstOrDefault(x=>x.IdSpecialization == _idSpecailization);
        MainViewModel.Instance.UserControl = new AddGroupView(specialization, yaer);
    }
}
