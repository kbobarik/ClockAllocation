using System.Collections.ObjectModel;
using System.Linq;
using clock_allocation.Models;
using ReactiveUI;

namespace clock_allocation.ViewModels;

public class CurrentGroupViewModel : ViewModelBase
{
    private ObservableCollection<GroupSummaryDto> _groups;

    public ObservableCollection<GroupSummaryDto> Groups
    {
        get => _groups;
        set => this.RaiseAndSetIfChanged(ref _groups, value);
    }

    public CurrentGroupViewModel(int idSpecialisation)
    {
        var result = db.Groups
            .GroupBy(g => new
            {
                g.IdGroup,
                g.YearOfAdmission,
                g.IdSpecialization,
                g.FormOfEducation
            })
            .Select(g => new GroupSummaryDto
            {
                IdGroup = g.Key.IdGroup,
                CountGroup = g.Max(x => x.StreamNumber),
                YearOfAdmission = g.Key.YearOfAdmission,
                IdSpecialization = g.Key.IdSpecialization,
                FormOfEducation = g.Key.FormOfEducation
            })
            .Where(g => g.IdSpecialization == idSpecialisation)
            .ToList();
        Groups = new ObservableCollection<GroupSummaryDto>(result);
    }
}

public class GroupSummaryDto
{
    public int IdGroup { get; set; }
    public int CountGroup { get; set; }
    public int YearOfAdmission { get; set; }
    public int IdSpecialization { get; set; }
    public string FormOfEducation { get; set; }
}