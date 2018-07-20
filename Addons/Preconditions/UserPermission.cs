using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Hifumi.Translation;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hifumi.Addons.Preconditions
{
    public class UserPermission : PreconditionAttribute
    {
        private string ErrorMessage { get; set; }
        private PermissionType Permission { get; set; }
        private GuildPermission GuildPermission {get;set;}
        private ChannelPermission ChannelPermission {get;set;}

        public UserPermission(ChannelPermission channelPermission, string message)
        {
            ErrorMessage = message;
            ChannelPermission = channelPermission;
            Permission = PermissionType.Channel;
        }

        public UserPermission(GuildPermission guildPermission, string message)
        {
            ErrorMessage = message;
            GuildPermission = guildPermission;
            Permission = PermissionType.Guild;
        }

        public UserPermission() => Permission = PermissionType.Default;

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext commandContext, CommandInfo command, IServiceProvider provider)
        {
            var context = commandContext as IContext;
            var user = context.User as SocketGuildUser;
            var defaultPerms = new[]
            {
                GuildPermission.Administrator, GuildPermission.BanMembers, GuildPermission.KickMembers,
                GuildPermission.ManageChannels, GuildPermission.ManageGuild, GuildPermission.ManageMessages
            };

            var special = user.Id == context.Client.GetApplicationInfoAsync().GetAwaiter().GetResult().Owner.Id || 
                user.Id == context.Guild.OwnerId;
            var success = false;
            switch (Permission)
            {
                case PermissionType.Channel:
                    success = user.GetPermissions(context.Channel as IGuildChannel).Has(ChannelPermission) || special;
                    break;
                case PermissionType.Guild:
                    success = user.GuildPermissions.Has(GuildPermission) || special;
                    break;
                case PermissionType.Default:
                    success = defaultPerms.Any(x => user.GuildPermissions.Has(x)) || special;
                    ErrorMessage = "owo"; // TODO: translation key
                    break;
            }

            return success ? Task.FromResult(PreconditionResult.FromSuccess()) : Task.FromResult(PreconditionResult.FromError("uwu")); // TODO: translation key
        }
    }

    public enum PermissionType
    {
        Guild,
        Default,
        Channel
    }
}
