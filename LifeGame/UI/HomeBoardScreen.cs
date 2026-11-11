using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LifeGame.Core;
using LifeGame.Entities;
using LifeGame.Systems;
using GameScreen = global::LifeGame.Core.Screen;

namespace LifeGame.UI;

public class HomeBoardScreen : Screen
{
    private readonly IReadOnlyList<Home> _listings = Home.Listings;
    private int _cursor = 0;

    private readonly HomeSystem _homeSystem;

    private string _status = "";
    private Color _statusColor = Color.LightGreen;
    private float _statusTimer = 0f;

    private KeyboardState _prev;
    private bool _firstFrame = true;

    public HomeBoardScreen(GameState state, SpriteFont font) : base(state, font)
    {
        _homeSystem = new HomeSystem(state);
    }

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
            _cursor = (_cursor + 1) % _listings.Count;
        if (k.IsKeyDown(Keys.Up) && !_prev.IsKeyDown(Keys.Up))
            _cursor = (_cursor - 1 + _listings.Count) % _listings.Count;

        if (k.IsKeyDown(Keys.Enter) && !_prev.IsKeyDown(Keys.Enter))
        {
            if (State.Player?.Home is not null)
            {
                ShowStatus("You already have a home. Leave it first (L).", false);
            }
            else if (_homeSystem.Rent(_listings[_cursor]))
            {
                ShowStatus($"Rented {_listings[_cursor].Address}!", true);
                State.CurrentScreen = GameScreen.InGame;
            }
            else
            {
                ShowStatus("Not enough money for first week's rent.", false);
            }
        }

        if (k.IsKeyDown(Keys.L) && !_prev.IsKeyDown(Keys.L))
        {
            if (State.Player?.Home is not null)
            {
                _homeSystem.Leave();
                ShowStatus("You moved out.", true);
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

        spriteBatch.DrawString(Font, "FIND A HOME", new Vector2(60, 60), Color.White);
        spriteBatch.DrawString(Font, "Weekly rent is charged every 7 in-game days.", new Vector2(60, 100), Color.Gray);
        spriteBatch.DrawString(Font, $"Your balance: ${State.Player?.Money:0}", new Vector2(60, 124), Color.Yellow);

        int y = 170;
        for (int i = 0; i < _listings.Count; i++)
        {
            var h = _listings[i];
            var isCursor = i == _cursor;
            var color = isCursor ? Color.LightGreen : Color.Gray;
            var prefix = isCursor ? "> " : "  ";
            var stars = new string('*', h.Quality);

            spriteBatch.DrawString(Font,
                $"{prefix}{h.Address}  ({h.District.Name})  {stars}",
                new Vector2(60, y), color);
            y += 24;

            spriteBatch.DrawString(Font,
                $"   ${h.WeeklyRent}/week   -   {h.Description}",
                new Vector2(60, y), isCursor ? Color.LightGray : Color.DarkGray);
            y += 32;
        }

        var panelX = 700;
        var panelY = 170;
        spriteBatch.DrawString(Font, "YOUR HOME", new Vector2(panelX, panelY), Color.White);
        panelY += 26;

        if (State.Player?.Home is null)
        {
            spriteBatch.DrawString(Font, "None (rent-free, no address)", new Vector2(panelX, panelY), Color.Gray);
        }
        else
        {
            var h = State.Player.Home;
            var daysLeft = State.Player.DaysBetweenRentPayments - State.Player.DaysSinceRentPaid;
            spriteBatch.DrawString(Font, h.Address, new Vector2(panelX, panelY), Color.LightGreen); panelY += 24;
            spriteBatch.DrawString(Font, h.District.Name, new Vector2(panelX, panelY), Color.LightGray); panelY += 24;
            spriteBatch.DrawString(Font, $"${h.WeeklyRent}/week", new Vector2(panelX, panelY), Color.Yellow); panelY += 24;
            spriteBatch.DrawString(Font, $"Next rent in {daysLeft} day(s)", new Vector2(panelX, panelY), Color.Cyan);
        }

        spriteBatch.DrawString(Font, "Enter = Rent   L = Leave home   Esc = Back",
            new Vector2(60, gd.Viewport.Height - 60), Color.DarkGray);

        if (!string.IsNullOrEmpty(_status))
            spriteBatch.DrawString(Font, _status,
                new Vector2(60, gd.Viewport.Height - 30), _statusColor);
    }
}