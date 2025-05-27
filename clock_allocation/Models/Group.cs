using System;
using System.Collections.Generic;

namespace clock_allocation.Models;

public partial class Group
{
    public int IdGroup { get; set; }

    public int StreamNumber { get; set; }

    public int YearOfAdmission { get; set; }

    public int IdSpecialization { get; set; }
    public string FormOfEducation { get; set; }
    
    public bool Splitting { get; set; }

    public virtual Specialization IdSpecializationNavigation { get; set; } = null!;

    public virtual ICollection<TeachingLoad> TeachingLoads { get; set; } = new List<TeachingLoad>();
}
