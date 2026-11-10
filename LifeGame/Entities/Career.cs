using System.Collections.Generic;

namespace LifeGame.Entities;

public class Career
{
    public string Title { get; }
    public string Description { get; }
    public string PrimarySkill { get; }
    public decimal BaseHourlyWage { get; }
    public int ShiftStartHour { get; }
    public int ShiftEndHour { get; }

    public Career(string title, string description, string primarySkill,
                  decimal baseHourlyWage, int shiftStartHour, int shiftEndHour)
    {
        Title = title;
        Description = description;
        PrimarySkill = primarySkill;
        BaseHourlyWage = baseHourlyWage;
        ShiftStartHour = shiftStartHour;
        ShiftEndHour = shiftEndHour;
    }

    public static readonly IReadOnlyList<Career> Catalog = new List<Career>
    {
        new("Programmer", "Write software for a tech company.",    "Programming", 25m, 9, 17),
        new("Doctor",     "Treat patients at the city hospital.",  "Logic",       45m, 7, 15),
        new("Teacher",    "Educate the next generation.",          "Charisma",    20m, 8, 15),
        new("Chef",       "Cook for the downtown bistro.",         "Cooking",     22m, 10, 18),
        new("Artist",     "Sell paintings at the old town studio.","Creativity",  18m, 11, 18),
    };
}