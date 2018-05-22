using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Hifumi.Addons;
using Hifumi.Addons.Preconditions;
using Hifumi.Personality;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using static Hifumi.Addons.Embeds;
using static Hifumi.Personality.Emotes;

namespace Hifumi.Modules
{
    [Name("imagemodule"),RequireCooldown(10) , RequireBotPermission(ChannelPermission.SendMessages)]
    public class ImageModule : Base
    {
        [Command("cuddle")]
        public async Task CuddleAsync(SocketGuildUser user = null)
        {
            string description;
            if (user == null || user == Context.User as SocketGuildUser)
                description = $"Would you like to snuggle together, {Context.User.Mention}? {GetEmote(EmoteType.Love)}";
            else
                description = $"{Context.User.Mention} cuddles with {user.Mention} {GetEmote(EmoteType.Love)}";

            var embed = GetEmbed(Paint.Aqua)
                .WithDescription(description)
                .WithImageUrl(JToken.Parse(await Context.HttpClient.GetStringAsync("https://nekos.life/api/v2/img/cuddle").ConfigureAwait(false))["url"].ToString())
                .WithFooter("Powered by: nekos.life")
                .Build();
            await ReplyAsync(string.Empty, embed);
        }

        [Command("feed")]
        public async Task FeedAsync(SocketGuildUser user = null)
        {
            string description;
            if (user == null || user == Context.User as SocketGuildUser)
                description = $"Little hungry there, {Context.User.Mention}? {GetEmote(EmoteType.Love)}";
            else
                description = $"{Context.User.Mention} gives food to {user.Mention} {GetEmote(EmoteType.Love)}";

            var embed = GetEmbed(Paint.Aqua)
                .WithDescription(description)
                .WithImageUrl(JToken.Parse(await Context.HttpClient.GetStringAsync("https://nekos.life/api/v2/img/feed").ConfigureAwait(false))["url"].ToString())
                .WithFooter("Powered by: nekos.life")
                .Build();
            await ReplyAsync(string.Empty, embed);
        }

        [Command("hug")]
        public async Task HugAsync(SocketGuildUser user = null)
        {
            string description;
            if (user == null || user == Context.User as SocketGuildUser)
                description = $"We all need hugs {Context.User.Mention} {GetEmote(EmoteType.Love)}";
            else
                description = $"{Context.User.Mention} hugs {user.Mention} {GetEmote(EmoteType.Love)}";

            var embed = GetEmbed(Paint.Aqua)
                .WithDescription(description)
                .WithImageUrl(JToken.Parse(await Context.HttpClient.GetStringAsync("https://nekos.life/api/v2/img/hug").ConfigureAwait(false))["url"].ToString())
                .WithFooter("Powered by: nekos.life")
                .Build();
            await ReplyAsync(string.Empty, embed);
        }

        [Command("kiss")]
        public async Task KissAsync(SocketGuildUser user = null)
        {
            string description;
            if (user == null || user == Context.User as SocketGuildUser)
                description = $"I-it's not like I wanted to kiss you, {Context.User.Mention} ...";
            else
                description = $"{Context.User.Mention} kisses {user.Mention} {GetEmote(EmoteType.Love)}";

            var embed = GetEmbed(Paint.Aqua)
                .WithDescription(description)
                .WithImageUrl(JToken.Parse(await Context.HttpClient.GetStringAsync("https://nekos.life/api/v2/img/kiss").ConfigureAwait(false))["url"].ToString())
                .WithFooter("Powered by: nekos.life")
                .Build();
            await ReplyAsync(string.Empty, embed);
        }

        [Command("pat")]
        public async Task PatAsync(SocketGuildUser user = null)
        {
            string description;
            if (user == null || user == Context.User as SocketGuildUser)
                description = $"Giving {Context.User.Mention} a pat on the head {GetEmote(EmoteType.Love)}";
            else
                description = $"{Context.User.Mention} pats {user.Mention} {GetEmote(EmoteType.Love)}";

            var embed = GetEmbed(Paint.Aqua)
                .WithDescription(description)
                .WithImageUrl(JToken.Parse(await Context.HttpClient.GetStringAsync("https://nekos.life/api/v2/img/pat").ConfigureAwait(false))["url"].ToString())
                .WithFooter("Powered by: nekos.life")
                .Build();
            await ReplyAsync(string.Empty, embed);
        }

        [Command("poke")]
        public async Task PokeAsync(SocketGuildUser user = null)
        {
            string description;
            if (user == null || user == Context.User as SocketGuildUser)
                description = $@"\*poke\* \*poke\* {Context.User.Mention}, you still there? {GetEmote(EmoteType.Worried)}";
            else
                description = $"{Context.User.Mention} pokes {user.Mention}";

            var embed = GetEmbed(Paint.Aqua)
                .WithDescription(description)
                .WithImageUrl(JToken.Parse(await Context.HttpClient.GetStringAsync("https://nekos.life/api/v2/img/poke").ConfigureAwait(false))["url"].ToString())
                .WithFooter("Powered by: nekos.life")
                .Build();
            await ReplyAsync(string.Empty, embed);
        }

        [Command("slap")]
        public async Task SlapAsync(SocketGuildUser user = null)
        {
            string description;
            if (user == null || user == Context.User as SocketGuildUser)
                description = $"Why must you hit yourself, {Context.User.Mention}? {GetEmote(EmoteType.Worried)}";
            else
                description = $"{Context.User.Mention} is hitting {user.Mention} {GetEmote(EmoteType.Worried)}";

            var embed = GetEmbed(Paint.Aqua)
                .WithDescription(description)
                .WithImageUrl(JToken.Parse(await Context.HttpClient.GetStringAsync("https://nekos.life/api/v2/img/slap").ConfigureAwait(false))["url"].ToString())
                .WithFooter("Powered by: nekos.life")
                .Build();
            await ReplyAsync(string.Empty, embed);
        }

        [Command("tickle")]
        public async Task TickleAsync(SocketGuildUser user = null)
        {
            string description;
            if (user == null || user == Context.User as SocketGuildUser)
                description = $"Are you ticklish, {Context.User.Mention}? {GetEmote(EmoteType.Happy)}";
            else
                description = $"{Context.User.Mention} tickles {user.Mention} {GetEmote(EmoteType.Happy)}";

            var embed = GetEmbed(Paint.Aqua)
                .WithDescription(description)
                .WithImageUrl(JToken.Parse(await Context.HttpClient.GetStringAsync("https://nekos.life/api/v2/img/tickle").ConfigureAwait(false))["url"].ToString())
                .WithFooter("Powered by: nekos.life")
                .Build();
            await ReplyAsync(string.Empty, embed);
        }
    }
}
