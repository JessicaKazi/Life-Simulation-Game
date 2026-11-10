using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using LifeGame.Entities;
using LifeGame.Systems;

namespace LifeGame.Core;

public static class SaveSystem
{
    private static readonly string SaveDir =
        Path.Combine(AppContext.BaseDirectory, "Saves");

    private static readonly JsonSerializerOptions Opts = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    // -------- DTOs ----------------------------------------------------------
    private class SaveData
    {
        public PlayerDto Player { get; set; } = new();
        public TimeDto Time { get; set; } = new();
        public string SavedAt { get; set; } = "";
    }

    private class PlayerDto
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
        public string Gender { get; set; } = "";
        public List<Personality> Traits { get; set; } = new();
        public decimal Money { get; set; }
        public Dictionary<string, int> Skills { get; set; } = new();
        public NeedsDto Needs { get; set; } = new();
    }

    private class NeedsDto
    {
        public float Hunger { get; set; }
        public float Energy { get; set; }
        public float Hygiene { get; set; }
        public float Social { get; set; }
        public float Fun { get; set; }
    }

    private class TimeDto
    {
        public int Year { get; set; }
        public int Day { get; set; }
        public float Hour { get; set; }
    }

    // -------- Public API ----------------------------------------------------
    public static void Save(string slot, GameState state)
    {
        if (state.Player is null) return;

        Directory.CreateDirectory(SaveDir);

        var data = new SaveData
        {
            Player = ToDto(state.Player),
            Time   = new TimeDto { Year = state.Time.Year, Day = state.Time.Day, Hour = state.Time.Hour },
            SavedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
        };

        var path = Path.Combine(SaveDir, $"{slot}.json");
        File.WriteAllText(path, JsonSerializer.Serialize(data, Opts));
    }

    public static bool Load(string slot, GameState state)
    {
        var path = Path.Combine(SaveDir, $"{slot}.json");
        if (!File.Exists(path)) return false;

        try
        {
            var data = JsonSerializer.Deserialize<SaveData>(File.ReadAllText(path), Opts);
            if (data is null) return false;

            state.Player = FromDto(data.Player);
            state.Time   = FromDto(data.Time);
            state.CurrentScreen = Screen.InGame;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static List<string> ListSlots()
    {
        if (!Directory.Exists(SaveDir)) return new();
        return Directory.GetFiles(SaveDir, "*.json")
            .Select(Path.GetFileNameWithoutExtension)
            .Where(n => n is not null)
            .Cast<string>()
            .OrderBy(n => n)
            .ToList();
    }

    public static bool Delete(string slot)
    {
        var path = Path.Combine(SaveDir, $"{slot}.json");
        if (!File.Exists(path)) return false;
        File.Delete(path);
        return true;
    }

    // -------- DTO <-> Domain -----------------------------------------------
    private static PlayerDto ToDto(Character c) => new()
    {
        Name    = c.Name,
        Age     = c.Age,
        Gender  = c.Gender,
        Traits  = new List<Personality>(c.Traits),
        Money   = c.Money,
        Skills  = new Dictionary<string, int>(c.Skills),
        Needs   = new NeedsDto
        {
            Hunger  = c.Needs.Hunger,
            Energy  = c.Needs.Energy,
            Hygiene = c.Needs.Hygiene,
            Social  = c.Needs.Social,
            Fun     = c.Needs.Fun
        }
    };

    private static Character FromDto(PlayerDto d)
    {
        var c = new Character
        {
            Name   = d.Name,
            Age    = d.Age,
            Gender = d.Gender,
            Money  = d.Money
        };
        foreach (var t in d.Traits) c.Traits.Add(t);
        foreach (var kv in d.Skills) c.Skills[kv.Key] = kv.Value;

        c.Needs.Hunger  = d.Needs.Hunger;
        c.Needs.Energy  = d.Needs.Energy;
        c.Needs.Hygiene = d.Needs.Hygiene;
        c.Needs.Social  = d.Needs.Social;
        c.Needs.Fun     = d.Needs.Fun;
        return c;
    }

    private static TimeSystem FromDto(TimeDto d)
    {
        var t = new TimeSystem();
        t.SetTime(d.Year, d.Day, d.Hour);
        return t;
    }
}