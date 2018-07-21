using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Hifumi.Handlers;
using Hifumi.Helpers;
using Hifumi.Services;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using Raven.Client.Http;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hifumi
{
    class Hifumi
    {
        public const string Version = "2018-Beta-07-18";
        public static bool Headless { get; private set; }

        static void Main(string[] args)
        {
            if (args.Length != 0 && args[0].ToLower() == "headless")
                Headless = true;
            else Headless = false;

            new Hifumi().InitializeAsync().GetAwaiter().GetResult();
        }

        async Task InitializeAsync()
        {
            var services = new ServiceCollection()
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    MessageCacheSize = 20,
                    AlwaysDownloadUsers = true,
                    LogLevel = LogSeverity.Error,
                    ShardId = ConfigSettings.Configuration.Shard,
                    TotalShards = ConfigSettings.Configuration.TotalShards
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
                    Certificate = ConfigSettings.Configuration.Certificate,
                    Database = ConfigSettings.Configuration.DatabaseName,
                    Urls = ConfigSettings.Configuration.DatabaseUrls,
                    Conventions = {
                        ReadBalanceBehavior = ReadBalanceBehavior.FastestNode
                    }
                }.Initialize())
                .AddSingleton<HttpClient>()
                .AddSingleton<LogService>()
                .AddSingleton<GuildHelper>()
                .AddSingleton<EventHelper>()
                .AddSingleton<MainHandler>()
                .AddSingleton<GuildHandler>()
                .AddSingleton<ConfigHandler>()
                .AddSingleton<MethodHelper>()
                .AddSingleton<EventsHandler>()
                .AddSingleton(new Random(Guid.NewGuid().GetHashCode()));

            var provider = services.BuildServiceProvider();
            new LogService().Initialize();
            provider.GetRequiredService<ConfigHandler>().CheckConfig();
            await provider.GetRequiredService<MainHandler>().InitializeAsync();
            await provider.GetRequiredService<EventsHandler>().InitializeAsync(provider);

            await Task.Delay(-1);
        }
    }
}
