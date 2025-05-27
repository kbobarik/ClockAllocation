using System;
using System.Collections.Generic;

namespace clock_allocation.Models;

public partial class Teacher
{
    public int IdTeacher { get; set; }

    public string Surname { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Patronymic { get; set; }
    
    public string Login { get; set; } = null!;
    
    public string Password { get; set; } = null!;

    public virtual ICollection<TeachingLoad> TeachingLoads { get; set; } = new List<TeachingLoad>();

    public override string ToString()
    {
        var name =  Surname + " " + Name[0] + ".";
        if (!string.IsNullOrEmpty(Patronymic))
        {
            name += " " + Patronymic[0]+".";
        }
        return name;
    }
}
