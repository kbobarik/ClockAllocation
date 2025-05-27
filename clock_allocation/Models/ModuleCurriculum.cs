using System.Collections.ObjectModel;
using clock_allocation.ViewModels;

namespace clock_allocation.Models;

public class ModuleCurriculum : CurriculumItem
{
    public ObservableCollection<Module> Children { get; } = new();
    public Module OriginalModule { get; set; }
    public Subject OriginalSubject { get; set; }

}