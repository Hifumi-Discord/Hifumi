using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Hifumi.Enums;
using Hifumi.Translation;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hifumi.Addons.Preconditions
{
    public class RequirePermission : PreconditionAttribute
    {
        AccessLevel AccessLevel { get; }

        public RequirePermission(AccessLevel al)
        {
            AccessLevel = al;
        }

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext commandContext, CommandInfo command, IServiceProvider services)
        {
            var context = commandContext as IContext;
            var guildUser = context.User as SocketGuildUser;

            var supportPerms = false;
            var adminPerms = context.Guild.OwnerId == context.User.Id || guildUser.GuildPermissions.Administrator || guildUser.GuildPermissions.ManageGuild;
            var modPerms = new[] { GuildPermission.KickMembers, GuildPermission.BanMembers, GuildPermission.ManageChannels, GuildPermission.ManageMessages, GuildPermission.ManageRoles };

            if (AccessLevel <= AccessLevel.Support && (supportPerms || (await context.Client.GetApplicationInfoAsync()).Owner.Id == guildUser.Id)) return PreconditionResult.FromSuccess();
            else if (AccessLevel <= AccessLevel.Admin && adminPerms) return PreconditionResult.FromSuccess();
            else if (AccessLevel <= AccessLevel.Moderator && modPerms.Any(x => guildUser.GuildPermissions.Has(x))) return PreconditionResult.FromSuccess();
            else return PreconditionResult.FromError(Translator.GetMessage("low-access", context.Server.Locale, command: command.Name, access: AccessLevel.ToString()));
        }
    }
}
