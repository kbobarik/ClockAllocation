using System.ComponentModel.DataAnnotations;

namespace clock_allocation.Models;

public class AcademicPlanRecord
{
   
    public string? ModuleCode { get; set; }
    public string? ModuleTitle { get; set; }
    public string? SubjectCode { get; set; }
    public string? SubjectName { get; set; }

    public string? Semester1Assessment { get; set; }
    public string? Semester2Assessment { get; set; }
    public string? Semester3Assessment { get; set; }
    public string? Semester4Assessment { get; set; }
    public string? Semester5Assessment { get; set; }
    public string? Semester6Assessment { get; set; }
    public string? Semester7Assessment { get; set; }
    public string? Semester8Assessment { get; set; }

    public int? TotalHours { get; set; }
    public int? TotalIntermediateCertification { get; set; }
    public int? TotalIndependentWork { get; set; }
    public int? Classes { get; set; }
    public int? Lessons { get; set; }
    public int? Practice { get; set; }
    public int? CourseDesign { get; set; }
    public int? EducationalAndIndustrialPractice { get; set; }

    public int? Semester1TotalHours { get; set; }
    public int? Semester1IntermediateCertification { get; set; }
    public int? Semester1Lessons { get; set; }
    public int? Semester1Consultations { get; set; }
    public int? Semester1Practice { get; set; }
    public int? Semester1IndependentWork { get; set; }

    public int? Semester2TotalHours { get; set; }
    public int? Semester2IntermediateCertification { get; set; }
    public int? Semester2Lessons { get; set; }
    public int? Semester2Consultations { get; set; }
    public int? Semester2Practice { get; set; }
    public int? Semester2IndependentWork { get; set; }

    public int? Semester3TotalHours { get; set; }
    public int? Semester3IntermediateCertification { get; set; }
    public int? Semester3Lessons { get; set; }
    public int? Semester3Consultations { get; set; }
    public int? Semester3Practice { get; set; }
    public int? Semester3IndependentWork { get; set; }

    public int? Semester4TotalHours { get; set; }
    public int? Semester4IntermediateCertification { get; set; }
    public int? Semester4Lessons { get; set; }
    public int? Semester4Consultations { get; set; }
    public int? Semester4Practice { get; set; }
    public int? Semester4IndependentWork { get; set; }

    public int? Semester5TotalHours { get; set; }
    public int? Semester5IntermediateCertification { get; set; }
    public int? Semester5Lessons { get; set; }
    public int? Semester5Consultations { get; set; }
    public int? Semester5Practice { get; set; }
    public int? Semester5IndependentWork { get; set; }

    public int? Semester6TotalHours { get; set; }
    public int? Semester6IntermediateCertification { get; set; }
    public int? Semester6Lessons { get; set; }
    public int? Semester6Consultations { get; set; }
    public int? Semester6Practice { get; set; }
    public int? Semester6IndependentWork { get; set; }

    public int? Semester7TotalHours { get; set; }
    public int? Semester7IntermediateCertification { get; set; }
    public int? Semester7Lessons { get; set; }
    public int? Semester7Consultations { get; set; }
    public int? Semester7Practice { get; set; }
    public int? Semester7IndependentWork { get; set; }

    public int? Semester8TotalHours { get; set; }
    public int? Semester8IntermediateCertification { get; set; }
    public int? Semester8Lessons { get; set; }
    public int? Semester8Consultations { get; set; }
    public int? Semester8Practice { get; set; }
    public int? Semester8IndependentWork { get; set; }
}