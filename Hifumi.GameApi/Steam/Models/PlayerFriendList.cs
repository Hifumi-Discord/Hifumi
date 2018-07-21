using Newtonsoft.Json;
using System.Collections.Generic;

namespace Hifumi.GameApi.Steam.Models
{
    public class Friend
    {
        [JsonProperty("steamid")]
        public string SteamId { get; set; }
        [JsonProperty("relationship")]
        public string Relationship { get; set; }
        [JsonProperty("friend_since")]
        public int FriendSince { get; set; }
    }

    public class Friendslist
    {
        [JsonProperty("friends")]
        public List<Friend> Friends { get; set; }
    }

    public class PlayerFriendList
    {
        [JsonProperty("friendslist")]
        public Friendslist FriendsList { get; set; }
    }
}