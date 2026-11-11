using System;
using LifeGame.Entities;
using LifeGame.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LifeGame.UI;

public sealed class HUD : IDisposable
{
    private static readonly Color PanelColor = new(18, 25, 40, 235);
    private static readonly Color MutedTextColor = new(160, 174, 195);
    private readonly Texture2D _pixel;

    public HUD(GraphicsDevice graphicsDevice)
    {
        _pixel = new Texture2D(graphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont font, Character player, TimeSystem time, Viewport viewport)
    {
        const int margin = 24;
        const int padding = 20;
        var panelWidth = Math.Max(0, viewport.Width - margin * 2);

        spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        Fill(spriteBatch, new Rectangle(margin, margin, panelWidth, 92), PanelColor);
        spriteBatch.DrawString(font, "LIFE.EXE", new Vector2(margin + padding, margin + 13), Color.White, 0f, Vector2.Zero, 1.25f, SpriteEffects.None, 0f);
        spriteBatch.DrawString(font, player.Name, new Vector2(margin + padding, margin + 52), MutedTextColor);

        var timeText = time.Display();
        var moneyText = $"${player.Money:0.00}";
        var rightX = viewport.Width - margin - padding;
        spriteBatch.DrawString(font, timeText, new Vector2(rightX - font.MeasureString(timeText).X, margin + 18), Color.White);
        spriteBatch.DrawString(font, moneyText, new Vector2(rightX - font.MeasureString(moneyText).X, margin + 52), new Color(124, 224, 160));

        var needsPanelY = margin + 108;
        const int needsPanelHeight = 294;
        Fill(spriteBatch, new Rectangle(margin, needsPanelY, panelWidth, needsPanelHeight), PanelColor);
        spriteBatch.DrawString(font, "NEEDS", new Vector2(margin + padding, needsPanelY + 16), Color.White);

        var barWidth = Math.Max(0, panelWidth - padding * 2);
        DrawNeed(spriteBatch, font, "Hunger", player.Needs.Hunger, margin + padding, needsPanelY + 56, barWidth);
        DrawNeed(spriteBatch, font, "Energy", player.Needs.Energy, margin + padding, needsPanelY + 100, barWidth);
        DrawNeed(spriteBatch, font, "Hygiene", player.Needs.Hygiene, margin + padding, needsPanelY + 144, barWidth);
        DrawNeed(spriteBatch, font, "Social", player.Needs.Social, margin + padding, needsPanelY + 188, barWidth);
        DrawNeed(spriteBatch, font, "Fun", player.Needs.Fun, margin + padding, needsPanelY + 232, barWidth);

        spriteBatch.End();
    }

    public void Dispose() => _pixel.Dispose();

    private void DrawNeed(SpriteBatch spriteBatch, SpriteFont font, string label, float value, int x, int y, int width)
    {
        var clampedValue = MathHelper.Clamp(value, 0f, 100f);
        var valueText = $"{clampedValue:0}%";
        spriteBatch.DrawString(font, label, new Vector2(x, y), MutedTextColor);
        spriteBatch.DrawString(font, valueText, new Vector2(x + width - font.MeasureString(valueText).X, y), Color.White);

        var barY = y + 24;
        Fill(spriteBatch, new Rectangle(x, barY, width, 8), new Color(48, 59, 78));
        var fillWidth = (int)(width * clampedValue / 100f);
        var fillColor = clampedValue < 25f ? new Color(238, 103, 103) : new Color(91, 190, 151);
        Fill(spriteBatch, new Rectangle(x, barY, fillWidth, 8), fillColor);
    }

    private void Fill(SpriteBatch spriteBatch, Rectangle rectangle, Color color)
    {
        if (rectangle.Width > 0 && rectangle.Height > 0)
        {
            spriteBatch.Draw(_pixel, rectangle, color);
        }
    }
}