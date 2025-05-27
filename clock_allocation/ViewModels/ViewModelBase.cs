using clock_allocation.Models;
using ReactiveUI;

namespace clock_allocation.ViewModels;

public class ViewModelBase : ReactiveObject
{
    public ClockAllocationContext db = new();
}