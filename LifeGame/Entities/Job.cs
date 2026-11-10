namespace LifeGame.Entities;

public enum CareerRank
{
    EntryLevel  = 0,
    Junior      = 1,
    Experienced = 2,
    Senior      = 3,
    Manager     = 4,
    Executive   = 5
}

public class Job
{
    public Career Career { get; }
    public CareerRank Rank { get; private set; } = CareerRank.EntryLevel;
    public int ShiftsWorked { get; private set; } = 0;
    public bool IsOnShiftNow { get; set; } = false;

    public int ShiftsForNextRank => (int)(Rank + 1) * 3;

    public decimal HourlyWage => Career.BaseHourlyWage * (1m + (int)Rank * 0.25m);

    public Job(Career career) { Career = career; }

    public Job(Career career, CareerRank rank, int shiftsWorked)
    {
        Career = career;
        Rank = rank;
        ShiftsWorked = shiftsWorked;
    }

    public bool TryPromote(out CareerRank newRank)
    {
        newRank = Rank;
        if (Rank >= CareerRank.Executive) return false;
        if (ShiftsWorked < ShiftsForNextRank) return false;
        newRank = Rank + 1;
        return true;
    }

    public void ApplyPromotion(CareerRank newRank)
    {
        Rank = newRank;
        ShiftsWorked = 0;
    }

    public void RecordShift() => ShiftsWorked++;
}