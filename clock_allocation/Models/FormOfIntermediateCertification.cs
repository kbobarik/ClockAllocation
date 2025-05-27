using System;
using System.Collections.Generic;

namespace clock_allocation.Models;

public partial class FormOfIntermediateCertification
{
    public int IdFormOfIntermediateCertification { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<Semester> Semesters { get; set; } = new List<Semester>();
}
