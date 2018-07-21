using Newtonsoft.Json;
using System.Collections.Generic;

namespace Hifumi.GameApi.Steam.Models
{
    public class GamesList
    {
        [JsonProperty("appid")]
        public int AppId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("playtime_2weeks")]
        public int Playtime2Weeks { get; set; }
        [JsonProperty("playtime_forever")]
        public int TotalPlaytime { get; set; }
        [JsonProperty("img_icon_url")]
        public string IconUrl { get; set; }
        [JsonProperty("img_logo_url")]
        public string LogoUrl { get; set; }
    }

    public class RecentGames
    {
        [JsonProperty("total_count")]
        public int TotalCount { get; set; }
        [JsonProperty("games")]
        public List<GamesList> GamesList { get; set; }
    }

    public class PlayerRecentGames
    {
        [JsonProperty("response")]
        public RecentGames RecentGames { get; set; }
    }
}