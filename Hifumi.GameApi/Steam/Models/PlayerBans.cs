using Newtonsoft.Json;
using System.Collections.Generic;

namespace Hifumi.GameApi.Steam.Models
{
    public class User
    {
        public string SteamId { get; set; }
        public bool CommunityBanned { get; set; }
        public bool VACBanned { get; set; }
        public int NumberOfVACBans { get; set; }
        public int DaysSinceLastBan { get; set; }
        public int NumberOfGameBans { get; set; }
        public string EconomyBan { get; set; }
    }

    public class PlayerBans
    {
        [JsonProperty("players")]
        public List<User> PlayersList { get; set; }
    }
}