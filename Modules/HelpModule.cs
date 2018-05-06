using System;
using Discord;
using System.Linq;
using Hifumi.Addons;
using Discord.Commands;
using System.Threading.Tasks;
using static Hifumi.Addons.Embeds;

namespace Hifumi.Modules
{
    [Name("Assistance Commands"), RequireBotPermission(ChannelPermission.SendMessages)]
    public class HelpModule : Base
    {
        IServiceProvider Provider { get; }
        CommandService CommandService { get; }
        HelpModule(CommandService command, IServiceProvider provider)
        {
            CommandService = command;
            Provider = provider;
        }

        [Command("help"), Summary("Displays all of the commands.")]
        public Task Help()
        {
            var Embed = GetEmbed(Paint.Aqua)
                .WithAuthor("List of all commands", Context.Client.CurrentUser.GetAvatarUrl())
                .WithFooter($"For More Information On A Command's Usage: {Context.Config.Prefix}info <command>", Emotes.Hifumi.Url);
            foreach (var Commands in CommandService.Commands.Where(x => x.Module.Name != "Owner Commands").GroupBy(x => x.Module.Name).OrderBy(y => y.Key))
                Embed.AddField(Commands.Key, $"`{string.Join("`, `", Commands.Select(x => x.Name).Distinct())}`");
            return ReplyAsync(string.Empty, Embed.Build());
        }

        [Command("info"), Alias("help"), Summary("Shows more information about a command and it's usage.")]
        public Task CommandInfo([Remainder] string CommandName)
        {
            var Command = CommandService.Search(Context, CommandName).Commands?.FirstOrDefault().Command;
            if (Command == null) return ReplyAsync($"{CommandName} command doesn't exist.");
            var Embed = GetEmbed(Paint.Magenta)
                .WithAuthor("Detailed Command Information", Context.Client.CurrentUser.GetAvatarUrl())
                .AddField("Name", Command.Name, true)
                .AddField("Aliases", string.Join(", ", Command.Aliases), true)
                .AddField("Arguments", Command.Parameters.Any() ? string.Join(", ", Command.Parameters.Select(x => $"`{x.Type.Name}` {x.Name}")) : "No arguments.")
                .AddField("Usage", $"{Context.Config.Prefix}{Command.Name} {string.Join(" ", Command.Parameters)}")
                .AddField("Summary", Command.Summary)
                .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl());
            var GetChar = Command.Parameters.Where(x => x.Type == typeof(char));
            if (GetChar.Any()) Embed.AddField($"{GetChar.FirstOrDefault()?.Name} Values", "a, r, m. a = Add, r = remove, m = Modify.");
            var GetEnum = Command.Parameters.Where(x => x.Type.IsEnum == true);
            if (GetEnum.Any()) Embed.AddField($"{GetEnum.FirstOrDefault()?.Name} Values", string.Join(", ", GetEnum?.FirstOrDefault().Type.GetEnumNames()));
            return ReplyAsync(string.Empty, Embed.Build());
        }

        [Command("support"), Summary("Provides a link to Hifumi's support server")]
        public Task Support() => ReplyAsync("Need help with Hifumi? Join the support server:\nhttps://discord.gg/jqpcmev");
    }
}
