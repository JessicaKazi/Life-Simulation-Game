#nullable enable

using LifeGame.Entities;
using LifeGame.Systems;

namespace LifeGame.Core;

public enum Screen
{
    MainMenu,
    CharacterCreation,
    LoadGame,
    InGame,
    JobBoard
}

public class GameState
{
    public Screen CurrentScreen { get; set; } = Screen.MainMenu;
    public TimeSystem Time { get; set; } = new();
    public Character? Player { get; set; }
    public string CurrentSlot { get; set; } = "session1";

    public void ResetToNewGame()
    {
        Player = null;
        Time = new TimeSystem();
        CurrentSlot = $"session-{System.DateTime.Now:yyyyMMdd-HHmmss}";
        CurrentScreen = Screen.CharacterCreation;
    }
}