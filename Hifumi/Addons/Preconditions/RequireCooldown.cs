using Discord.Commands;
using Hifumi.Helpers;
using System;
using System.Threading.Tasks;
using Hifumi.Translation;

namespace Hifumi.Addons.Preconditions
{
    public class RequireCooldown : PreconditionAttribute
    {
        TimeSpan cool;

        public RequireCooldown(int seconds) => cool = TimeSpan.FromSeconds(seconds);

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext commandContext, CommandInfo command, IServiceProvider provider)
        {
            var context = commandContext as IContext;
            var profile = context.GuildHelper.GetProfile(context.Guild.Id, context.User.Id);
            string commandName = command.Name.Split(" ")[0];
            if (!profile.Commands.ContainsKey(commandName)) return Task.FromResult(PreconditionResult.FromSuccess());

            var passed = DateTime.UtcNow - profile.Commands[commandName];
            if (passed >= cool) return Task.FromResult(PreconditionResult.FromSuccess());

            var wait = cool - passed;
            if (wait.Minutes == 0 && wait.Seconds == 0) return Task.FromResult(PreconditionResult.FromSuccess());
            return Task.FromResult(PreconditionResult.FromError(Translator.GetCooldown(context.Server.Locale, wait)));
        }
    }
}
