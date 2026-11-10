using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LifeGame.Core;
using LifeGame.Entities;
using GameScreen = LifeGame.Core.GameScreen;

namespace LifeGame.UI;

public class CharacterCreationScreen : Screen
{
    private enum Step { Name, Gender, Traits, Confirm }
    private Step _step = Step.Name;

    private string _name = "";
    private int _genderIndex = 0;
    private readonly string[] _genders = { "Female", "Male" };

    private readonly Personality[] _allTraits = (Personality[])Enum.GetValues(typeof(Personality));
    private readonly List<int> _selectedTraits = new();
    private int _traitCursor = 0;

    // Input edge-detection so each keypress registers once
    private KeyboardState _prev;

    public CharacterCreationScreen(GameState state, SpriteFont font) : base(state, font) { }

    public override void Update(GameTime gameTime)
    {
        var k = Keyboard.GetState();

        switch (_step)
        {
            case Step.Name:     HandleNameInput(k);     break;
            case Step.Gender:   HandleGenderInput(k);   break;
            case Step.Traits:   HandleTraitInput(k);    break;
            case Step.Confirm:  HandleConfirmInput(k);  break;
        }

        _prev = k;
    }

    // ---- NAME STEP --------------------------------------------------------
    private void HandleNameInput(KeyboardState k)
    {
        foreach (var key in k.GetPressedKeys())
        {
            if (_prev.IsKeyDown(key)) continue;

            if (key == Keys.Back && _name.Length > 0)
                _name = _name[..^1];
            else if (key == Keys.Enter && _name.Length >= 2)
                _step = Step.Gender;
            else
            {
                var ch = KeyToChar(key, k);
                if (ch.HasValue && _name.Length < 20)
                    _name += ch.Value;
            }
        }
    }

    // ---- GENDER STEP ------------------------------------------------------
    private void HandleGenderInput(KeyboardState k)
    {
        if (k.IsKeyDown(Keys.Left)  && !_prev.IsKeyDown(Keys.Left))  _genderIndex = (_genderIndex - 1 + _genders.Length) % _genders.Length;
        if (k.IsKeyDown(Keys.Right) && !_prev.IsKeyDown(Keys.Right)) _genderIndex = (_genderIndex + 1) % _genders.Length;
        if (k.IsKeyDown(Keys.Enter) && !_prev.IsKeyDown(Keys.Enter)) _step = Step.Traits;
        if (k.IsKeyDown(Keys.Escape))                                 _step = Step.Name;
    }

    // ---- TRAITS STEP ------------------------------------------------------
    private void HandleTraitInput(KeyboardState k)
    {
        if (k.IsKeyDown(Keys.Down) && !_prev.IsKeyDown(Keys.Down))
            _traitCursor = (_traitCursor + 1) % _allTraits.Length;
        if (k.IsKeyDown(Keys.Up) && !_prev.IsKeyDown(Keys.Up))
            _traitCursor = (_traitCursor - 1 + _allTraits.Length) % _allTraits.Length;

        if (k.IsKeyDown(Keys.Space) && !_prev.IsKeyDown(Keys.Space))
        {
            if (_selectedTraits.Contains(_traitCursor)) _selectedTraits.Remove(_traitCursor);
            else if (_selectedTraits.Count < 3)          _selectedTraits.Add(_traitCursor);
        }

        if (k.IsKeyDown(Keys.Enter) && !_prev.IsKeyDown(Keys.Enter) && _selectedTraits.Count >= 1)
            _step = Step.Confirm;
        if (k.IsKeyDown(Keys.Escape))
            _step = Step.Gender;
    }

    // ---- CONFIRM STEP -----------------------------------------------------
    private void HandleConfirmInput(KeyboardState k)
    {
        if (k.IsKeyDown(Keys.Enter) && !_prev.IsKeyDown(Keys.Enter))
        {
            var player = new Character { Name = _name, Gender = _genders[_genderIndex] };
            foreach (var idx in _selectedTraits)
                player.Traits.Add(_allTraits[idx]);

            State.Player = player;
            State.CurrentScreen = GameScreen.InGame;
        }
        if (k.IsKeyDown(Keys.Escape))
            _step = Step.Traits;
    }

    // ---- RENDER -----------------------------------------------------------
    public override void Draw(SpriteBatch spriteBatch, GraphicsDevice gd)
    {
        gd.Clear(new Color(20, 20, 30));

        float x = 60, y = 60;
        spriteBatch.DrawString(Font, "CREATE YOUR CHARACTER", new Vector2(x, y), Color.White);
        y += 60;

        // Name
        var nameColor = _step == Step.Name ? Color.LightGreen : Color.Gray;
        spriteBatch.DrawString(Font, $"Name: {_name}_", new Vector2(x, y), nameColor);
        y += 40;

        // Gender
        var genderColor = _step == Step.Gender ? Color.LightGreen : Color.Gray;
        spriteBatch.DrawString(Font, $"Gender: < {_genders[_genderIndex]} >", new Vector2(x, y), genderColor);
        y += 40;

        // Traits
        var traitHeader = _step == Step.Traits ? Color.LightGreen : Color.Gray;
        spriteBatch.DrawString(Font, "Traits (pick 1-3, Space to toggle):", new Vector2(x, y), traitHeader);
        y += 30;

        for (int i = 0; i < _allTraits.Length; i++)
        {
            var isSelected = _selectedTraits.Contains(i);
            var isCursor   = i == _traitCursor && _step == Step.Traits;
            var prefix     = isCursor ? "> " : "  ";
            var mark       = isSelected ? "[X] " : "[ ] ";
            var color      = isCursor ? Color.LightGreen : isSelected ? Color.Yellow : Color.Gray;
            spriteBatch.DrawString(Font, prefix + mark + _allTraits[i], new Vector2(x + 20, y), color);
            y += 22;
        }

        y += 30;
        spriteBatch.DrawString(Font, "Press Enter to confirm, Esc to go back",
            new Vector2(x, y), Color.DarkGray);
    }

    // ---- INPUT HELPERS ----------------------------------------------------
    private static char? KeyToChar(Keys key, KeyboardState state)
    {
        bool shift = state.IsKeyDown(Keys.LeftShift) || state.IsKeyDown(Keys.RightShift);
        if (key >= Keys.A && key <= Keys.Z)
            return shift ? (char)('A' + (key - Keys.A)) : (char)('a' + (key - Keys.A));
        if (key >= Keys.D0 && key <= Keys.D9)
        {
            if (shift)
            {
                var shifted = ")!@#$%^&*(";
                return shifted[key - Keys.D0];
            }
            return (char)('0' + (key - Keys.D0));
        }
        if (key == Keys.Space) return ' ';
        if (key == Keys.OemPeriod) return '.';
        if (key == Keys.OemMinus) return shift ? '_' : '-';
        return null;
    }
}