using System;
using System.Collections.Generic;

namespace clock_allocation.Models;

public partial class Language
{
    public int IdLanguage { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<Specialization> Specializations { get; set; } = new List<Specialization>();
}
