#nullable enable

using LifeGame.Entities;
using LifeGame.Systems;

namespace LifeGame.Core;

public enum GameScreen { MainMenu, CharacterCreation, InGame }

public class GameState
{
    public GameScreen CurrentScreen { get; set; } = GameScreen.MainMenu;

    // Live at all times — screens read from here
    public TimeSystem Time { get; } = new();

    // Populated by CharacterCreation, null until then
    public Character? Player { get; set; }

    public void ResetToNewGame()
    {
        Player = null;
        CurrentScreen = GameScreen.CharacterCreation;
    }
}