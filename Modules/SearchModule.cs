using Discord;
using Discord.Commands;
using Hifumi.Addons;
using static Hifumi.Addons.Embeds;
using Hifumi.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hifumi.Modules
{
    public class SearchModule : Base
    {
        [Command("dotnet"), Summary("Search microsoft docs for classes, methods, and namespaces.")]
        public async Task DotNetAsync([Remainder] string search)
        {
            var response = await Context.HttpClient.GetAsync($"https://docsapibrowser.azurewebsites.net/api/apibrowser/dotnet/search?search={search}&top=10");
            if (!response.IsSuccessStatusCode) return;
            var convertedJson = JsonConvert.DeserializeObject<DotnetModel>(await response.Content.ReadAsStringAsync());
            if (!convertedJson.Results.Any())
            {
                await ReplyAsync($"Couldn't find anything for {search}.");
                return;
            }
            var embed = GetEmbed(Paint.Aqua)
                .WithAuthor($"Displaying results for {search}", "https://avatars2.githubusercontent.com/u/9141961?s=200&v=4");
            foreach (var result in convertedJson.Results.Take(5).OrderBy(x => x.Name))
                embed.AddField(result.Name, $"Kind: {result.Kind} | Type: {result.Type}\n[{result.Snippet}]({result.URL})");
            await ReplyAsync(string.Empty, embed.Build());
        }

        [Command("lmgtfy"), Summary("Help someone search for something who is unable to.")]
        public Task Lmgtfy([Remainder] string search = "How to use Lmgtfy") => ReplyAsync($"http://lmgtfy.com/?q={Uri.EscapeDataString(search)}");

        // TODO: More searches
    }
}
