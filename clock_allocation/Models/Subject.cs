using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using clock_allocation.Models;

public partial class Subject : INotifyPropertyChanged
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdSubject { get; set; }
    public virtual ICollection<TeachingLoad> TeachingLoads { get; set; } = new List<TeachingLoad>();

    private string _title;

    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged(nameof(Title));
        }
    }

    private string _code;
    public string Code 
    {
        get => _code;
        set
        {
            _code = value;
            OnPropertyChanged(nameof(Code));
        }
    }

    public int IdModule { get; set; }

    private int _courseDesign;

    public int CourseDesign
    {
        get => _courseDesign;
        set
        {
            _courseDesign = value;
            OnPropertyChanged(nameof(CourseDesign));
            OnPropertyChanged(nameof(Classes));
            OnPropertyChanged(nameof(TotalHours));
        }
    }

    private int _lessons;

    public int Lessons
    {
        get => _lessons;
        set
        {
            _lessons = value;
            OnPropertyChanged(nameof(Lessons));
            OnPropertyChanged(nameof(Classes));
            OnPropertyChanged(nameof(TotalHours));
        }
    }

    private int _labPrac;

    public int LaboratoryAndPracticalExercises
    {
        get => _labPrac;
        set
        {
            _labPrac = value;
            OnPropertyChanged(nameof(LaboratoryAndPracticalExercises));
            OnPropertyChanged(nameof(Classes));
            OnPropertyChanged(nameof(TotalHours));
        }
    }


    private Module _idModuleNavigation;

    public virtual Module IdModuleNavigation
    {
        get => _idModuleNavigation;
        set
        {
            _idModuleNavigation = value;
            OnPropertyChanged(nameof(IdModuleNavigation));
        }
    }

    private ObservableCollection<Semester> _semesters = new();

    public virtual ObservableCollection<Semester> Semesters
    {
        get => _semesters;
        set
        {
            if (_semesters != null)
                foreach (var s in _semesters)
                    s.PropertyChanged -= Semester_PropertyChanged;

            _semesters = value;

            if (_semesters != null)
                foreach (var s in _semesters)
                    s.PropertyChanged += Semester_PropertyChanged;

            OnPropertyChanged(nameof(Semesters));
            OnPropertyChanged(nameof(TotalIntermediateCertification));
            OnPropertyChanged(nameof(TotalIndependentWork));
            OnPropertyChanged(nameof(EducationalAndIndustrialPractice));
            OnPropertyChanged(nameof(TotalHours));
        }
    }

    private void Semester_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        
        if (e.PropertyName is nameof(Semester.IntermediateCertification))
            OnPropertyChanged(nameof(TotalIntermediateCertification));

        if (e.PropertyName is nameof(Semester.IndependentWork))
            OnPropertyChanged(nameof(TotalIndependentWork));

        if (e.PropertyName is nameof(Semester.Practice))
            OnPropertyChanged(nameof(EducationalAndIndustrialPractice));

        if (e.PropertyName is
            nameof(Semester.IntermediateCertification) or
            nameof(Semester.IndependentWork) or
            nameof(Semester.Practice))
        {
            OnPropertyChanged(nameof(TotalHours));
        }
    }
   


    [NotMapped]
    public int TotalIntermediateCertification =>
        Semesters?.Sum(s => s.IntermediateCertification) ?? 0;

    [NotMapped]
    public int TotalIndependentWork =>
        Semesters?.Sum(s => s.IndependentWork) ?? 0;

    [NotMapped] public int EducationalAndIndustrialPractice => Semesters?.Sum(s => s.Practice) ?? 0;

    [NotMapped]
    public int Classes =>
        Lessons + LaboratoryAndPracticalExercises + CourseDesign;

    [NotMapped]
    public int TotalHours =>
        TotalIntermediateCertification +
        TotalIndependentWork +
        Lessons +
        LaboratoryAndPracticalExercises +
        CourseDesign;

    [NotMapped] public bool Splitting;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public override string ToString()
    {
        var title = "";
        if (!string.IsNullOrEmpty(Code))
        {
            title = $"{Code}";
        }
        title += " "+Title;
        return title;
    }
}