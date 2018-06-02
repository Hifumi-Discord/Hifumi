using Discord.WebSocket;
using Hifumi.Addons;
using static Hifumi.Addons.Embeds;
using Hifumi.Handlers;
using Hifumi.Models;
using Newtonsoft.Json;
using Raven.Client.Documents;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Hifumi.Services
{
    public class RedditService
    {
        HttpClient HttpClient { get; }
        IDocumentStore Store { get; }
        GuildHandler GuildHandler { get; }
        ConfigHandler ConfigHandler { get; }
        DiscordSocketClient Client { get; }
        Timer AutoFeedTimer { get; set; }
        ConcurrentDictionary<ulong, Timer> ChannelTimers { get; set; } = new ConcurrentDictionary<ulong, Timer>();
        ConcurrentDictionary<ulong, List<string>> PostTrack { get; set; } = new ConcurrentDictionary<ulong, List<string>>();

        public RedditService(HttpClient httpClient, GuildHandler guildHandler, DiscordSocketClient client, IDocumentStore store, ConfigHandler configHandler)
        {
            Client = client;
            Store = store;
            GuildHandler = guildHandler;
            HttpClient = httpClient;
            ConfigHandler = configHandler;
        }

        public void Initialize()
        {
            AutoFeedTimer = new Timer(_ =>
            {
                foreach (var server in Store.OpenSession().Query<GuildModel>().Customize(x => x.WaitForNonStaleResults())
                    .Where(x => x.Reddit.IsEnabled && x.Reddit.Subreddits.Any() && x.Reddit.TextChannel != 0))
                    Start(Convert.ToUInt64(server.Id));
            }, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
        }

        public Task Start(ulong guildId)
        {
            var server = GuildHandler.GetGuild(guildId);
            if (!server.Reddit.Subreddits.Any()) return Task.CompletedTask;
            if (ChannelTimers.ContainsKey(server.Reddit.TextChannel)) return Task.CompletedTask;
            ChannelTimers.TryAdd(server.Reddit.TextChannel, new Timer(async _ =>
            {
                foreach (var subreddit in server.Reddit.Subreddits)
                {
                    var postIds = new List<string>();
                    var checkSub = await SubredditAsync(subreddit).ConfigureAwait(false);
                    if (checkSub == null) return;
                    var subData = checkSub.Data.Children[0].ChildData;
                    if (PostTrack.ContainsKey(server.Reddit.TextChannel)) PostTrack.TryGetValue(server.Reddit.TextChannel, out postIds);
                    if (postIds.Contains(subData.Id)) return;
                    string description = subData.Selftext.Length > 500 ? $"{subData.Selftext.Substring(0, 400)} ..." : subData.Selftext;
                    string title = subData.Title.Length > 40 ? $"{subData.Title.Substring(0, 35)} ..." : subData.Title;
                    string image = await GetImgurLinkAsync(subData.Url).ConfigureAwait(false);
                    var embed = GetEmbed(Paint.Temporary)
                        .WithAuthor($"New Post in **r/{subData.Subreddit} by **{subData.Author}**", "https://png.icons8.com/dusk/100/000000/reddit.png", subData.Url)
                        .WithTitle(title)
                        .WithDescription(description ?? null)
                        .WithImageUrl(image)
                        .Build();

                    await ((Client as DiscordSocketClient).GetChannel(server.Reddit.TextChannel) as SocketTextChannel).SendMessageAsync(string.Empty, embed: embed);
                    postIds.Add(subData.Id);
                    PostTrack.TryRemove(server.Reddit.TextChannel, out List<string> useless);
                    PostTrack.TryAdd(server.Reddit.TextChannel, postIds);
                }
            }, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30)));
            return Task.CompletedTask;
        }

        public async Task<RedditModel> SubredditAsync(string subreddit)
        {
            var sub = await HttpClient.GetAsync($"https://reddit.com/r/{subreddit}/new.json?limit=5").ConfigureAwait(false);
            if (!sub.IsSuccessStatusCode) return null;
            return JsonConvert.DeserializeObject<RedditModel>(await sub.Content.ReadAsStringAsync().ConfigureAwait(false));
        }

        // TODO: Get all types of images, and youtube thumbnails
        async Task<string> GetImgurLinkAsync(string link)
        {
            if (link.Contains("https://www.reddit.com/")) return null;
            if (!link.Contains("https://imgur.com/")) return link;
            var getLink = link.Split('/', StringSplitOptions.None);
            bool isAlbum = false;
            string url = null;
            if (link.Contains("/a/"))
            {
                url = $"https://api.imgur.com/3/album/{getLink[4]}?client_id={ConfigHandler.Config.APIKeys["Imgur"]}";
                isAlbum = true;
            }
            else
            {
                url = $"https://api.imgur.com/3/image/{getLink[3]}?client_id={ConfigHandler.Config.APIKeys["Imgur"]}";
                isAlbum = false;
            }
            try
            {
                var image = await HttpClient.GetStringAsync(url).ConfigureAwait(false);
                var convert = JsonConvert.DeserializeObject<ImgurModel>(image);
                if (!convert.Success || convert.Status == 401) return link;
                return isAlbum ? convert.Data.Images[0].Link : convert.Data.Link;
            }
            catch
            {
                return link;
            }
        }

        public void Stop(ulong channelId)
        {
            if (!ChannelTimers.ContainsKey(channelId)) return;
            var getTimer = ChannelTimers[channelId];
            getTimer.Change(Timeout.Infinite, Timeout.Infinite);
            getTimer.Dispose();
            ChannelTimers.TryRemove(channelId, out Timer useless);
            useless.Change(Timeout.Infinite, Timeout.Infinite);
            useless.Dispose();
        }
    }
}
