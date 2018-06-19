using Discord;
using Discord.Commands;
using Hifumi.Enums;
using Hifumi.Services;
using System;
using System.Threading.Tasks;

namespace Hifumi.Addons
{
    public class Base : ModuleBase<IContext>
    {
        public async Task<IUserMessage> ReplyAsync(string message, Embed embed = null, DocumentType document = DocumentType.None)
        {
            await Context.Channel.TriggerTypingAsync();
            // TODO: save config
            return await base.ReplyAsync(message, false, embed, null);
        }
    }
}
