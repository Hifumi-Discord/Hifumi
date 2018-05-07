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

            var passed = context.MethodHelper.EasternTime - profile.Commands[command.Name];
            if (passed >= cool) return Task.FromResult(PreconditionResult.FromSuccess());

            var wait = cool - passed;
            string min = string.Empty;
            string sec = string.Empty;
            if (wait.Minutes != 0)
            {
                if (wait.Minutes > 1) min = $"{wait.Minutes} minutes,";
                else min = $"{wait.Minutes} minute,";
            }
            if (wait.Seconds != 0)
            {
                if (wait.Seconds > 1) sec = $"{wait.Seconds} seconds";
                else sec = $"{wait.Seconds} second";
            }
            return Task.FromResult(PreconditionResult.FromError($"Too fast!  {GetEmote(EmoteType.Worried)} Please wait {min} {sec}."));
        }
    }
}
