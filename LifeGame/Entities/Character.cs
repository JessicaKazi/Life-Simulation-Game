using System.Collections.Generic;

namespace LifeGame.Entities;

public enum Personality
{
    Friendly, Ambitious, Lazy, Creative, Shy,
    Confident, Funny, Serious, Adventurous, Introverted
}

public class Character
{
    public string Name { get; set; } = "Unnamed";
    public int Age { get; set; } = 18;
    public string Gender { get; set; } = "Female";
    public List<Personality> Traits { get; } = new();
    public Needs Needs { get; } = new();
    public decimal Money { get; set; } = 500m;
    public Job? Job { get; set; }
    public Home? Home { get; set; }
    public int DaysSinceRentPaid { get; set; } = 0;
    public int DaysBetweenRentPayments { get; } = 7;

    public Dictionary<string, int> Skills { get; } = new()
    {
        { "Cooking", 0 }, { "Programming", 0 }, { "Fitness", 0 },
        { "Creativity", 0 }, { "Charisma", 0 }, { "Logic", 0 },
        { "Handiness", 0 }, { "Fishing", 0 }
    };
}

public class Needs
{
    public float Hunger  { get; set; } = 100f;
    public float Energy  { get; set; } = 100f;
    public float Hygiene { get; set; } = 100f;
    public float Social  { get; set; } = 100f;
    public float Fun     { get; set; } = 100f;

    public void Tick(float hoursElapsed)
    {
        Hunger  = Clamp(Hunger  - 4f * hoursElapsed);
        Energy  = Clamp(Energy  - 3f * hoursElapsed);
        Hygiene = Clamp(Hygiene - 2f * hoursElapsed);
        Social  = Clamp(Social  - 1.5f * hoursElapsed);
        Fun     = Clamp(Fun     - 2f * hoursElapsed);
    }

    private static float Clamp(float v) => v < 0f ? 0f : v > 100f ? 100f : v;
}