using System;
using Discord;
using System.Linq;
using Hifumi.Addons;
using Hifumi.Translation;
using Discord.Commands;
using System.Threading.Tasks;
using static Hifumi.Addons.Embeds;

namespace Hifumi.Modules
{
    [Name("helpmodule"), RequireBotPermission(ChannelPermission.SendMessages)]
    public class HelpModule : Base
    {
        IServiceProvider Provider { get; }
        CommandService CommandService { get; }
        HelpModule(CommandService command, IServiceProvider provider)
        {
            CommandService = command;
            Provider = provider;
        }

        [Command("help")]
        public Task Help()
        {
            var Embed = GetEmbed(Paint.Aqua)
                .WithAuthor("List of all commands", Context.Client.CurrentUser.GetAvatarUrl())
                .WithFooter($"For More Information On A Command's Usage: {Context.Config.Prefix}info <command>", Emotes.Hifumi.Url);
            foreach (var commands in CommandService.Commands.Where(x => x.Module.Name != "Owner Commands").GroupBy(x => x.Module.Name).OrderBy(y => y.Key))
                Embed.AddField(Translator.GetModule(commands.Key, Context.Server.Locale), $"`{string.Join("`, `", commands.Select(x => x.Name).Distinct())}`");
            return ReplyAsync(string.Empty, Embed.Build());
        }

        [Command("info"), Alias("help")]
        public Task CommandInfo([Remainder] string commandName)
        {
            var command = CommandService.Search(Context, commandName).Commands?.FirstOrDefault().Command;
            if (command == null) return ReplyAsync($"{commandName} command doesn't exist.");
            var translatedCommand = Translator.GetCommand(command.Name, Context.Server.Locale);
            return ReplyAsync($"**{command.Name}**\n\n{translatedCommand.Summary}\n{translatedCommand.Usage}");
        }

        [Command("support")]
        public Task Support() => ReplyAsync("Need help with Hifumi? Join the support server:\nhttps://discord.gg/jqpcmev");
    }
}
