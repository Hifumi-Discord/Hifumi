using Newtonsoft.Json;
using System.Collections.Generic;

namespace Hifumi.GameApi.Steam.Models
{
    public class Game
    {
        [JsonProperty("appid")]
        public int GamesId { get; set; }
        [JsonProperty("playtime_forever")]
        public int TotalPlaytime { get; set; }
        [JsonProperty("playtime_2weeks")]
        public int? Playtime2Weeks { get; set; }
    }

    public class OwnedGames
    {
        [JsonProperty("game_count")]
        public int GamesCount { get; set; }
        [JsonProperty("games")]
        public List<Game> GamesList { get; set; }
    }

    public class PlayerOwnedGames
    {
        [JsonProperty("response")]
        public OwnedGames OwnedGames { get; set; }
    }
}