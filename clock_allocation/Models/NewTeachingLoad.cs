using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace clock_allocation.Models;

public class NewTeachingLoad:INotifyPropertyChanged
{
    private Subject subject;

    public Subject Subject
    {
        get => subject;
        set
        {
            if (subject != value)
            {
                subject = value;
                OnPropertyChanged();
            }
        }
    }

    private ObservableCollection<Group> groups;

    public ObservableCollection<Group> Groups
    {
        get => groups;
        set
        {
            if (groups != value)
            {
                groups = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}