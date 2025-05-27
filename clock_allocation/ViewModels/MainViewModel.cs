using Avalonia.Animation;
using Avalonia.Controls;
using clock_allocation.Models;
using clock_allocation.Views;
using ReactiveUI;

namespace clock_allocation.ViewModels;

public class MainViewModel : ViewModelBase
{
    UserControl _userControl;
    public static MainViewModel Instance;

    public UserControl UserControl
    {
        get => _userControl;
        set => this.RaiseAndSetIfChanged(ref _userControl, value);
    }

    public MainViewModel()
    {
        UserControl = new CurriculumView();
        Instance = this;
    }

    public void toSpecializationView()
    {
        UserControl = new SpecializationView();
    }

    public void toCurriculumView()
    {
        UserControl = new CurriculumView();
    }
    public void toAddModulesSubjectView()
    {
        UserControl = new ModuleAndSubjectView();
    }
    public void toGroupView()
    {
        UserControl = new GroupView();
    }

    public void toTeacherView()
    {
        UserControl = new TeachersView();
    }
    public void toTeachingLoadingView()
    {
        UserControl = new TeachingLoadingView();
    }
}