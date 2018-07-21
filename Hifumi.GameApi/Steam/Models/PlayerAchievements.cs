using Newtonsoft.Json;
using System.Collections.Generic;

namespace Hifumi.GameApi.Steam.Models
{
    public class Achievement
    {
        [JsonProperty("apiname")]
        public string APIName { get; set; }
        [JsonProperty("achieved")]
        public int Achieved { get; set; }
        [JsonProperty("unlocktime")]
        public int UnlockTIme { get; set; }
    }

    public class Playerstats
    {
        [JsonProperty("steamID")]
        public string SteamId { get; set; }
        [JsonProperty("gameName")]
        public string GameName { get; set; }
        [JsonProperty("achievements")]
        public List<Achievement> Achievements { get; set; }
        [JsonProperty("error")]
        public string Error { get; set; }
        [JsonProperty("success")]
        public bool IsSuccess { get; set; }
    }

    public class PlayerAchievements
    {
        [JsonProperty("playerstats")]
        public Playerstats PlayerStats { get; set; }
    }
}