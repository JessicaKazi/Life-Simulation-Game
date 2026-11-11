using System;
using LifeGame.Core;
using LifeGame.Entities;

namespace LifeGame.Systems;

public class HomeSystem
{
    private readonly GameState _state;
    private int _lastDaySeen;
    private int _lastYearSeen;

    public event Action<Home, decimal>? OnRentPaid;
    public event Action<Home>? OnEvicted;

    public HomeSystem(GameState state)
    {
        _state = state;
        _lastDaySeen  = state.Time.Day;
        _lastYearSeen = state.Time.Year;
    }

    public bool Rent(Home home)
    {
        if (_state.Player is null) return false;
        if (_state.Player.Money < home.WeeklyRent) return false;

        _state.Player.Home = home;
        _state.Player.Money -= home.WeeklyRent;
        _state.Player.DaysSinceRentPaid = 0;
        return true;
    }

    public void Leave()
    {
        if (_state.Player is null) return;
        _state.Player.Home = null;
        _state.Player.DaysSinceRentPaid = 0;
    }

    public void Tick()
    {
        if (_state.Player?.Home is null) return;

        var t = _state.Time;

        var dayAdvanced = (t.Year > _lastYearSeen) ||
                          (t.Year == _lastYearSeen && t.Day > _lastDaySeen);

        if (!dayAdvanced) return;

        _lastDaySeen  = t.Day;
        _lastYearSeen = t.Year;

        _state.Player.DaysSinceRentPaid++;

        if (_state.Player.DaysSinceRentPaid < _state.Player.DaysBetweenRentPayments) return;

        var home = _state.Player.Home;
        var rent = home.WeeklyRent;

        if (_state.Player.Money >= rent)
        {
            _state.Player.Money -= rent;
            _state.Player.DaysSinceRentPaid = 0;
            OnRentPaid?.Invoke(home, rent);
        }
        else
        {
            _state.Player.Home = null;
            _state.Player.DaysSinceRentPaid = 0;
            OnEvicted?.Invoke(home);
        }
    }
}