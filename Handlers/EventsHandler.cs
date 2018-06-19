using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Hifumi.Addons;
using Hifumi.Helpers;
using Hifumi.Services;
using Hifumi.Translation;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Hifumi.Handlers
{
    public class EventsHandler
    {
        CommandService CommandService { get; }
        ConfigHandler ConfigHandler { get; }
        DiscordSocketClient Client { get; }
        EventHelper EventHelper { get; }
        GuildHandler GuildHandler { get; }
        GuildHelper GuildHelper { get; }
        Random Random { get; }

        bool CommandExecuted { get; set; }
        IServiceProvider ServiceProvider { get; set; }

        public EventsHandler(CommandService commandService, ConfigHandler configHandler, DiscordSocketClient client, EventHelper eventHelper, GuildHandler guildHandler, GuildHelper guildHelper, Random random)
        {
            CommandService = commandService;
            ConfigHandler = configHandler;
            EventHelper = eventHelper;
            Client = client;
            GuildHandler = guildHandler;
            GuildHelper = guildHelper;
            Random = random;
        }

        public async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            await CommandService.AddModulesAsync(Assembly.GetEntryAssembly(), serviceProvider);
        }

        internal Task Ready()
        {
            LogService.Write("EVENTS", "Ready to serve users", ConsoleColor.Green);
            Client.SetActivityAsync(new Game($"{ConfigHandler.Config.Prefix}help | hifumi.vic485.xyz", ActivityType.Playing));
            return Task.CompletedTask;
        }

        internal Task LeftGuild(SocketGuild guild)
        {
            GuildHandler.RemoveGuild(guild.Id, guild.Name);
            return Task.CompletedTask;
        }

        internal Task GuildAvailable(SocketGuild guild)
        {
            if (!ConfigHandler.Config.ServerBlacklist.Contains($"{guild.Id}"))
                GuildHandler.AddGuild(guild.Id, guild.Name);
            else
                guild.LeaveAsync();
            return Task.CompletedTask;
        }

        internal Task Connected()
        {
            LogService.Write("DISCORD", "Connected to Discord", ConsoleColor.DarkBlue);
            return Task.CompletedTask;
        }

        internal Task Log(LogMessage log)
        {
            LogService.Write("LOG", log.Message ?? log.Exception.Message, ConsoleColor.DarkGreen);
            return Task.CompletedTask;
        }

        internal Task Disconnected(Exception error)
        {
            LogService.Write("DISCORD", $"Disconnected from Discord: {error.Message}", ConsoleColor.DarkBlue);
            return Task.CompletedTask;
        }

        internal Task LatencyUpdated(int old, int newer)
            => Client.SetStatusAsync((Client.ConnectionState == ConnectionState.Disconnected || newer > 500) ? UserStatus.DoNotDisturb
                : (Client.ConnectionState == ConnectionState.Connecting || newer > 250) ? UserStatus.Idle
                : (Client.ConnectionState == ConnectionState.Connected || newer < 100) ? UserStatus.Online : UserStatus.AFK);

        internal async Task JoinedGuildAsync(SocketGuild guild)
        {
            if (!ConfigHandler.Config.ServerBlacklist.Contains($"{guild.Id}"))
            {
                GuildHandler.AddGuild(guild.Id, guild.Name);
                await GuildHelper.DefaultChannel(guild.Id).SendMessageAsync("Thank you for inviting me to your server. Guild prefix is `h!`. Type `h!help` for commands.");
            }
            else
                await guild.LeaveAsync();
        }

        internal async Task UserLeftAsync(SocketGuildUser user)
        {
            var config = GuildHandler.GetGuild(user.Guild.Id);
            string message = !config.LeaveMessages.Any() ? Translator.GetMessage("default-leave", config.Locale, user.Username)
                : StringHelper.Replace(config.LeaveMessages[Random.Next(config.LeaveMessages.Count)], user.Guild.Name, user.Username);
            var channel = user.Guild.GetTextChannel(Convert.ToUInt64(config.LeaveChannel));
            if (channel != null) await channel.SendMessageAsync(message).ConfigureAwait(false);
            // TODO: logging?
        }

        internal async Task UserJoinedAsync(SocketGuildUser user)
        {
            var config = GuildHandler.GetGuild(user.Guild.Id);
            string message = !config.JoinMessages.Any() ? Translator.GetMessage("default-join", config.Locale, user.Mention)
                : StringHelper.Replace(config.JoinMessages[Random.Next(config.JoinMessages.Count)], user.Guild.Name, user.Mention);
            var channel = user.Guild.GetTextChannel(Convert.ToUInt64(config.JoinChannel));
            if (channel != null) await channel.SendMessageAsync(message).ConfigureAwait(false);
            var joinRole = user.Guild.GetRole(Convert.ToUInt64(config.Mod.JoinRole));
            if (joinRole != null) await user.AddRoleAsync(joinRole).ConfigureAwait(false);
            if (config.Mod.MutedUsers.Contains($"{user.Id}"))
            {
                var muteRole = user.Guild.GetRole(Convert.ToUInt64(config.Mod.MuteRole));
                await user.AddRoleAsync(muteRole).ConfigureAwait(false);
            }
            // TODO: logging?
        }

        internal Task HandleMessage(SocketMessage message)
        {
            var guild = (message.Channel as SocketGuildChannel).Guild;
            var config = GuildHandler.GetGuild(guild.Id);
            if (!(message is SocketUserMessage userMessage) || !(message.Author is SocketGuildUser user)) return Task.CompletedTask;
            if (userMessage.Source != MessageSource.User || userMessage.Author.IsBot || ConfigHandler.Config.UserBlacklist.Contains($"{user.Id}") ||
                ConfigHandler.Config.ServerBlacklist.Contains($"{guild.Id}") || GuildHelper.GetProfile(guild.Id, userMessage.Author.Id).IsBlacklisted) return Task.CompletedTask;

            _ = EventHelper.XPHandler(userMessage, config);
            _ = EventHelper.ModeratorAsync(userMessage, config);
            _ = EventHelper.ExecuteTag(userMessage, config);
            _ = EventHelper.AFKHandlerAsync(userMessage, config);
            return Task.CompletedTask;
        }

        internal async Task CommandHandlerAsync(SocketMessage message)
        {
            if (!(message is SocketUserMessage userMessage)) return;
            int argPos = 0;
            var context = new IContext(Client, userMessage, ServiceProvider);
            if (!(userMessage.HasStringPrefix(context.Config.Prefix, ref argPos) || userMessage.HasStringPrefix(context.Server.Prefix, ref argPos))
                || userMessage.Source != MessageSource.User || userMessage.Author.IsBot) return;
            if (context.Config.UserBlacklist.Contains($"{context.User.Id}") || ConfigHandler.Config.ServerBlacklist.Contains($"{context.Guild.Id}") || GuildHelper.GetProfile(context.Guild.Id, context.User.Id).IsBlacklisted) return;
            var result = await CommandService.ExecuteAsync(context, argPos, ServiceProvider, MultiMatchHandling.Best);
            CommandExecuted = result.IsSuccess;
            switch (result.Error)
            {
                case CommandError.Exception:
                    LogService.Write("EXCEPTION", result.ErrorReason, ConsoleColor.Red);
                    return;
                case CommandError.UnmetPrecondition:
                    // TODO: DM error if we can't send a message?
                    if (!result.ErrorReason.Contains("SendMessages")) await context.Channel.SendMessageAsync(result.ErrorReason);
                    return;
            }
            _ = Task.Run(() => EventHelper.RecordCommand(CommandService, context, argPos));
        }
    }
}
