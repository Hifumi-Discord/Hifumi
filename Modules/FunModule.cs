using Discord;
using Discord.Commands;
using Hifumi.Addons;
using Hifumi.Addons.Preconditions;
using Hifumi.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Hifumi.Addons.Embeds;

namespace Hifumi.Modules
{
    [Name("funmodule"), RequireBotPermission(ChannelPermission.SendMessages)]
    public class FunModule : Base
    {
        [Command("clap"), Summary("Replaces spaces in your message with the clap emoji.")]
        public Task Clap([Remainder] string message) => ReplyAsync($"👏 {message.Replace(" ", " 👏 ")} 👏");

        [Command("clapd"), Summary("Deletes your message and replaces spaces in your message with the clap emoji.")]
        public async Task ClapDeleteAsync([Remainder] string message)
        {
            await Context.Message.DeleteAsync();
            await ReplyAsync($"👏 {message.Replace(" ", " 👏 ")} 👏");
        }

        [Command("expand"), Summary("Converts text to full width.")]
        public Task Expand([Remainder] string message)
            => ReplyAsync(string.Join("", message.Select(x => StringHelper.Normal.Contains(x) ? x : ' ').Select(x => StringHelper.FullWidth[StringHelper.Normal.IndexOf(x)])));

        [Command("expandd"), Summary("Deletes your message and converts the text to full width.")]
        public async Task ExpandAsync([Remainder] string message)
        {
            await Context.Message.DeleteAsync();
            await ReplyAsync(string.Join("", message.Select(x => StringHelper.Normal.Contains(x) ? x : ' ').Select(x => StringHelper.FullWidth[StringHelper.Normal.IndexOf(x)])));
        }

        [Command("fox"), Summary("Get an image of a random mischievous fox."), RequireCooldown(10)]
        public async Task FoxAsync()
        {
            var fox = JToken.Parse(await Context.HttpClient.GetStringAsync("https://randomfox.ca/floof/"));
            var embed = GetEmbed(Paint.Aqua)
                .WithAuthor("Here's a random fox!", url: fox["link"].ToString())
                .WithImageUrl(fox["image"].ToString())
                .WithFooter("Powered by: randomfox.ca")
                .Build();
            await ReplyAsync(string.Empty, embed);
        }

        [Command("neko"), Summary("The best command out there. Option: --[gif/holo/kemono/kitsune]"), RequireCooldown(10)]
        public async Task NekoAsync(string option = null)
        {
            string url = "https://nekos.life/api/v2/img/";
            switch (option)
            {
                case "--gif":
                    url += "ngif";
                    break;
                case "--kemono":
                    url += "kemonomimi";
                    break;
                case "--holo":
                    url += "holo";
                    break;
                case "--kitsune":
                    url += "fox_girl";
                    break;
                default:
                    url += "neko";
                    break;
            }
            var embed = GetEmbed(Paint.Aqua)
                .WithImageUrl(JToken.Parse(await Context.HttpClient.GetStringAsync(url).ConfigureAwait(false))["url"].ToString())
                .WithFooter("Powered by: nekos.life")
                .Build();
            await ReplyAsync(string.Empty, embed);
        }

        [Command("owo"), RequireCooldown(10)]
        public async Task OwoAsync([Remainder] string message = null)
        {
            if (message == null)
            {
                await Context.Channel.SendFileAsync("./resources/owo.png");
                return;
            }
            var data = JToken.Parse(await Context.HttpClient.GetStringAsync($"https://nekos.life/api/v2/owoify?text={Uri.EscapeDataString(message)}").ConfigureAwait(false))["owo"];
            await ReplyAsync(data.ToString());
        }

        [Command("rip"), Summary("Mark the death of a user.")]
        public async Task RipAsync(IGuildUser user)
        {
            bool ayana = (await Context.Guild.GetUserAsync(185476724627210241) != null || await Context.Guild.GetUserAsync(349764485348589568) != null);
            string path = await ImageHelper.GraveAsync(user, Context.HttpClient, ayana);
            await Context.Channel.SendFileAsync(path);
        }

        // TODO: more commands
    }
}
