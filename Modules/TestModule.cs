using Discord.Commands;
using Hifumi.Addons;
using Hifumi.Addons.Preconditions;
using Hifumi.Enums;
using System.Threading.Tasks;

namespace Hifumi.Modules
{
    public class TestModule : Base
    {
        [Command("ping"), RequireNSFW]
        public async Task PingAsync()
            => await ReplyAsync("pong");
    }
}
