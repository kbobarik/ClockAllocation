using System;
using System.Collections.Generic;

namespace clock_allocation.Models;

public partial class Specialization
{
    public int IdSpecialization { get; set; }

    public string Code { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Direction { get; set; } = null!;

    public string Qualification { get; set; } = null!;

    public string EducationLevel { get; set; } = null!;

    public string FormOfEducation { get; set; } = null!;

    public string DurationOfTraining { get; set; } = null!;

    public int IdLanguage { get; set; }

    public virtual ICollection<Curriculum> Curricula { get; set; } = new List<Curriculum>();

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual Language IdLanguageNavigation { get; set; } = null!;
    public override string ToString()
    {
        return $"{Qualification}, {FormOfEducation}";
    }
}