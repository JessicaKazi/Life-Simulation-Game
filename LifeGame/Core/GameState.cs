#nullable enable

using LifeGame.Entities;
using LifeGame.Systems;

namespace LifeGame.Core;

public enum Screen { MainMenu, CharacterCreation, InGame, LoadGame }

public class GameState
{
    public Screen CurrentScreen { get; set; } = Screen.MainMenu;
    public TimeSystem Time { get; set; } = new();
    public Character? Player { get; set; }

    public void ResetToNewGame()
    {
        Player = null;
        Time = new TimeSystem();
        CurrentScreen = Screen.CharacterCreation;
    }
}