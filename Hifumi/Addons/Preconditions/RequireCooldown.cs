using Discord.Commands;
using Hifumi.Helpers;
using System;
using System.Threading.Tasks;

namespace Hifumi.Addons.Preconditions
{
    public class RequireCooldown : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider provider)
        {
            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
