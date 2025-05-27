using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using clock_allocation.ViewModels;
using ReactiveUI;

namespace clock_allocation.Models;

public class CurriculumItem : INotifyPropertyChanged
{
    private bool _hasSubModules;
    private bool _hasSubjects;
    
    public int Id { get; set; }
    public string Code { get; set; }
    public string Title { get; set; }
    
    public bool HasSubModules
    {
        get => _hasSubModules;
        set
        {
            if (_hasSubModules != value)
            {
                _hasSubModules = value;
                OnPropertyChanged();
            }
        }
    }
    
    public bool HasSubjects
    {
        get => _hasSubjects;
        set
        {
            if (_hasSubjects != value)
            {
                _hasSubjects = value;
                OnPropertyChanged();
            }
        }
    }
    
    public ObservableCollection<ModuleCurriculum> Items { get; } = new();
    
    public event PropertyChangedEventHandler PropertyChanged;
    
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
