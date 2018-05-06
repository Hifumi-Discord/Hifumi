using Discord;
using Discord.Commands;
using Hifumi.Addons;
using System.Threading.Tasks;

namespace Hifumi.Modules
{
    [Name("Owner Commands"), RequireOwner, RequireBotPermission(ChannelPermission.SendMessages)]
    public class OwnerModule : Base
    {
        [Command("owntest")]
        public Task OwnerTest() => ReplyAsync("Run owner test");
    }
}
