using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace clock_allocation.Models;

public partial class TeachingLoad
{
    public int IdTeachingLoad { get; set; }

    public int IdTeacher { get; set; }

    public int IdSemester { get; set; }

    public int IdGroup { get; set; }

    public bool Split { get; set; }

    public int Year { get; set; }

    public int IdSubject { get; set; }

    public int MaximumWorkload { get; set; }

    public int HoursPerWeek { get; set; }

    public int Lectures { get; set; }

    public int Laboratory { get; set; }

    public int CourseDesign { get; set; }

    public int IndependentWork { get; set; }

    public int Consultations { get; set; }

    public int Exam { get; set; }

    public int СonsultationsPayment { get; set; }

    public int CheckingCoursework { get; set; }

    public int ReviewAndLecture { get; set; }

    public int Billing { get; set; }

    public int TotalYear { get; set; }
    public int AcademicHoursPerYear { get; set; }

    public virtual Group IdGroupNavigation { get; set; } = null!;

    public virtual Semester IdSemesterNavigation { get; set; } = null!;

    public virtual Subject IdSubjectNavigation { get; set; } = null!;

    public virtual Teacher IdTeacherNavigation { get; set; } = null!;
        
    
}