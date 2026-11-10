using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LifeGame.Core;
using GameScreen = global::LifeGame.Core.Screen;

namespace LifeGame.UI;

public class InGameHudScreen : Screen
{
    private readonly Texture2D _pixel;
    private float _autosaveTimer = 0f;
    private const float AutosaveInterval = 10f;   // real seconds — doc says 10-15 min, using 10s for testing
    private string _status = "";
    private float _statusTimer = 0f;
    private KeyboardState _prev;

    public InGameHudScreen(GameState state, SpriteFont font, Texture2D pixel) : base(state, font)
    {
        _pixel = pixel;
    }

    public override void Update(GameTime gameTime)
    {
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        State.Time.Update(dt);

        if (State.Player is not null)
            State.Player.Needs.Tick(dt * 0.005f);

        _autosaveTimer += dt;
        if (_autosaveTimer >= AutosaveInterval)
        {
            _autosaveTimer = 0f;
            SaveSystem.Save("slot1", State);
            ShowStatus("Autosaved → slot1");
        }

        if (_statusTimer > 0f)
        {
            _statusTimer -= dt;
            if (_statusTimer <= 0f) _status = "";
        }

        var k = Keyboard.GetState();
        if (k.IsKeyDown(Keys.F5) && !_prev.IsKeyDown(Keys.F5))
        {
            SaveSystem.Save("slot1", State);
            ShowStatus("Saved → slot1");
        }
        if (k.IsKeyDown(Keys.Escape) && !_prev.IsKeyDown(Keys.Escape))
        {
            State.CurrentScreen = GameScreen.MainMenu;
        }
        _prev = k;
    }

    private void ShowStatus(string s) { _status = s; _statusTimer = 2f; }

    public override void Draw(SpriteBatch spriteBatch, GraphicsDevice gd)
    {
        gd.Clear(new Color(30, 30, 40));
        if (State.Player is null) return;

        var p = State.Player;

        spriteBatch.DrawString(Font, $"Life.exe — {State.Time.Display()}", new Vector2(20, 20), Color.White);
        spriteBatch.DrawString(Font, $"{p.Name} ({p.Gender})  |  ${p.Money:0}", new Vector2(20, 50), Color.LightGreen);

        int y = 90;
        DrawBar(spriteBatch, "Hunger", p.Needs.Hunger, y, Color.Orange);         y += 28;
        DrawBar(spriteBatch, "Energy", p.Needs.Energy, y, Color.Cyan);           y += 28;
        DrawBar(spriteBatch, "Hygiene", p.Needs.Hygiene, y, Color.MediumPurple); y += 28;
        DrawBar(spriteBatch, "Social", p.Needs.Social, y, Color.Pink);           y += 28;
        DrawBar(spriteBatch, "Fun", p.Needs.Fun, y, Color.Yellow);               y += 40;

        spriteBatch.DrawString(Font, "Traits: " + string.Join(", ", p.Traits),
            new Vector2(20, y), Color.LightGray);
        y += 40;

        spriteBatch.DrawString(Font, "F5 = Save    Esc = Main Menu",
            new Vector2(20, gd.Viewport.Height - 60), Color.DarkGray);

        if (_status != "")
            spriteBatch.DrawString(Font, _status,
                new Vector2(20, gd.Viewport.Height - 30), Color.LightGreen);
    }

    private void DrawBar(SpriteBatch sb, string label, float value, int y, Color color)
    {
        sb.DrawString(Font, label, new Vector2(20, y), Color.LightGray);
        var barX = 140; var barWidth = 200;
        sb.Draw(_pixel, new Rectangle(barX, y + 4, barWidth, 14), new Color(60, 60, 70));
        sb.Draw(_pixel, new Rectangle(barX, y + 4, (int)(barWidth * value / 100f), 14), color);
        sb.DrawString(Font, value.ToString("0"), new Vector2(barX + barWidth + 10, y), Color.White);
    }
}