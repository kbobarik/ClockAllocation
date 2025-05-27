using Avalonia.Controls;
using clock_allocation.Models;
using clock_allocation.Views;
using ReactiveUI;

namespace clock_allocation.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private UserControl uc;
    public static MainWindowViewModel Instance;
    private User _currentUser;

    public User CurrentUser
    {
        get => _currentUser;
        set => this.RaiseAndSetIfChanged(ref _currentUser, value);
    }
    public UserControl UC
    {
        get => uc;
        set => this.RaiseAndSetIfChanged(ref uc, value);
    }

    public MainWindowViewModel()
    {
        UC = new MainView();
        Instance = this;
    }
}