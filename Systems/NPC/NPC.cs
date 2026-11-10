using LifeGame.Entities;

namespace LifeGame.Systems.NPC;

public class NPC
{
    public Character Character { get; } = new();
    public bool HasJob { get; set; } = true;
    public int WorkStartHour { get; set; } = 9;
    public int WorkEndHour { get; set; } = 17;

    public bool IsWorkHours() => Character != null;

    // Simple placeholder — will use TimeSystem.Hour when wired up
    public bool IsWorkHours(int hour) => hour >= WorkStartHour && hour < WorkEndHour;
}