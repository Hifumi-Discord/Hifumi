using Hifumi.GameApi.Helpers;
using Hifumi.GameApi.Steam.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hifumi.GameApi.Steam
{
    public class SteamClient
    {
        readonly string key;
        public SteamClient(string apiKey) => key = apiKey;

        public async Task<PlayerSummaries> GetUsersInfoAsync(List<string> userIds)
            => JsonConvert.DeserializeObject<PlayerSummaries>(await HttpHelper.GetContentAsync(
                $"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0001/?key={key}&steamids={string.Join(",", userIds)}"
            ).ConfigureAwait(false));

        public async Task<PlayerFriendList> GetFriendListAsync(string userId)
            => JsonConvert.DeserializeObject<PlayerFriendList>(await HttpHelper.GetContentAsync(
                $"http://api.steampowered.com/ISteamUser/GetFriendList/v0001/?key={key}&steamid={userId}"
            ).ConfigureAwait(false));

        public async Task<PlayerOwnedGames> OwnedGamesAsync(string userId)
            => JsonConvert.DeserializeObject<PlayerOwnedGames>(await HttpHelper.GetContentAsync(
                $"http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key={key}&steamid={userId}"
            ).ConfigureAwait(false));

        public async Task<PlayerRecentGames> RecentGamesAsync(string userId)
            => JsonConvert.DeserializeObject<PlayerRecentGames>(await HttpHelper.GetContentAsync(
                $"http://api.steampowered.com/IPlayerService/GetRecentlyPlayedGames/v0001/?key={key}&steamid={userId}"
            ).ConfigureAwait(false));

        public async Task<PlayerBans> PlayerBansAsync(List<string> userIds)
            => JsonConvert.DeserializeObject<PlayerBans>(await HttpHelper.GetContentAsync(
                $"http://api.steampowered.com/ISteamUser/GetPlayerBans/v1/?key={key}&steamids={string.Join(",", userIds)}"
            ).ConfigureAwait(false));

        public async Task<SchemaForGames> GameInfoAsync(string appId)
            => JsonConvert.DeserializeObject<SchemaForGames>(await HttpHelper.GetContentAsync(
                $"http://api.steampowered.com/ISteamUserStats/GetSchemaForGame/v2/?key={key}&appid={appId}"
            ).ConfigureAwait(false));

        public async Task<PlayerAchievements> GetAchievementsAsync(string appId, string userId)
            => JsonConvert.DeserializeObject<PlayerAchievements>(await HttpHelper.GetContentAsync(
                $"http://api.steampowered.com/ISteamUserStats/GetPlayerAchievements/v0001/?appid={appId}&key={key}&steamid={userId}"
            ).ConfigureAwait(false));
    }
}
