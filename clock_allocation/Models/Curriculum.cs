using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace clock_allocation.Models;

public partial class Curriculum
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdCurriculum { get; set; }

    public int Year { get; set; }

    public int IdSpecialization { get; set; }

    public virtual Specialization IdSpecializationNavigation { get; set; } = null!;

    public virtual ICollection<Module> Modules { get; set; } = new List<Module>();
}