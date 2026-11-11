using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using LifeGame.Entities;
using LifeGame.Systems;

namespace LifeGame.Core;

public enum LoadResult { Success, NotFound, Corrupt, InvalidData }

public static class SaveSystem
{
    private static readonly string SaveDir =
        Path.Combine(AppContext.BaseDirectory, "Saves");

    private static readonly JsonSerializerOptions Opts = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

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
        public JobDto? Job { get; set; }
        public string? HomeAddress { get; set; }
        public int DaysSinceRentPaid { get; set; }
    }

    private class JobDto
    {
        public string CareerTitle { get; set; } = "";
        public CareerRank Rank { get; set; }
        public int ShiftsWorked { get; set; }
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

    public static bool Save(string slot, GameState state)
    {
        if (state.Player is null)
        {
            Console.Error.WriteLine($"[SaveSystem] Cannot save '{slot}': player is null.");
            return false;
        }

        try
        {
            Directory.CreateDirectory(SaveDir);

            var data = new SaveData
            {
                Player  = ToDto(state.Player),
                Time    = new TimeDto { Year = state.Time.Year, Day = state.Time.Day, Hour = state.Time.Hour },
                SavedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            var path    = Path.Combine(SaveDir, $"{slot}.json");
            var tmpPath = path + ".tmp";
            var bakPath = path + ".bak";

            File.WriteAllText(tmpPath, JsonSerializer.Serialize(data, Opts));

            if (File.Exists(path))
                File.Move(path, bakPath, overwrite: true);
            File.Move(tmpPath, path, overwrite: true);

            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[SaveSystem] Save failed for '{slot}': {ex.Message}");
            return false;
        }
    }

    public static LoadResult Load(string slot, GameState state)
    {
        var path    = Path.Combine(SaveDir, $"{slot}.json");
        var bakPath = path + ".bak";

        var readPath = File.Exists(path) ? path
                     : File.Exists(bakPath) ? bakPath
                     : null;

        if (readPath is null) return LoadResult.NotFound;

        try
        {
            var data = JsonSerializer.Deserialize<SaveData>(File.ReadAllText(readPath), Opts);
            if (data is null || string.IsNullOrWhiteSpace(data.Player.Name))
                return LoadResult.InvalidData;

            state.Player      = FromDto(data.Player);
            state.Time        = FromDto(data.Time);
            state.CurrentSlot = slot;
            state.CurrentScreen = Screen.InGame;
            return LoadResult.Success;
        }
        catch (JsonException ex)
        {
            Console.Error.WriteLine($"[SaveSystem] Corrupt save '{slot}': {ex.Message}");
            return LoadResult.Corrupt;
        }
        catch (IOException ex)
        {
            Console.Error.WriteLine($"[SaveSystem] IO error loading '{slot}': {ex.Message}");
            return LoadResult.Corrupt;
        }
    }

    public static List<string> ListSlots()
    {
        if (!Directory.Exists(SaveDir)) return new();
        return Directory.GetFiles(SaveDir, "*.json")
            .Select(Path.GetFileNameWithoutExtension)
            .Where(n => n is not null)
            .Cast<string>()
            .OrderByDescending(n => n, StringComparer.Ordinal)
            .ToList();
    }

    public static bool Delete(string slot)
    {
        var path    = Path.Combine(SaveDir, $"{slot}.json");
        var bakPath = path + ".bak";
        var deleted = false;

        try
        {
            if (File.Exists(path))    { File.Delete(path);    deleted = true; }
            if (File.Exists(bakPath)) { File.Delete(bakPath); deleted = true; }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[SaveSystem] Delete failed for '{slot}': {ex.Message}");
            return false;
        }

        return deleted;
    }

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
        },
        Job = c.Job is null ? null : new JobDto
        {
            CareerTitle  = c.Job.Career.Title,
            Rank         = c.Job.Rank,
            ShiftsWorked = c.Job.ShiftsWorked
        },
        HomeAddress       = c.Home?.Address,
        DaysSinceRentPaid = c.DaysSinceRentPaid
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

        c.Needs.Hunger  = Clamp100(d.Needs.Hunger);
        c.Needs.Energy  = Clamp100(d.Needs.Energy);
        c.Needs.Hygiene = Clamp100(d.Needs.Hygiene);
        c.Needs.Social  = Clamp100(d.Needs.Social);
        c.Needs.Fun     = Clamp100(d.Needs.Fun);

        if (d.Job is not null)
        {
            var career = Career.Catalog.FirstOrDefault(x => x.Title == d.Job.CareerTitle);
            if (career is not null)
                c.Job = new Job(career, d.Job.Rank, d.Job.ShiftsWorked);
        }

        if (!string.IsNullOrEmpty(d.HomeAddress))
        {
            var home = Home.Listings.FirstOrDefault(x => x.Address == d.HomeAddress);
            if (home is not null)
                c.Home = home;
        }

        c.DaysSinceRentPaid = d.DaysSinceRentPaid;

        return c;
    }

    private static TimeSystem FromDto(TimeDto d)
    {
        var t = new TimeSystem();
        t.SetTime(d.Year, d.Day, d.Hour);
        return t;
    }

    private static float Clamp100(float v) => v < 0f ? 0f : v > 100f ? 100f : v;
}