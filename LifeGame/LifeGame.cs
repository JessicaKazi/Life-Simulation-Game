using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LifeGame.Core;
using LifeGame.UI;
using GameScreen = global::LifeGame.Core.Screen;

namespace LifeGame;

public class LifeGame : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;
    private SpriteFont _font = null!;
    private Texture2D _pixel = null!;

    private readonly GameState _state = new();
    private global::LifeGame.UI.Screen _screen = null!;

    public LifeGame()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = 1280,
            PreferredBackBufferHeight = 720
        };
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _font = Content.Load<SpriteFont>("DefaultFont");
        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });

        _screen = new MainMenuScreen(_state, _font);
    }

    protected override void Update(GameTime gameTime)
    {
        // Swap screen if state changed
        _screen = _state.CurrentScreen switch
        {
            GameScreen.MainMenu          => _screen is MainMenuScreen          ? _screen : new MainMenuScreen(_state, _font),
            GameScreen.CharacterCreation => _screen is CharacterCreationScreen ? _screen : new CharacterCreationScreen(_state, _font),
            GameScreen.LoadGame          => _screen is LoadGameScreen          ? _screen : new LoadGameScreen(_state, _font),
            GameScreen.InGame            => _screen is InGameHudScreen         ? _screen : new InGameHudScreen(_state, _font, _pixel),
            _ => _screen
        };

        _screen.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _screen.Draw(_spriteBatch, GraphicsDevice);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    protected override void UnloadContent()
    {
        _pixel.Dispose();
        base.UnloadContent();
    }
}