using System;
using System.Linq;
using clock_allocation.Models;
using clock_allocation.Views;
using ReactiveUI;

namespace clock_allocation.ViewModels;

public class AddTeacherViewModel:ViewModelBase
{
    private string _title;
    private Teacher _teacher;
    private string  _errorMessage;
    private string _password;
    private bool _update;

    public bool Update
    {
        get=> _update;
        set => this.RaiseAndSetIfChanged(ref _update, value);
    }
    public string Password
    {
        get=>_password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }
    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }
    public Teacher Teacher
    {
        get => _teacher;
        set => this.RaiseAndSetIfChanged(ref _teacher, value);
    }
    public string Title
    {
        get => _title;
        set =>this.RaiseAndSetIfChanged(ref _title, value);
    }
    

    public AddTeacherViewModel()
    {
        Title = "Добавление преподавателя";
        Teacher = new Teacher();
        Update = true;
    }

    public AddTeacherViewModel(Teacher teacher)
    {
        Teacher = teacher;
        Title = "Редактирование преподавателя";
        Update = true;
    }

    public void Save()
    {
        if (string.IsNullOrEmpty(Teacher.Name) || string.IsNullOrEmpty(Teacher.Surname) ||
            string.IsNullOrEmpty(Teacher.Login) || string.IsNullOrEmpty(Teacher.Password))
        {
            ErrorMessage = "Заполните все поля";
            return;
        }
        if (Password != Teacher.Password && !Update)
        {
            ErrorMessage = "Пароли не совпадают";
            return;
        }

        var teacher = db.Teachers.FirstOrDefault(x=>x.Login == Teacher.Login);
        if (teacher != null && !Update)
        {
            ErrorMessage = "Преподаватель с таким логином уже существует";
            return;
        }
        try
        {
            var context = new ClockAllocationContext();
            if (Update)
            {
                context.Teachers.Attach(Teacher);
                context.Teachers.Update(Teacher);
            }
            else
            {
                context.Teachers.Add(Teacher);
            }

            context.SaveChanges();
            MainViewModel.Instance.UserControl = new TeachersView();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}