using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LifeGame.Core;
using GameScreen = global::LifeGame.Core.Screen;

namespace LifeGame.UI;

public class LoadGameScreen : Screen
{
    private List<string> _slots = new();
    private int _cursor = 0;
    private KeyboardState _prev;

    public LoadGameScreen(GameState state, SpriteFont font) : base(state, font)
    {
        Refresh();
    }

    private void Refresh()
    {
        _slots = SaveSystem.ListSlots();
        if (_cursor >= _slots.Count) _cursor = 0;
    }

    public override void Update(GameTime gameTime)
    {
        var k = Keyboard.GetState();

        if (_slots.Count > 0)
        {
            if (k.IsKeyDown(Keys.Down) && !_prev.IsKeyDown(Keys.Down))
                _cursor = (_cursor + 1) % _slots.Count;

            if (k.IsKeyDown(Keys.Up) && !_prev.IsKeyDown(Keys.Up))
                _cursor = (_cursor - 1 + _slots.Count) % _slots.Count;

            if (k.IsKeyDown(Keys.Enter) && !_prev.IsKeyDown(Keys.Enter))
            {
                SaveSystem.Load(_slots[_cursor], State);
                // Load() itself sets CurrentScreen = InGame
            }

            if (k.IsKeyDown(Keys.Delete) && !_prev.IsKeyDown(Keys.Delete))
            {
                SaveSystem.Delete(_slots[_cursor]);
                Refresh();
            }
        }

        if (k.IsKeyDown(Keys.Escape) && !_prev.IsKeyDown(Keys.Escape))
            State.CurrentScreen = GameScreen.MainMenu;

        _prev = k;
    }

    public override void Draw(SpriteBatch spriteBatch, GraphicsDevice gd)
    {
        gd.Clear(new Color(15, 15, 25));

        spriteBatch.DrawString(Font, "LOAD GAME", new Vector2(60, 60), Color.White);

        if (_slots.Count == 0)
        {
            spriteBatch.DrawString(Font, "No saves found. Press Esc to go back.",
                new Vector2(60, 120), Color.Gray);
            return;
        }

        for (int i = 0; i < _slots.Count; i++)
        {
            var color  = i == _cursor ? Color.LightGreen : Color.Gray;
            var prefix = i == _cursor ? "> " : "  ";
            spriteBatch.DrawString(Font, prefix + _slots[i], new Vector2(60, 120 + i * 30), color);
        }

        spriteBatch.DrawString(Font, "Enter = Load   Delete = Delete   Esc = Back",
            new Vector2(60, gd.Viewport.Height - 60), Color.DarkGray);
    }
}