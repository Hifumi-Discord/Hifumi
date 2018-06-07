using Discord;
using Discord.WebSocket;
using Raven.Client.Documents;
using System.Threading.Tasks;

namespace Hifumi.Handlers
{
    public class MainHandler
    {
        ConfigHandler ConfigHandler { get; }
        IDocumentStore Store { get; }
        DiscordSocketClient Client { get; }
        EventsHandler EventsHandler { get; }

        public MainHandler(ConfigHandler configHandler, EventsHandler eventsHandler, DiscordSocketClient client, IDocumentStore store)
        {
            ConfigHandler = configHandler;
            Store = store;
            Client = client;
            EventsHandler = eventsHandler;
        }

        public async Task InitializeAsync()
        {
            Client.Log += EventsHandler.Log;
            Client.Ready += EventsHandler.Ready;
            Client.LeftGuild += EventsHandler.LeftGuild;
            Client.Connected += EventsHandler.Connected;
            Client.UserLeft += EventsHandler.UserLeftAsync;
            Client.Disconnected += EventsHandler.Disconnected;
            Client.GuildAvailable += EventsHandler.GuildAvailable;
            Client.UserJoined += EventsHandler.UserJoinedAsync;
            Client.JoinedGuild += EventsHandler.JoinedGuildAsync;
            Client.LatencyUpdated += EventsHandler.LatencyUpdated;
            Client.ReactionAdded += EventsHandler.ReactionAddedAsync;
            Client.MessageReceived += EventsHandler.HandleMessage;
            Client.MessageDeleted += EventsHandler.MessageDeletedAsync;
            Client.ReactionRemoved += EventsHandler.ReactionRemovedAsync;
            Client.MessageReceived += EventsHandler.CommandHandlerAsync;

            await Client.LoginAsync(TokenType.Bot, ConfigHandler.Config.Token).ConfigureAwait(false);
            await Client.StartAsync().ConfigureAwait(false);
        }

        public async void ShutdownAsync(object sender, System.ConsoleCancelEventArgs e)
        {
            Services.LogService.Write(Enums.LogSource.EVT, "Shut down request recieved", System.Drawing.Color.DarkOrange);
            await Client.LogoutAsync();
            await Client.StopAsync();
        }
    }
}
