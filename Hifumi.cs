using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Hifumi.Handlers;
using Hifumi.Helpers;
using Hifumi.Services;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hifumi
{
    class Hifumi
    {
        static void Main(string[] args) => new Hifumi().InitializeAsync().GetAwaiter().GetResult();

        async Task InitializeAsync()
        {
            var services = new ServiceCollection()
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    MessageCacheSize = 20,
                    AlwaysDownloadUsers = true,
                    LogLevel = LogSeverity.Error
                }))
                .AddSingleton(new CommandService(new CommandServiceConfig
                {
                    ThrowOnError = true,
                    IgnoreExtraArgs = false,
                    CaseSensitiveCommands = false,
                    DefaultRunMode = RunMode.Async
                }))
                .AddSingleton(new DocumentStore
                {
                    Certificate = DatabaseHandler.DatabaseConfig.Certificate,
                    Database = DatabaseHandler.DatabaseConfig.DatabaseName,
                    Urls = DatabaseHandler.DatabaseConfig.DatabaseUrls
                }.Initialize())
                .AddSingleton<HttpClient>()
                .AddSingleton<LogService>()
                .AddSingleton<GuildHelper>()
                .AddSingleton<EventHelper>()
                .AddSingleton<MainHandler>()
                .AddSingleton<GuildHandler>()
                .AddSingleton<ConfigHandler>()
                .AddSingleton<RedditService>()
                .AddSingleton<MethodHelper>()
                .AddSingleton<EventsHandler>()
                .AddSingleton<DatabaseHandler>()
                .AddSingleton(new Random(Guid.NewGuid().GetHashCode()));

            var provider = services.BuildServiceProvider();
            provider.GetRequiredService<LogService>().PrintApplicationInformation();
            await provider.GetRequiredService<DatabaseHandler>().DatabaseCheck();
            await provider.GetRequiredService<MainHandler>().InitializeAsync();
            await provider.GetRequiredService<EventsHandler>().InitializeAsync(provider);

            await Task.Delay(-1);
        }
    }
}
