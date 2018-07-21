using Newtonsoft.Json;
using System.Collections.Generic;

namespace Hifumi.GameApi.Steam.Models
{
    public class GameAchievement
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("defaultvalue")]
        public int DefaultValue { get; set; }
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        [JsonProperty("hidden")]
        public int Hidden { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }
        [JsonProperty("icongray")]
        public string GrayIcon { get; set; }
    }

    public class Stat
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("defaultvalue")]
        public int DefaultValue { get; set; }
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
    }

    public class AvailableGameStats
    {
        [JsonProperty("achievements")]
        public List<GameAchievement> Achievements { get; set; }
        [JsonProperty("stats")]
        public List<Stat> Stats { get; set; }
    }

    public class GameInfo
    {
        [JsonProperty("gameName")]
        public string Name { get; set; }
        [JsonProperty("gameVersion")]
        public string Version { get; set; }
        [JsonProperty("availableGameStats")]
        public AvailableGameStats GameStats { get; set; }
    }

    public class SchemaForGames
    {
        [JsonProperty("game")]
        public GameInfo GameInfo { get; set; }
    }
}