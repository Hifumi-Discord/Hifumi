using Discord.Commands;
using Hifumi.Addons;
using System.Threading.Tasks;

namespace Hifumi.Modules
{
    public class TestModule : Base
    {
        [Command("ping")]
        public async Task PingAsync()
            => await ReplyAsync("pong");
    }
}
