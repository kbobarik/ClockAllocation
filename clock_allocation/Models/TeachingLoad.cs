using System;
using System.Collections.Generic;

namespace clock_allocation.Models;

public partial class TeachingLoad
{
    public int IdTeachingLoad { get; set; }

    public int IdTeacher { get; set; }

    public int IdSemester { get; set; }

    public int IdGroup { get; set; }

    public bool Split { get; set; }

    public int Year { get; set; }

    public virtual Group IdGroupNavigation { get; set; } = null!;

    public virtual Semester IdSemesterNavigation { get; set; } = null!;

    public virtual Teacher IdTeacherNavigation { get; set; } = null!;
}
