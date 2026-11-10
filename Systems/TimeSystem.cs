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

        while (Hour >= 24f)
        {
            Hour -= 24f;
            Day++;
            if (Day > 30)
            {
                Day = 1;
                Year++;
            }
        }

        OnHoursElapsed?.Invoke(elapsedHours);
    }

    public void SetTime(int year, int day, float hour)
    {
        Year = year;
        Day  = day;
        Hour = hour;
    }

    public (int Year, int Day, float Hour) Snapshot() => (Year, Day, Hour);

    public float HoursSince(int year, int day, float hour)
    {
        var now  = Year * 720f + Day * 24f + Hour;
        var then = year * 720f + day * 24f + hour;
        return now - then;
    }

    public string Display() =>
        $"Y{Year} D{Day} {(int)Hour:00}:{(int)((Hour % 1) * 60):00}";
}