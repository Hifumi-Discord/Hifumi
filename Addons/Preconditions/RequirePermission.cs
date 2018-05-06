using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Hifumi.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hifumi.Addons.Preconditions
{
    public class RequirePermission : PreconditionAttribute
    {
        AccessLevel AccessLevel { get; }
        public RequirePermission(AccessLevel accessLevel)
        {
            AccessLevel = accessLevel;
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext commandContext, CommandInfo command, IServiceProvider provider)
        {
            var context = commandContext as IContext;
            var guildUser = context.User as SocketGuildUser;
            var adminPerms = context.Guild.OwnerId == context.User.Id || guildUser.GuildPermissions.Administrator || guildUser.GuildPermissions.ManageGuild;
            var modPerms = new[] { GuildPermission.KickMembers, GuildPermission.BanMembers, GuildPermission.ManageChannels, GuildPermission.ManageMessages, GuildPermission.ManageRoles };
            if (AccessLevel >= AccessLevel.Administrator && adminPerms) return Task.FromResult(PreconditionResult.FromSuccess());
            else if (AccessLevel >= AccessLevel.Moderator && modPerms.Any(x => guildUser.GuildPermissions.Has(x))) return Task.FromResult(PreconditionResult.FromSuccess());
            else return Task.FromResult(PreconditionResult.FromError($"{command.Name} requires **{AccessLevel}** AccessLevel. `{context.Config.Prefix}info` for more information."));
        }
    }
}