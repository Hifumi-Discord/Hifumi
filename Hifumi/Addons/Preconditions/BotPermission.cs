using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Hifumi.Addons.Preconditions
{
    public class BotPermission : PreconditionAttribute
    {
        GuildPermission GuildPermission {get;}

        public BotPermission(GuildPermission guildPermission)
        {
            GuildPermission = guildPermission;
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider provider)
        {
            string permission = null;
            var isSuccess = ((SocketGuild)context.Guild).CurrentUser.GuildPermissions.Has(GuildPermission);
            switch(GuildPermission)
            {
                case GuildPermission.BanMembers:
                    permission = "ban users";
                    break;
                case GuildPermission.KickMembers:
                    permission = "kick users";
                    break;
                case GuildPermission.ManageRoles:
                    permission = "manage roles";
                    break;
                case GuildPermission.ManageMessages:
                    permission = "manage messages";
                    break;
                case GuildPermission.ManageChannels:
                    permission = "manage channels";
                    break;
            }

            return isSuccess ? Task.FromResult(PreconditionResult.FromSuccess()) : Task.FromResult(PreconditionResult.FromError($"missing permission: {permission}."));
            // TODO: translation key
        }
    }
}
