using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Hifumi.Addons;
using Hifumi.Enums;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Hifumi.Modules
{
    [Name("Owner Commands"), RequireOwner, RequireBotPermission(ChannelPermission.SendMessages)]
    public class OwnerModule : Base
    {
        [Command("userblacklist"), Summary("Adds or Removes a user from the global blacklist.")]
        public Task UserBlacklist(string action, ulong userid)
        {
            switch (action.ToLower())
            {
                case "add":
                    if (Context.Config.UserBlacklist.Contains(userid)) return ReplyAsync($"User ({userid}) is already globally blacklisted");
                    Context.Config.UserBlacklist.Add(userid);
                    return ReplyAsync($"User ({userid}) has been added to the global blacklist.", document: DocumentType.Config);
                case "remove":
                    if (!Context.Config.UserBlacklist.Contains(userid)) return ReplyAsync($"User ({userid}) is not globally blacklisted.");
                    Context.Config.UserBlacklist.Remove(userid);
                    return ReplyAsync($"User ({userid}) has been removed from the global blacklist.");
            }
            return Task.CompletedTask;
        }

        [Command("serverblacklist"), Summary("Adds or Removes a server from the global blacklist.")]
        public Task ServerBlacklist(string action, ulong guildid)
        {
            switch (action.ToLower())
            {
                case "add":
                    if (Context.Config.ServerBlacklist.Contains(guildid)) return ReplyAsync($"Guild ({guildid}) is already globally blacklisted");
                    Context.Config.ServerBlacklist.Add(guildid);
                    return ReplyAsync($"Guild ({guildid}) has been added to the global blacklist.", document: DocumentType.Config);
                case "remove":
                    if (!Context.Config.ServerBlacklist.Contains(guildid)) return ReplyAsync($"Guild ({guildid}) is not globally blacklisted.");
                    Context.Config.ServerBlacklist.Remove(guildid);
                    return ReplyAsync($"Guild ({guildid}) has been removed from the global blacklist.");
            }
            return Task.CompletedTask;
        }

        [Command("update"), Summary("Updates Hifumi's Information.")]
        public async Task UpdateAsync(UpdateType updateType, [Remainder] string value)
        {
            var channelCheck = Context.GuildHelper.GetChannelId(Context.Guild as SocketGuild, value);
            var getChannel = (Context.Guild as SocketGuild).GetTextChannel(channelCheck.Item2) as SocketTextChannel;
            switch (updateType)
            {
                case UpdateType.Avatar:
                    using (var picture = new FileStream(value, FileMode.Open))
                        await Context.Client.CurrentUser.ModifyAsync(x => x.Avatar = new Image(picture));
                    break;
                case UpdateType.Username:
                    await Context.Client.CurrentUser.ModifyAsync(x => x.Username = value);
                    break;
                case UpdateType.Status:
                    var split = value.Split(":");
                    await (Context.Channel as DiscordSocketClient).SetActivityAsync(new Game(split[1], (ActivityType)Enum.Parse<ActivityType>(split[0]))).ConfigureAwait(false);
                    break;
                case UpdateType.Prefix:
                    Context.Config.Prefix = value;
                    break;
                case UpdateType.Nickname:
                    await (await Context.Guild.GetCurrentUserAsync(CacheMode.AllowDownload)).ModifyAsync(x => x.Nickname = value);
                    break;
                case UpdateType.ReportChannel:
                    Context.Config.ReportChannel = getChannel.Id;
                    break;
                case UpdateType.JoinMessage:
                    Context.Config.JoinMessage = value;
                    break;
            }
            await ReplyAsync($"{updateType} has been updated.", document: DocumentType.Config);
        }
    }
}
