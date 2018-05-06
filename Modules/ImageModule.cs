using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Hifumi.Addons;
using static Hifumi.Addons.Embeds;
using Hifumi.Personality;
using static Hifumi.Personality.Emotes;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Hifumi.Modules
{
    [Name("Image Commands"), RequireBotPermission(ChannelPermission.SendMessages)]
    public class ImageModule : Base
    {
        // Endpoints
        // /api/v2/img/<'tickle', 'feed', 'kiss', 'hug'>
        [Command("cuddle")]
        public async Task CuddleAsync(SocketGuildUser user = null)
        {
            string description;
            if (user == null || user == Context.User as SocketGuildUser)
                description = $"Would you like to snuggle together, {Context.User.Mention}? {GetEmote(EmoteType.Love)}";
            else
                description = $"{Context.User.Mention} cuddles with {user.Mention}";

            var embed = GetEmbed(Paint.Aqua)
                .WithDescription(description)
                .WithImageUrl(JToken.Parse(await Context.HttpClient.GetStringAsync("https://nekos.life/api/v2/img/cuddle").ConfigureAwait(false))["url"].ToString())
                .WithFooter("Powered by: nekos.life")
                .Build();
            await ReplyAsync(string.Empty, embed);
        }

        [Command("slap")]
        public async Task SlapAsync(SocketGuildUser user = null)
        {
            string description;
            if (user == null || user == Context.User as SocketGuildUser)
                description = $"Why must you hit yourself, {Context.User.Mention}?";
            else
                description = $"{Context.User.Mention} is hitting {user.Mention}";

            var embed = GetEmbed(Paint.Aqua)
                .WithDescription(description)
                .WithImageUrl(JToken.Parse(await Context.HttpClient.GetStringAsync("https://nekos.life/api/v2/img/slap").ConfigureAwait(false))["url"].ToString())
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
                description = $"{Context.User.Mention} pats {user.Mention}";

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
    }
}
