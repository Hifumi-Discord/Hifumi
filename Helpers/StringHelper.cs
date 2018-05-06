using Discord;
using Discord.WebSocket;
using Hifumi.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hifumi.Helpers
{
    public class StringHelper
    {
        public static string CacheFolder
        {
            get
            {
                if (!Directory.Exists($"{Path.Combine(Directory.GetCurrentDirectory(), "cache")}"))
                    Directory.CreateDirectory($"{Path.Combine(Directory.GetCurrentDirectory(), "cache")}");
                return $"{Path.Combine(Directory.GetCurrentDirectory(), "cache")}";
            }
        }
        public static string Normal = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!#$%&()*+,-./:;<=>?@[\\]^_`{|}~ ";
        public static string FullWidth = "０１２３４５６７８９ａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯ" +
            "ＰＱＲＳＴＵＶＷＸＹＺ！＃＄％＆（）＊＋、ー。／：；〈＝〉？＠［\\］＾＿‘｛｜｝～ ";

        public static string CheckUser(IDiscordClient client, ulong userId)
        {
            var socketClient = client as DiscordSocketClient;
            var user = socketClient.GetUser(userId);
            return user == null ? "Unknown User." : user.Username;
        }

        public static string CheckRole(SocketGuild guild, ulong id)
        {
            var role = guild.GetRole(id);
            return role == null ? "Unknown Role." : role.Name;
        }

        public static string CheckChannel(SocketGuild guild, ulong id)
        {
            var channel = guild.GetTextChannel(id);
            return channel == null ? "Unknown Channel." : channel.Name;
        }

        public static string Star(int stars)
        {
            if (stars <= 5 && stars > 0) return "⭐";
            else if (stars > 5 && stars <= 15) return "🌟";
            else if (stars > 15 && stars <= 25) return "💫";
            else return "✨";
        }

        public static string Replace(string message, string guild = null, string user = null, int level = 0, int credits = 0)
        {
            StringBuilder builder = new StringBuilder(message);
            builder.Replace("{guild}", guild);
            builder.Replace("{user}", user);
            builder.Replace("{level}", $"{level}");
            builder.Replace("{credits}", $"{credits}");
            return builder.ToString();
        }

        public static async Task<string> NsfwAsync(HttpClient httpClient, Random random, string url, int max)
        {
            try
            {
                var parse = JArray.Parse(await httpClient.GetStringAsync($"{url}{random.Next(max)}").ConfigureAwait(false))[0];
                return ($"{parse["preview"]}");
            }
            catch { return null; }
        }

        public static async Task<string> HentaiAsync(HttpClient httpClient, Random random, NsfwType nsfwType, List<string> tags)
        {
            string url = null;
            string result = null;
            MatchCollection matches = null;
            tags = !tags.Any() ? new[] { "boobs", "tits", "ass", "sexy", "neko" }.ToList() : tags;
            switch (nsfwType)
            {
                case NsfwType.Danbooru:
                    url = $"http://danbooru.donmai.us/posts?page={random.Next(0, 15)}{string.Join("+", tags.Select(x => x.Replace(" ", "_")))}";
                    break;
                case NsfwType.Gelbooru:
                    url = $"http://gelbooru.com/index.php?page=dapi&s=post&q=index&limit=100&tags={string.Join("+", tags.Select(x => x.Replace(" ", "_")))}";
                    break;
                case NsfwType.Rule34:
                    url = $"http://rule34.xxx/index.php?page=dapi&s=post&q=index&limit=100&tags={string.Join("+", tags.Select(x => x.Replace(" ", "_")))}";
                    break;
                case NsfwType.Cureninja:
                    url = $"https://cure.ninja/booru/api/json?f=a&o=r&s=1&q={string.Join("+", tags.Select(x => x.Replace(" ", "_")))}";
                    break;
                case NsfwType.Konachan:
                    url = $"http://konachan.com/post?page={random.Next(0, 5)}&tags={string.Join("+", tags.Select(x => x.Replace(" ", "_")))}";
                    break;
                case NsfwType.Yandere:
                    url = $"https://yande.re/post.xml?limit=25&page={random.Next(0, 15)}&tags={string.Join("+", tags.Select(x => x.Replace(" ", "_")))}";
                    break;
            }
            var get = await httpClient.GetStringAsync(url).ConfigureAwait(false);
            switch (nsfwType)
            {
                case NsfwType.Danbooru:
                    matches = Regex.Matches(get, "data-large-file-url=\"(.*)\"");
                    break;
                case NsfwType.Yandere:
                case NsfwType.Gelbooru:
                case NsfwType.Rule34:
                    matches = Regex.Matches(get, "file_url=\"(.*?)\" ");
                    break;
                case NsfwType.Cureninja:
                    matches = Regex.Matches(get, "\"url\":\"(.*?)\"");
                    break;
                case NsfwType.Konachan:
                    matches = Regex.Matches(get, "<a class=\"directlink smallimg\" href=\"(.*?)\"");
                    break;
            }
            if (!matches.Any()) return "No results found.";
            switch (nsfwType)
            {
                case NsfwType.Danbooru:
                    result = $"http://danbooru.donmai.us/{matches[random.Next(matches.Count)].Groups[1].Value}";
                    break;
                case NsfwType.Konachan:
                case NsfwType.Gelbooru:
                    result = $"http:{matches[random.Next(matches.Count)].Groups[1].Value}";
                    break;
                case NsfwType.Yandere:
                case NsfwType.Rule34:
                    result = $"http:{matches[random.Next(matches.Count)].Groups[1].Value}";
                    break;
                case NsfwType.Cureninja:
                    result = matches[random.Next(matches.Count)].Groups[1].Value.Replace("\\/", "/");
                    break;
            }
            return result;
        }

        public static async Task<string> DownloadUserImageAsync(HttpClient httpClient, IUser user)
        {
            if (!File.Exists($"{CacheFolder}/{user.Id}.gif"))
            {
                var image = await httpClient.GetByteArrayAsync(user.GetAvatarUrl(ImageFormat.Gif, 2048)).ConfigureAwait(false);
                using (var userImage = File.Create($"{CacheFolder}/{user.Id}.gif"))
                    await userImage.WriteAsync(image, 0, image.Length).ConfigureAwait(false);
            }
            return $"{CacheFolder}/{user.Id}.gif";
        }
    }
}
