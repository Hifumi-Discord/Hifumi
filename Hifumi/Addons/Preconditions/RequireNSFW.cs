using Discord.Commands;
using Hifumi.Translation;
using System;
using System.Threading.Tasks;

namespace Hifumi.Addons.Preconditions
{
    public class RequireNSFW : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo info, IServiceProvider provider)
        {
            var channel = context.Channel as Discord.ITextChannel;
            if (channel.IsNsfw) return Task.FromResult(PreconditionResult.FromSuccess());
            return Task.FromResult(PreconditionResult.FromError(Translator.GetMessage("require-nsfw", (context as IContext).Server.Locale, command: info.Name)));
        }
    }
}
