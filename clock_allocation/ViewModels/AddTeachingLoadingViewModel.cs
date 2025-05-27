using System.Collections.ObjectModel;
using clock_allocation.Models;
using ReactiveUI;

namespace clock_allocation.ViewModels;

public class AddTeachingLoadingViewModel:ViewModelBase
{
    private string _year;
    private bool _isLoading;
    private string _message;
    private ObservableCollection<TeachingLoad> _teachingLoads;

    public string Year
    {
        get=> _year;
        set=> this.RaiseAndSetIfChanged(ref _year,value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading,value);
    }

    public string Message
    {
        get => _message;
        set => this.RaiseAndSetIfChanged(ref _message,value);
    }

    public ObservableCollection<TeachingLoad> TeachingLoads
    {
        get => _teachingLoads;
        set => this.RaiseAndSetIfChanged(ref _teachingLoads,value);
    }

    public AddTeachingLoadingViewModel()
    {
        IsLoading = false;
        TeachingLoads = new ObservableCollection<TeachingLoad>();
    }
}