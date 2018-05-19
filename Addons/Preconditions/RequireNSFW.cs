using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Hifumi.Addons.Preconditions
{
    public class RequireNSFW : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo info, IServiceProvider provider)
        {
            var channel = context.Channel as Discord.ITextChannel;
            if (channel.IsNsfw || channel.Name.Contains("nsfw")) return Task.FromResult(PreconditionResult.FromSuccess());
            return Task.FromResult(PreconditionResult.FromError($"**{info.Name}** command can only be ran in NSFW channel."));
        }
    }
}