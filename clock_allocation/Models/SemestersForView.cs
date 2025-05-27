using System.Collections.Generic;

namespace clock_allocation.Models;

public class SemestersForView
{
    public string TitleSubject { get; set; }
    public List<int> Hours { get; set; }
}