#nullable enable

namespace LifeGame.Systems;

public class TimeSystem
{
    public int Year { get; private set; } = 1;
    public int Day  { get; private set; } = 1;
    public float Hour { get; private set; } = 6f;

    public event System.Action<float>? OnHoursElapsed;

    public void Update(float realSeconds)
    {
        float elapsedHours = realSeconds * 2f / 60f;
        Hour += elapsedHours;
        while (Hour >= 24f) { Hour -= 24f; Day++; }
        OnHoursElapsed?.Invoke(elapsedHours);
    }

    public void SetTime(int year, int day, float hour)
    {
        Year = year; Day = day; Hour = hour;
    }

    public string Display() =>
        $"Y{Year} D{Day} {(int)Hour:00}:{(int)((Hour % 1) * 60):00}";
}