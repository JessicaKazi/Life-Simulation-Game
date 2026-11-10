using System.IO;
using System.Text.Json;
using LifeGame.Entities;

namespace LifeGame.Core;

public static class SaveSystem
{
    private static readonly JsonSerializerOptions Opts = new() { WriteIndented = true };

    public static void Save(string slot, Character player, Systems.TimeSystem time)
    {
        var dir = Path.Combine("Saves");
        Directory.CreateDirectory(dir);
        var data = new { Player = player, Time = new { time.Year, time.Day, time.Hour } };
        File.WriteAllText(Path.Combine(dir, $"{slot}.json"), JsonSerializer.Serialize(data, Opts));
    }
}