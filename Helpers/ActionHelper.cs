using Discord.WebSocket;
using Hifumi.Enums;
using Hifumi.Models;
using System.Threading.Tasks;
using static Hifumi.Addons.Embeds;

namespace Hifumi.Helpers
{
    public class ActionHelper
    {
        public static Task LogUserAction(GuildActions action, SocketGuildUser user, GuildModel config, string role = null)
        {
            var channel = user.Guild.GetTextChannel(config.GuildActions.TextChannel);
            if (channel == null) return Task.CompletedTask;
            string title = null;
            switch (action)
            {
                case GuildActions.AutoRoleAdd:
                    if (!config.GuildActions.AutoRoleAdd) return Task.CompletedTask;
                    title = "AUTOROLE ADD";
                    break;
                case GuildActions.SelfroleAdd:
                    if (!config.GuildActions.SelfroleAdd) return Task.CompletedTask;
                    title = "SELFROLE ADD";
                    break;
                case GuildActions.SelfroleRemove:
                    if (!config.GuildActions.SelfroleRemove) return Task.CompletedTask;
                    title = "SELFROLE REMOVE";
                    break;
                case GuildActions.UserJoin:
                    if (!config.GuildActions.UserJoin) return Task.CompletedTask;
                    title = "USER JOIN";
                    break;
                case GuildActions.UserLeave:
                    if (!config.GuildActions.UserLeave) return Task.CompletedTask;
                    title = "USER LEAVE";
                    break;
            }

            var embed = GetEmbed(Paint.Temporary)
                .WithTitle(title)
                .AddField("User", $"{user}", true)
                .AddField("Mention", user.Mention, true)
                .WithCurrentTimestamp();

            if (role != null)
                embed.AddField("Role", role);

            channel.SendMessageAsync(string.Empty, embed: embed.Build());
            return Task.CompletedTask;
        }
    }
}
