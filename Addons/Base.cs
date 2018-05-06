using Discord;
using Discord.Commands;
using Discord.WebSocket;
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
            _ = Task.Run(() => SaveDocuments(document));
            return await base.ReplyAsync(message, false, embed, null);
        }

        public async Task<IUserMessage> ReplyAndDeleteAsync(string message, TimeSpan? timeout = null)
        {
            timeout = timeout ?? TimeSpan.FromSeconds(5);
            var msg = await ReplyAsync(message).ConfigureAwait(false);
            _ = Task.Delay(timeout.Value).ContinueWith(_ => msg.DeleteAsync().ConfigureAwait(false)).ConfigureAwait(false);
            return msg;
        }

        public Task<SocketMessage> ResponseWaitAsync(bool user = true, bool channel = true, TimeSpan? timeout = null)
        {
            var interactive = new Interactive<SocketMessage>();
            if (user) interactive.AddInteractive(new InteractiveUser());
            if (channel) interactive.AddInteractive(new InteractiveChannel());
            return ResponseWaitAync(interactive, timeout);
        }

        async Task<SocketMessage> ResponseWaitAync(VInteractive<SocketMessage> interactive, TimeSpan? timeout = null)
        {
            timeout = timeout ?? TimeSpan.FromSeconds(15);
            var trigger = new TaskCompletionSource<SocketMessage>();
            async Task InteractiveHandlerAsync(SocketMessage message)
            {
                var result = await interactive.JudgeAsync(Context, message).ConfigureAwait(false);
                if (result) trigger.SetResult(message);
            }
            (Context.Client as DiscordSocketClient).MessageReceived += InteractiveHandlerAsync;
            var personalTask = await Task.WhenAny(trigger.Task, Task.Delay(timeout.Value)).ConfigureAwait(false);
            (Context.Client as DiscordSocketClient).MessageReceived -= InteractiveHandlerAsync;
            if (personalTask == trigger.Task) return await trigger.Task.ConfigureAwait(false); else return null;
        }

        void SaveDocuments(DocumentType document)
        {
            bool check = false;
            switch (document)
            {
                case DocumentType.None: check = true; break;
                case DocumentType.Config:
                    Context.ConfigHandler.Save(Context.Config);
                    check = !Context.Session.Advanced.HasChanges;
                    break;
                case DocumentType.Server:
                    Context.GuildHandler.Save(Context.Server);
                    check = !Context.Session.Advanced.HasChanges;
                    break;
            }
            if (check == false) LogService.Write(LogSource.DOC, $"Failed to save {document} document.", System.Drawing.Color.Crimson);
        }
    }
}
