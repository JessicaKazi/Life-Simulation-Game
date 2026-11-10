namespace LifeGame.Systems;

public class TimeSystem
{
    public int Year { get; private set; } = 1;
    public int Day  { get; private set; } = 1;
    public float Hour { get; private set; } = 6f;   // 0..24
    public float GameMinutesPerRealSecond { get; set; } = 2f;

    public event System.Action<float> OnHoursElapsed;

    public void Update(float realSeconds)
    {
        float elapsedGameMinutes = realSeconds * GameMinutesPerRealSecond;
        float elapsedHours = elapsedGameMinutes / 60f;

        Hour += elapsedHours;
        while (Hour >= 24f) { Hour -= 24f; Day++; }
        if (Day > 30) { Day = 1; Year++; }

        if (OnHoursElapsed != null)
        {
            OnHoursElapsed(elapsedHours);
        }
    }

    public string Display() => $"Y{Year} D{Day} {(int)Hour:00}:{(int)((Hour % 1) * 60):00}";
}