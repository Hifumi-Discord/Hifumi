using Discord;
using Discord.WebSocket;
using Raven.Client.Documents;
using System.Threading.Tasks;

namespace Hifumi.Handlers
{
    public class MainHandler
    {
        ConfigHandler ConfigHandler { get; }
        DiscordSocketClient Client { get; }
        EventsHandler EventsHandler { get; }

        public MainHandler(ConfigHandler configHandler, DiscordSocketClient client, EventsHandler eventsHandler, IDocumentStore store)
        {
            ConfigHandler = configHandler;
            Client = client;
            EventsHandler = eventsHandler;
        }

        public async Task InitializeAsync()
        {
            Client.Connected += EventsHandler.Connected;
            Client.Disconnected += EventsHandler.Disconnected;
            Client.GuildAvailable += EventsHandler.GuildAvailable;
            Client.JoinedGuild += EventsHandler.JoinedGuildAsync;
            Client.LatencyUpdated += EventsHandler.LatencyUpdated;
            Client.LeftGuild += EventsHandler.LeftGuild;
            Client.Log += EventsHandler.Log;
            Client.MessageReceived += EventsHandler.CommandHandlerAsync;
            Client.MessageReceived += EventsHandler.HandleMessage;
            Client.Ready += EventsHandler.Ready;
            Client.UserJoined += EventsHandler.UserJoinedAsync;
            Client.UserLeft += EventsHandler.UserLeftAsync;

            await Client.LoginAsync(TokenType.Bot, ConfigHandler.Config.Token).ConfigureAwait(false);
            await Client.StartAsync().ConfigureAwait(false);
        }
    }
}
