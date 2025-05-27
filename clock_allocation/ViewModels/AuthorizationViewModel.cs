using System.Linq;
using clock_allocation.Views;
using ReactiveUI;

namespace clock_allocation.ViewModels;

public class AuthorizationViewModel : ViewModelBase
{
    string login;
    string password;
    string errorMessage;

    public string Login
    {
        get => login;
        set => this.RaiseAndSetIfChanged(ref login, value);
    }

    public string Password
    {
        get => password;
        set => this.RaiseAndSetIfChanged(ref password, value);
    }

    public string ErrorMessage
    {
        get => errorMessage;
        set => this.RaiseAndSetIfChanged(ref errorMessage, value);
    }


    public void Entrance()
    {
        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
        {
            ErrorMessage = "Введите логин и пароль";
            return;
        }

        var user = db.Users.FirstOrDefault(x => x.Login == login);
        if (user == null)
        {
            ErrorMessage = "Неверный логин";
            return;
        }

        if (user.Password != password)
        {
            ErrorMessage = "Неверный пароль";
            return;
        }

        MainWindowViewModel.Instance.CurrentUser = user;
        MainWindowViewModel.Instance.UC = new MainView();
    }
}