using System;
using LifeGame.Core;
using LifeGame.Entities;

namespace LifeGame.Systems;

public class CareerSystem
{
    private readonly GameState _state;

    public CareerSystem(GameState state) { _state = state; }

    public bool Hire(Career career)
    {
        if (_state.Player is null) return false;
        if (_state.Player.Job is not null) return false;
        _state.Player.Job = new Job(career);
        return true;
    }

    public bool Quit()
    {
        if (_state.Player?.Job is null) return false;
        _state.Player.Job = null;
        return true;
    }

    public void Tick(float gameHoursElapsed)
    {
        var job = _state.Player?.Job;
        if (job is null) return;

        var hour = _state.Time.Hour;
        var isShiftNow = hour >= job.Career.ShiftStartHour && hour < job.Career.ShiftEndHour;

        if (job.IsOnShiftNow && !isShiftNow)
            OnShiftEnd(job);

        job.IsOnShiftNow = isShiftNow;

        if (isShiftNow && gameHoursElapsed > 0f)
        {
            _state.Player!.Needs.Energy = Math.Max(0f,
                _state.Player.Needs.Energy - 2f * gameHoursElapsed);
        }
    }

    private void OnShiftEnd(Job job)
    {
        if (_state.Player is null) return;

        var hoursWorked = job.Career.ShiftEndHour - job.Career.ShiftStartHour;
        var pay = job.HourlyWage * hoursWorked;
        _state.Player.Money += pay;

        var skill = job.Career.PrimarySkill;
        if (_state.Player.Skills.TryGetValue(skill, out var current))
            _state.Player.Skills[skill] = current + 1;

        job.RecordShift();

        if (job.TryPromote(out var newRank))
        {
            var requiredSkill = 5 + (int)newRank * 5;
            var actualSkill = _state.Player.Skills.TryGetValue(job.Career.PrimarySkill, out var currentSkill)
                ? currentSkill
                : 0;
            if (actualSkill >= requiredSkill)
                job.ApplyPromotion(newRank);
        }

        Console.WriteLine($"[Career] Shift ended - earned ${pay:0}, {job.Career.PrimarySkill} +1");
    }
}