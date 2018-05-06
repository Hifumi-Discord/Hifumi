using Discord.Commands;
using Hifumi.Helpers;
using Hifumi.Personality;
using static Hifumi.Personality.Emotes;
using System;
using System.Threading.Tasks;

namespace Hifumi.Addons.Preconditions
{
    public class RequireCooldown : PreconditionAttribute
    {
        TimeSpan cool;

        public RequireCooldown(int seconds) => cool = TimeSpan.FromSeconds(seconds);

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext commandContext, CommandInfo command, IServiceProvider serviceProvider)
        {
            var context = commandContext as IContext;
            var profile = context.GuildHelper.GetProfile(context.Guild.Id, context.User.Id);
            if (!profile.Commands.ContainsKey(command.Name)) return Task.FromResult(PreconditionResult.FromSuccess());

            var passed = DateTime.UtcNow - profile.Commands[command.Name];
            if (passed >= cool) return Task.FromResult(PreconditionResult.FromSuccess());

            var wait = cool - passed;
            // TODO: personality and don't send minutes if there are none
            return Task.FromResult(PreconditionResult.FromError($"Too fast!  {GetEmote(EmoteType.Worried)} Please wait {wait.Minutes} minute(s) and {wait.Seconds} second(s)."));
        }
    }
}
