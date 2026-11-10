using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LifeGame.Core;
using LifeGame.Entities;
using GameScreen = global::LifeGame.Core.Screen;

namespace LifeGame.UI;

public class JobBoardScreen : Screen
{
    private readonly IReadOnlyList<Career> _careers = Career.Catalog;
    private int _cursor = 0;

    private string _status = "";
    private Color _statusColor = Color.LightGreen;
    private float _statusTimer = 0f;

    private KeyboardState _prev;
    private bool _firstFrame = true;

    public JobBoardScreen(GameState state, SpriteFont font) : base(state, font) { }

    public override void Update(GameTime gameTime)
    {
        var k = Keyboard.GetState();
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_statusTimer > 0f)
        {
            _statusTimer -= dt;
            if (_statusTimer <= 0f) _status = "";
        }

        if (_firstFrame)
        {
            _prev = k;
            _firstFrame = false;
            return;
        }

        if (k.IsKeyDown(Keys.Down) && !_prev.IsKeyDown(Keys.Down))
            _cursor = (_cursor + 1) % _careers.Count;
        if (k.IsKeyDown(Keys.Up) && !_prev.IsKeyDown(Keys.Up))
            _cursor = (_cursor - 1 + _careers.Count) % _careers.Count;

        if (k.IsKeyDown(Keys.Enter) && !_prev.IsKeyDown(Keys.Enter))
        {
            if (State.Player?.Job is not null)
            {
                ShowStatus("You already have a job. Quit it first.", false);
            }
            else
            {
                var career = _careers[_cursor];
                State.Player!.Job = new Job(career);
                State.CurrentScreen = GameScreen.InGame;
            }
        }

        if (k.IsKeyDown(Keys.Escape) && !_prev.IsKeyDown(Keys.Escape))
            State.CurrentScreen = GameScreen.InGame;

        _prev = k;
    }

    private void ShowStatus(string msg, bool ok)
    {
        _status = msg;
        _statusColor = ok ? Color.LightGreen : Color.OrangeRed;
        _statusTimer = 3f;
    }

    public override void Draw(SpriteBatch spriteBatch, GraphicsDevice gd)
    {
        gd.Clear(new Color(20, 20, 30));

        spriteBatch.DrawString(Font, "JOB BOARD", new Vector2(60, 60), Color.White);
        spriteBatch.DrawString(Font, "Choose a career to begin.", new Vector2(60, 100), Color.Gray);

        int y = 150;
        for (int i = 0; i < _careers.Count; i++)
        {
            var c = _careers[i];
            var isCursor = i == _cursor;
            var color = isCursor ? Color.LightGreen : Color.Gray;
            var prefix = isCursor ? "> " : "  ";

            spriteBatch.DrawString(Font, $"{prefix}{c.Title}  ({c.BaseHourlyWage:0}/hr)",
                new Vector2(60, y), color);
            y += 24;

            spriteBatch.DrawString(Font, $"   Shift: {c.ShiftStartHour:00}:00-{c.ShiftEndHour:00}:00  |  Skill: {c.PrimarySkill}",
                new Vector2(60, y), isCursor ? Color.LightGray : Color.DarkGray);
            y += 32;   
        }
    }
}