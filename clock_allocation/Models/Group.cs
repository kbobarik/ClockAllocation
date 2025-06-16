using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace clock_allocation.Models;

public partial class Group : INotifyPropertyChanged
{
    private int _streamNumber;
    private int _yearOfAdmission;
    private int _idSpecialization;
    private string _formOfEducation;
    private Subject _subject;

    public int IdGroup { get; set; }

    public int StreamNumber
    {
        get => _streamNumber;
        set
        {
            if (_streamNumber != value)
            {
                _streamNumber = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayName));
            }
        }
    }

    public int YearOfAdmission
    {
        get => _yearOfAdmission;
        set
        {
            if (_yearOfAdmission != value)
            {
                _yearOfAdmission = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayName));
                OnPropertyChanged(nameof(CurrentCourse));
            }
        }
    }

    public int IdSpecialization
    {
        get => _idSpecialization;
        set
        {
            if (_idSpecialization != value)
            {
                _idSpecialization = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayName));
            }
        }
    }

    public string FormOfEducation
    {
        get => _formOfEducation;
        set
        {
            if (_formOfEducation != value)
            {
                _formOfEducation = value;
                OnPropertyChanged();
            }
        }
    }

    public virtual Specialization IdSpecializationNavigation { get; set; } = null!;

    public virtual ICollection<TeachingLoad> TeachingLoads { get; set; } = new List<TeachingLoad>();

    [NotMapped]
    public Subject Subject
    {
        get => _subject;
        set
        {
            if (_subject != value)
            {
                _subject = value;
                OnPropertyChanged();
            }
        }
    }

    [NotMapped]
    public int CurrentCourse
    {
        get
        {
            if (DateTime.Now.Month >= 1 && DateTime.Now.Month <= 6)
            {
                return DateTime.Now.Year - YearOfAdmission;
            }
            return DateTime.Now.Year - YearOfAdmission + 1;
        }
    }

    [NotMapped]
    public string DisplayName => $"{CurrentCourse}{StreamNumber}{NameSpecialisation()}";

    private bool splitting;

    [NotMapped]
    public bool Splitting
    {
        get => splitting;
        set
        {
            if (splitting != value)
            {
                splitting = value;
                OnPropertyChanged();
            }
        } 
    }

    public override string ToString() => DisplayName;

    private char NameSpecialisation()
    {
        return IdSpecializationNavigation?.Qualification switch
        {
            "Программист" => 'П',
            "Разработчик веб и мультимедийных приложений" => 'В',
            "Бухгалтер, специалист по налогообложению" => 'Б',
            "Операционный логист" => 'Л',
            "Юрист" => 'Ю',
            _ => ' '
        };
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}