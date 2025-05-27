using System;
using System.Collections.ObjectModel;
using System.Linq;
using clock_allocation.Models;
using clock_allocation.Views;
using ReactiveUI;

namespace clock_allocation.ViewModels;

public class TeachersViewModel : ViewModelBase
{
    private string _query;
    private ObservableCollection<Teacher> _teachers;
    private ObservableCollection<Teacher> _teachers0;
    private bool _isDeleted;
    private Teacher _deletedTeacher;
    private Teacher _updateTeacher;
    private bool _isUpdate;
    private bool _updatePassword;
    private bool _isUpdatingPassword;
    private bool _updatelogin;
    private string _password;
    private string _repeatedPassword;
    private string _login;
    private string _errorMessage;


    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public string RepeatedPassword
    {
        get => _repeatedPassword;
        set => this.RaiseAndSetIfChanged(ref _repeatedPassword, value);
    }

    public bool IsUpdate
    {
        get => _isUpdate;
        set => this.RaiseAndSetIfChanged(ref _isUpdate, value);
    }

    public string Login
    {
        get => _login;
        set => this.RaiseAndSetIfChanged(ref _login, value);
    }

    public bool UpdateLogin
    {
        get => _updatelogin;
        set => this.RaiseAndSetIfChanged(ref _updatelogin, value);
    }

    public bool UpdatePassword
    {
        get => _updatePassword;
        set => this.RaiseAndSetIfChanged(ref _updatePassword, value);
    }

    public bool IsDeleted
    {
        get => _isDeleted;
        set => this.RaiseAndSetIfChanged(ref _isDeleted, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    public string Query
    {
        get => _query;
        set
        {
            this.RaiseAndSetIfChanged(ref _query, value);
            SearchTeachers();
        }
    }

    public ObservableCollection<Teacher> Teachers
    {
        get => _teachers;
        set => this.RaiseAndSetIfChanged(ref _teachers, value);
    }

    public ObservableCollection<Teacher> Teachers0
    {
        get => _teachers0;
        set => this.RaiseAndSetIfChanged(ref _teachers0, value);
    }

    public TeachersViewModel()
    {
        IsDeleted = false;
        Teachers0 = new ObservableCollection<Teacher>(db.Teachers.OrderBy(x => x.Surname).ToList());
        Teachers = new ObservableCollection<Teacher>(Teachers0.ToList());
        IsUpdate = false;
    }

    public void AddTeacher()
    {
        MainViewModel.Instance.UserControl = new AddTeacherView();
    }

    public void UpdateTeacher(Teacher teacher)
    {
        MainViewModel.Instance.UserControl = new AddTeacherView(teacher);
    }

    public void ConfirmationOfDeletion(Teacher teacher)
    {
        IsDeleted = true;
        _deletedTeacher = teacher;
    }

    public void DeleteTeacher()
    {
        db.Teachers.Remove(_deletedTeacher);
        db.SaveChanges();
        Teachers0 = new ObservableCollection<Teacher>(db.Teachers.ToList());
        Teachers = new ObservableCollection<Teacher>(Teachers0.ToList());
        IsDeleted = false;
    }

    public void CancelDeleteCommand()
    {
        IsDeleted = false;
    }

    public void CancelUpdate()
    {
        IsUpdate = false;
    }

    public void CancelUpdatePassword()
    {
        UpdatePassword = false;
        
        Password = string.Empty;
        RepeatedPassword = string.Empty;
        Login = string.Empty;
    }

    public void CancelUpdateLogin()
    {
        UpdateLogin = false;
        Password = string.Empty;
        RepeatedPassword = string.Empty;
        Login = string.Empty;
    }

    public void CheckPassword()
    {
        ErrorMessage = "";
        if (string.IsNullOrEmpty(Password))
        {
            ErrorMessage = "Введите пароль";
            return;
        }

        if (MainWindowViewModel.Instance.CurrentUser.Password != Password)
        {
            ErrorMessage = "Неверный пароль";
            return;
        }

        IsUpdate = false;
        Password = string.Empty;
        if (_isUpdatingPassword)
        {
            UpdatePassword = true;
            UpdateLogin = false;
        }
        else
        {
            UpdateLogin = true;
            UpdatePassword = false;
        }
        
        Password = string.Empty;
        RepeatedPassword = string.Empty;
        Login = string.Empty;
    }

    public void UpdateLoginFun()
    {
        ErrorMessage = string.Empty;
        if (string.IsNullOrEmpty(Login))
        {
            ErrorMessage = "Введите логин";
            return;
        }

        _updateTeacher.Login = Login;
        try
        {
            db.Teachers.Update(_updateTeacher);
            db.SaveChanges();
            UpdateLogin = false;
            Password = string.Empty;
            RepeatedPassword = string.Empty;
            Login = string.Empty;
        }
        catch (Exception e)
        {
            ErrorMessage = e.Message;
        }
        
    }

    public void UpdatePasswordFun()
    {
        ErrorMessage = string.Empty;
        if (string.IsNullOrEmpty(Password))
        {
            ErrorMessage = "Введите пароль";
            return;
        }

        if (RepeatedPassword != Password)
        {
                ErrorMessage = "Пароли не совпадают";
            return;
        }

        _updateTeacher.Password = Password;
        try
        {
            db.Teachers.Update(_updateTeacher);
            db.SaveChanges();
            UpdatePassword = false;
            
            Password = string.Empty;
            RepeatedPassword = string.Empty;
            Login = string.Empty;
        }
        catch (Exception e)
        {
            ErrorMessage = e.Message;
        }
    }

    public void Update((Teacher teacher, bool isUpdatePassword) tuple)
    {
        _isUpdatingPassword = tuple.isUpdatePassword;
        _updateTeacher = tuple.teacher;
        IsUpdate = true;
    }

    private void SearchTeachers()
    {
        var teachers = Teachers0.Where(x =>
            x.Surname.Contains(Query, StringComparison.CurrentCultureIgnoreCase) ||
            x.Name.Contains(Query, StringComparison.CurrentCultureIgnoreCase) ||
            x.Patronymic.Contains(Query, StringComparison.CurrentCultureIgnoreCase)).ToList();
        Teachers = new ObservableCollection<Teacher>(teachers);
    }
}