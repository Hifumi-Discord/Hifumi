using Discord;
using Discord.Commands;
using Hifumi.Addons;
using Hifumi.Addons.Preconditions;
using System;
using System.Threading.Tasks;

#warning Temporary commands. Do not include in build

namespace Hifumi.Modules
{
    [Name("Temporary Non-shippable Commands"), RequireOwner]
    public class NoBuildModule : Base
    {
        [Command("shutdown")]
        public Task Shutdown()
        {
            Environment.Exit(Environment.ExitCode);
            return Task.CompletedTask;
        }

        [Command("cooltest"), RequireCooldown(120)]
        public Task TestCommand() => ReplyAsync("Ran test command");

        [Command("channeltest")]
        public Task ChannelTest() => ReplyAsync($"Default channel is: {Context.GuildHelper.DefaultChannel(Context.Guild.Id).Name}");
    }
}
