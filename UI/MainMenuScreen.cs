using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LifeGame.Core;

namespace LifeGame.UI;

public class MainMenuScreen : Screen
{
    private readonly string[] _options = { "New Game", "Load Game", "Exit" };
    private int _selected = 0;

    public MainMenuScreen(GameState state, SpriteFont font) : base(state, font) { }

    public override void Update(GameTime gameTime)
    {
        var k = Keyboard.GetState();

        if (k.IsKeyDown(Keys.Down) && !_prevDown)  _selected = (_selected + 1) % _options.Length;
        if (k.IsKeyDown(Keys.Up)   && !_prevUp)    _selected = (_selected - 1 + _options.Length) % _options.Length;

        if (k.IsKeyDown(Keys.Enter) && !_prevEnter)
        {
            switch (_selected)
            {
                case 0: State.ResetToNewGame(); break;
                case 2: System.Environment.Exit(0); break;
            }
        }

        _prevDown = k.IsKeyDown(Keys.Down);
        _prevUp   = k.IsKeyDown(Keys.Up);
        _prevEnter= k.IsKeyDown(Keys.Enter);
    }

    private bool _prevDown, _prevUp, _prevEnter;

    public override void Draw(SpriteBatch spriteBatch, GraphicsDevice gd)
    {
        gd.Clear(new Color(15, 15, 25));

        var title = "L I F E . E X E";
        var titleSize = Font.MeasureString(title);
        spriteBatch.DrawString(Font, title,
            new Vector2((gd.Viewport.Width - titleSize.X) / 2, 100), Color.White);

        for (int i = 0; i < _options.Length; i++)
        {
            var color = i == _selected ? Color.LightGreen : Color.Gray;
            var prefix = i == _selected ? "> " : "  ";
            spriteBatch.DrawString(Font, prefix + _options[i],
                new Vector2(gd.Viewport.Width / 2f - 60, 250 + i * 40), color);
        }
    }
}