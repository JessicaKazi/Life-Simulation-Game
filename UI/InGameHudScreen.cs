#nullable enable

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LifeGame.Core;
using LifeGame.Entities;
using LifeGame.Systems;
using GameScreen = global::LifeGame.Core.Screen;

namespace LifeGame.UI;

public class InGameHudScreen : Screen
{
    private readonly Texture2D _pixel;

#if DEBUG
    private const float AutosaveInterval = 10f;
#else
    private const float AutosaveInterval = 600f;
#endif

    private readonly CareerSystem _career;

    private float _autosaveTimer = 0f;
    private float _sinceLastSave = 0f;

    private string _status = "";
    private Color _statusColor = Color.LightGreen;
    private float _statusTimer = 0f;

    private KeyboardState _prev;
    private bool _firstFrame = true;

    public InGameHudScreen(GameState state, SpriteFont font, Texture2D pixel) : base(state, font)
    {
        _career = new CareerSystem(state);
        _pixel = pixel;
    }

    public override void Update(GameTime gameTime)
    {
        var k = Keyboard.GetState();
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_firstFrame)
        {
            _prev = k;
            _firstFrame = false;
            return;
        }

        var snap = State.Time.Snapshot();
        State.Time.Update(dt);
        var gameHoursElapsed = State.Time.HoursSince(snap.Year, snap.Day, snap.Hour);

        if (State.Player is not null && gameHoursElapsed > 0f)
            State.Player.Needs.Tick(gameHoursElapsed);

        _career.Tick(gameHoursElapsed);

        _autosaveTimer += dt;
        _sinceLastSave += dt;

        while (_autosaveTimer >= AutosaveInterval)
        {
            _autosaveTimer -= AutosaveInterval;
            if (_sinceLastSave < 5f) continue;

            var slot = $"_autosave-{State.CurrentSlot}";
            var ok = SaveSystem.Save(slot, State);
            _sinceLastSave = 0f;

            if (ok) ShowStatus($"Autosaved -> {slot}", true);
            else    ShowStatus("Autosave FAILED (see console)", false);
        }

        if (_statusTimer > 0f)
        {
            _statusTimer -= dt;
            if (_statusTimer <= 0f) _status = "";
        }

        if (k.IsKeyDown(Keys.F5) && !_prev.IsKeyDown(Keys.F5))
        {
            var slot = $"manual-{DateTime.Now:yyyyMMdd-HHmmss}";
            var ok = SaveSystem.Save(slot, State);
            _sinceLastSave = 0f;
            ShowStatus(ok ? $"Saved -> {slot}" : "Save FAILED (see console)", ok);
        }

        if (k.IsKeyDown(Keys.J) && !_prev.IsKeyDown(Keys.J))
            State.CurrentScreen = GameScreen.JobBoard;

        if (k.IsKeyDown(Keys.Q) && !_prev.IsKeyDown(Keys.Q))
        {
            if (State.Player?.Job is not null)
            {
                _career.Quit();
                ShowStatus("You quit your job.", true);
            }
        }

        if (k.IsKeyDown(Keys.Escape) && !_prev.IsKeyDown(Keys.Escape))
            State.CurrentScreen = GameScreen.MainMenu;

        _prev = k;
    }

    private void ShowStatus(string msg, bool success)
    {
        _status = msg;
        _statusColor = success ? Color.LightGreen : Color.Red;
        _statusTimer = 3f;
    }

    public override void Draw(SpriteBatch spriteBatch, GraphicsDevice gd)
    {
        gd.Clear(new Color(30, 30, 40));
        if (State.Player is null) return;

        var p = State.Player;

        spriteBatch.DrawString(Font, $"Life.exe — {State.Time.Display()}", new Vector2(20, 20), Color.White);
        spriteBatch.DrawString(Font, $"{p.Name} ({p.Gender})  |  ${p.Money:0}",
            new Vector2(20, 50), Color.LightGreen);

        DrawCareerPanel(spriteBatch, gd, p.Job);

        int y = 90;
        DrawBar(spriteBatch, "Hunger",  p.Needs.Hunger,  y, Color.Orange);        y += 28;
        DrawBar(spriteBatch, "Energy",  p.Needs.Energy,  y, Color.Cyan);          y += 28;
        DrawBar(spriteBatch, "Hygiene", p.Needs.Hygiene, y, Color.MediumPurple);  y += 28;
        DrawBar(spriteBatch, "Social",  p.Needs.Social,  y, Color.Pink);          y += 28;
        DrawBar(spriteBatch, "Fun",     p.Needs.Fun,     y, Color.Yellow);        y += 40;

        spriteBatch.DrawString(Font, "Traits: " + string.Join(", ", p.Traits),
            new Vector2(20, y), Color.LightGray);
        y += 40;

        spriteBatch.DrawString(Font, "J = Job Board   Q = Quit Job   F5 = Save   Esc = Main Menu",
            new Vector2(20, gd.Viewport.Height - 60), Color.DarkGray);

        var remaining = AutosaveInterval - _autosaveTimer;
        spriteBatch.DrawString(Font, $"Next autosave in {remaining:0.0}s",
            new Vector2(20, gd.Viewport.Height - 90), Color.DarkGray);

        if (!string.IsNullOrEmpty(_status))
            spriteBatch.DrawString(Font, _status,
                new Vector2(20, gd.Viewport.Height - 30), _statusColor);
    }

    private void DrawCareerPanel(SpriteBatch sb, GraphicsDevice gd, Job? job)
    {
        var x = gd.Viewport.Width - 320;
        var y = 20;

        sb.DrawString(Font, "CAREER", new Vector2(x, y), Color.White);
        y += 26;

        if (job is null)
        {
            sb.DrawString(Font, "Unemployed", new Vector2(x, y), Color.Gray);
            sb.DrawString(Font, "Press J to browse jobs.", new Vector2(x, y + 22), Color.DarkGray);
            return;
        }

        sb.DrawString(Font, job.Career.Title, new Vector2(x, y), Color.LightGreen);      y += 24;
        sb.DrawString(Font, $"Rank: {job.Rank}", new Vector2(x, y), Color.LightGray);    y += 22;
        sb.DrawString(Font, $"Pay: ${job.HourlyWage:0}/hr", new Vector2(x, y), Color.Yellow); y += 22;
        sb.DrawString(Font, $"Shifts: {job.ShiftsWorked}/{job.ShiftsForNextRank}", new Vector2(x, y), Color.LightGray); y += 22;
        sb.DrawString(Font, $"Hours: {job.Career.ShiftStartHour:00}:00-{job.Career.ShiftEndHour:00}:00", new Vector2(x, y), Color.Gray); y += 22;

        if (job.IsOnShiftNow)
            sb.DrawString(Font, "ON SHIFT", new Vector2(x, y), Color.Cyan);
        else
            sb.DrawString(Font, "Off shift", new Vector2(x, y), Color.DarkGray);
    }

    private void DrawBar(SpriteBatch sb, string label, float raw, int y, Color color)
    {
        var value = Math.Clamp(raw, 0f, 100f);

        sb.DrawString(Font, label, new Vector2(20, y), Color.LightGray);

        var barX = 140;
        var barWidth = 200;
        sb.Draw(_pixel, new Rectangle(barX, y + 4, barWidth, 14), new Color(60, 60, 70));
        sb.Draw(_pixel, new Rectangle(barX, y + 4, (int)(barWidth * value / 100f), 14), color);
        sb.DrawString(Font, value.ToString("0"), new Vector2(barX + barWidth + 10, y), Color.White);
    }
}