using Discord;
using Discord.Commands;
using Hifumi.Addons;
using Hifumi.Addons.Preconditions;
using Hifumi.Enums;
using Hifumi.Helpers;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;
using static Hifumi.Addons.Embeds;

namespace Hifumi.Modules
{
    [Name("nsfwmodule"), RequireNSFW, RequireCooldown(15), RequireBotPermission(ChannelPermission.SendMessages)]
    public class NsfwModule : Base
    {
        [Command("cureninja"), Summary("Searches Cureninja for tags.")]
        public async Task CureninjaAsync([Remainder] string tags) => await ReplyAsync(await StringHelper.HentaiAsync(Context.HttpClient, Context.Random, NsfwType.Cureninja, tags));

        [Command("danbooru"), Summary("Searches Danbooru for tags.")]
        public async Task DanbooruAsync([Remainder] string tags) => await ReplyAsync(await StringHelper.HentaiAsync(Context.HttpClient, Context.Random, NsfwType.Danbooru, tags));

        [Command("ero"), Summary("More softcore than lewd.")]
        public async Task EroAsync(string option = null)
        {
            string url = "https://nekos.life/api/v2/img/";
            switch (option)
            {
                case "--holo":
                    url += "holoero";
                    break;
                case "--kemo":
                    url += "erokemo";
                    break;
                case "--yuri":
                    url += "eroyuri";
                    break;
                case "--neko":
                    url += "eron";
                    break;
                case "--feet":
                    url += "erofeet";
                    break;
                case "--kitsune":
                    url += "erok";
                    break;
                default:
                    url += "ero";
                    break;
            }
            var embed = GetEmbed(Paint.Temporary)
                .WithImageUrl(JToken.Parse(await Context.HttpClient.GetStringAsync(url).ConfigureAwait(false))["url"].ToString())
                .WithFooter("Powered by: nekos.life")
                .Build();
            await ReplyAsync(string.Empty, embed);
        }

        [Command("gelbooru"), Summary("Searches Gelbooru for tags.")]
        public async Task GelbooruAsync([Remainder] string tags) => await ReplyAsync(await StringHelper.HentaiAsync(Context.HttpClient, Context.Random, NsfwType.Gelbooru, tags));

        [Command("konachan"), Summary("Searches Konachan for tags.")]
        public async Task KonaChanAsync([Remainder] string tags) => await ReplyAsync(await StringHelper.HentaiAsync(Context.HttpClient, Context.Random, NsfwType.Konachan, tags));


        [Command("lewd"), Summary("Oh my, how lewd!")]
        public async Task LewdAsync([Remainder] string option = null)
        {
            string url = "https://nekos.life/api/v2/img/";
            switch (option)
            {
                case "--feet":
                    url += "feet";
                    break;
                case "--yuri":
                    url += "yuri";
                    break;
                case "--trap":
                    url += "trap";
                    break;
                case "--futa":
                    url += "futanari";
                    break;
                case "--holo":
                    url += "hololewd";
                    break;
                case "--kemo":
                    url += "lewdkemo";
                    break;
                case "--solo --gif":
                case "--gif --solo":
                    url += "solog";
                    break;
                case "--feet --gif":
                case "--gif --feet":
                    url += "feetg";
                    break;
                case "--cum --gif":
                case "--gif --cum":
                    url += "cum";
                    break;
                case "--kitsune":
                    url += "lewdk";
                    break;
                case "--cum":
                    url += "cum_jpg";
                    break;
                case "--bj --gif":
                case "--gif --bj":
                    url += "bj";
                    break;
                case "--solo":
                    url += "solo";
                    break;
                case "--anal":
                    url += "anal";
                    break;
                case "--hentai":
                    url += "hentai";
                    break;
                case "--keta":
                    url += "keta";
                    break;
                case "--bj":
                    url += "blowjob";
                    break;
                case "--pussy":
                    url += "pussy_jpg";
                    break;
                case "--pussy --gif":
                case "--gif --pussy":
                    url += "pussy";
                    break;
                case "--busty":
                    url += "tits";
                    break;
                case "--busty --gif":
                case "--gif --busty":
                    url += "boobs";
                    break;
                case "--flat":
                    url += "smallboobs";
                    break;
                case "--kuni":
                    url += "kuni";
                    break;
                case "--femdom":
                    url += "femdom";
                    break;
                case "--hentai --gif":
                case "--gif --hentai":
                    url += "Random_hentai_gif";
                    break;
                default:
                    url += "lewd";
                    break;
            }
            var embed = GetEmbed(Paint.Temporary)
                .WithImageUrl(JToken.Parse(await Context.HttpClient.GetStringAsync(url).ConfigureAwait(false))["url"].ToString())
                .WithFooter("Powered by: nekos.life")
                .Build();
            await ReplyAsync(string.Empty, embed);
        }

        [Command("rule34"), Summary("Searches Rule34 for tags.")]
        public async Task Rule34Async([Remainder] string tags) => await ReplyAsync(await StringHelper.HentaiAsync(Context.HttpClient, Context.Random, NsfwType.Rule34, tags));

        [Command("yandere"), Summary("Searches Yandere for tags.")]
        public async Task YandereAsync([Remainder] string tags) => await ReplyAsync(await StringHelper.HentaiAsync(Context.HttpClient, Context.Random, NsfwType.Yandere, tags));
    }
}
