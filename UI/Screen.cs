using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LifeGame.Core;

namespace LifeGame.UI;

public abstract class Screen
{
    protected GameState State { get; }
    protected SpriteFont Font { get; }

    protected Screen(GameState state, SpriteFont font)
    {
        State = state;
        Font = font;
    }

    public abstract void Update(GameTime gameTime);
    public abstract void Draw(SpriteBatch spriteBatch, GraphicsDevice gd);
}