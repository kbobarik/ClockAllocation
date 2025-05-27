using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace clock_allocation.Models;

public sealed partial class Semester:INotifyPropertyChanged
{
    public int IdSemester { get; set; }

    public int IdSubject { get; set; }


    private int _number;
    public int Number
    {
        get => _number;
        set
        {
            _number = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Number)));
            
        }
    }
    
    public int IdFormOfIntermediateCertification { get; set; }

    private int _lessons;
    public int Lessons
    {
        get => _lessons;
        set
        {
            if (_lessons == value) return;
            _lessons = value;
            OnPropertyChanged(nameof(Lessons));
            OnPropertyChanged(nameof(TotalHours)); // Обновляем TotalHours
        }
    }

    private int _consultations;
    public int Consultations
    {
        get => _consultations;
        set
        {
            if (_consultations != value)
            {
                _consultations = value;
                OnPropertyChanged(nameof(Consultations));
                OnPropertyChanged(nameof(TotalHours)); // Обновляем TotalHours
            }
        }
    }

    private int _practice;
    public int Practice
    {
        get => _practice;
        set
        {
            if (_practice != value)
            {
                _practice = value;
                OnPropertyChanged(nameof(Practice));
                OnPropertyChanged(nameof(TotalHours)); // Обновляем TotalHours
            }
        }
    }

    private int _independentWork;
    public int IndependentWork
    {
        get => _independentWork;
        set
        {
            if (_independentWork != value)
            {
                _independentWork = value;
                OnPropertyChanged(nameof(IndependentWork));
                OnPropertyChanged(nameof(TotalHours)); // Обновляем TotalHours
            }
        }
    }

    private int _intermediateCertification;
    public int IntermediateCertification
    {
        get => _intermediateCertification;
        set
        {
            if (_intermediateCertification != value)
            {
                _intermediateCertification = value;
                OnPropertyChanged(nameof(IntermediateCertification));
                OnPropertyChanged(nameof(TotalHours)); // Обновляем TotalHours
            }
        }
    }

    private FormOfIntermediateCertification _IdFormOfIntermediateCertificationNavigation;
    public FormOfIntermediateCertification IdFormOfIntermediateCertificationNavigation
    {
        get => _IdFormOfIntermediateCertificationNavigation;
        set
        {
            if (_IdFormOfIntermediateCertificationNavigation != value)
            {
                _IdFormOfIntermediateCertificationNavigation = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IdFormOfIntermediateCertificationNavigation)));
            }
        }
    }
    public int TotalHours =>
        Lessons +
        Practice +
        IndependentWork +
        IntermediateCertification;

    public Subject IdSubjectNavigation { get; set; } = null!;

    public ICollection<TeachingLoad> TeachingLoads { get; set; } = new List<TeachingLoad>();

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

}
