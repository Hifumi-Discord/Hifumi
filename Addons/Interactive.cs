using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hifumi.Addons
{
    public class Interactive<T> : VInteractive<T>
    {
        List<VInteractive<T>> interactiveList = new List<VInteractive<T>>();

        public Interactive<T> AddInteractive(VInteractive<T> interactive)
        {
            interactiveList.Add(interactive);
            return this;
        }

        public async Task<bool> JudgeAsync(IContext context, T typeParameter)
        {
            foreach (var interactive in interactiveList)
            {
                var result = await interactive.JudgeAsync(context, typeParameter);
                if (!result) return false;
            }
            return true;
        }
    }

    public class InteractiveUser : VInteractive<SocketMessage>
    {
        public Task<bool> JudgeAsync(IContext context, SocketMessage message) => Task.FromResult(context.User.Id == message.Author.Id);
    }

    public class InteractiveChannel : VInteractive<SocketMessage>
    {
        public Task<bool> JudgeAsync(IContext context, SocketMessage message) => Task.FromResult(context.Channel.Id == message.Channel.Id);
    }

    public interface VInteractive<T>
    {
        Task<bool> JudgeAsync(IContext context, T typeParameter);
    }
}
