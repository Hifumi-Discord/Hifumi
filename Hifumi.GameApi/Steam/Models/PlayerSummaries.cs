using Newtonsoft.Json;
using System.Collections.Generic;

namespace Hifumi.GameApi.Steam.Models
{
    public class Player
    {
        [JsonProperty("steamid")]
        public string SteamId { get; set; }
        [JsonProperty("communityvisibilitystate")]
        public int VisibilityState { get; set; }
        [JsonProperty("profilestate")]
        public int ProfileState { get; set; }
        [JsonProperty("personaname")]
        public string Name { get; set; }
        [JsonProperty("lastlogoff")]
        public int LastLogOff { get; set; }
        [JsonProperty("profileurl")]
        public string ProfileLink { get; set; }
        [JsonProperty("avatar")]
        public string AvatarUrl { get; set; }
        [JsonProperty("avatarmedium")]
        public string AvatarMediumUrl { get; set; }
        [JsonProperty("avatarfull")]
        public string AvatarFullUrl { get; set; }
        [JsonProperty("personastate")]
        public int UserState { get; set; }
        [JsonProperty("realname")]
        public string RealName { get; set; }
        [JsonProperty("primaryclanid")]
        public string PrimaryClanId { get; set; }
        [JsonProperty("timecreated")]
        public int TimeCreated { get; set; }
        [JsonProperty("personastateflags")]
        public int Flags { get; set; }
        [JsonProperty("loccountrycode")]
        public string Country { get; set; }
        [JsonProperty("locstatecode")]
        public string State { get; set; }
        [JsonProperty("loccityid")]
        public int CityId { get; set; }
    }

    public class PlayersInfo
    {
        [JsonProperty("players")]
        public List<Player> Players { get; set; }
    }

    public class PlayerSummaries
    {
        [JsonProperty("response")]
        public PlayersInfo PlayersInfo { get; set; }
    }
}