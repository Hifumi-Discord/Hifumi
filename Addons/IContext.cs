using Discord;
using Discord.Commands;
using Hifumi.Handlers;
using Hifumi.Helpers;
using Hifumi.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Hifumi.Addons
{
    public class IContext : ICommandContext
    {
        public IUser User { get; }
        public IGuild Guild { get; }
        public Random Random { get; }
        public GuildModel Server { get; }
        public ConfigModel Config { get; }
        public IDiscordClient Client { get; }
        public HttpClient HttpClient { get; }
        public IUserMessage Message { get; }
        public GuildHelper GuildHelper { get; }
        public IMessageChannel Channel { get; }
        public GuildHandler GuildHandler { get; }
        public IDocumentSession Session { get; }
        public ConfigHandler ConfigHandler { get; }
        public MethodHelper MethodHelper { get; }

        public IContext(IDiscordClient client, IUserMessage message, IServiceProvider serviceProvider)
        {
            Client = client;
            Message = message;
            User = message.Author;
            Channel = message.Channel;
            Guild = (message.Channel as IGuildChannel).Guild;
            Random = serviceProvider.GetRequiredService<Random>();
            HttpClient = serviceProvider.GetRequiredService<HttpClient>();
            GuildHelper = serviceProvider.GetRequiredService<GuildHelper>();
            GuildHandler = serviceProvider.GetRequiredService<GuildHandler>();
            Config = serviceProvider.GetRequiredService<ConfigHandler>().Config;
            ConfigHandler = serviceProvider.GetRequiredService<ConfigHandler>();
            MethodHelper = serviceProvider.GetRequiredService<MethodHelper>();
            Server = serviceProvider.GetRequiredService<GuildHandler>().GetGuild(Guild.Id);
            Session = serviceProvider.GetRequiredService<IDocumentStore>().OpenSession();
        }
    }
}
