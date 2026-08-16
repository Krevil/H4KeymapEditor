using System.IO;
using System.Text.Json;

namespace H4KeymapEditor.Models
{
    public class Settings
    {
        private static string FilePath = Path.Combine(AppContext.BaseDirectory, "settings.json");

        public bool UseDarkMode { get; set; }
        public bool ShowUnknowns { get; set; }

        public void Save()
        {
            string json = JsonSerializer.Serialize(this);
            File.WriteAllText(FilePath, json);
        }

        public static Settings Load()
        {
            if (!File.Exists(FilePath))
                return new Settings();

            string json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
        }
    }
}
