using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Hifumi.Addons;
using static Hifumi.Addons.Embeds;
using Hifumi.Enums;
using Hifumi.Helpers;
using Hifumi.Models;
using Hifumi.Personality;
using static Hifumi.Personality.Emotes;
using Hifumi.Services;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CC = System.Drawing.Color;

namespace Hifumi.Handlers
{
    public class EventsHandler
    {
        Random Random { get; }
        GuildHelper GuildHelper { get; }
        EventHelper EventHelper { get; }
        GuildHandler GuildHandler { get; }
        DiscordSocketClient Client { get; }
        bool CommandExecuted { get; set; }
        ConfigHandler ConfigHandler { get; }
        IServiceProvider ServiceProvider { get; set; }
        CommandService CommandService { get; }
        CancellationTokenSource CancellationToken { get; set; }

        public EventsHandler(GuildHandler guildHandler, ConfigHandler configHandler, DiscordSocketClient client, CommandService commandService, Random random, GuildHelper guildHelper, EventHelper eventHelper)
        {
            Client = client;
            Random = random;
            GuildHandler = guildHandler;
            GuildHelper = guildHelper;
            ConfigHandler = configHandler;
            EventHelper = eventHelper;
            CommandService = commandService;
            CancellationToken = new CancellationTokenSource();
        }

        public async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            await CommandService.AddModulesAsync(Assembly.GetEntryAssembly(), serviceProvider);
        }

        internal Task Ready() => Task.Run(() =>
        {
            LogService.Write(LogSource.RDY, "Ready to serve users", CC.GreenYellow);
            Client.SetActivityAsync(new Game(!ConfigHandler.Config.Games.Any() ?
                $"{ConfigHandler.Config.Prefix}help" : $"{ConfigHandler.Config.Games[Random.Next(ConfigHandler.Config.Games.Count)]}", ActivityType.Playing));
        });

        internal Task LeftGuild(SocketGuild guild) => Task.Run(() => GuildHandler.RemoveGuild(guild.Id, guild.Name));

        internal Task GuildAvailable(SocketGuild guild)
        {
            if (!ConfigHandler.Config.ServerBlacklist.Contains(guild.Id))
                GuildHandler.AddGuild(guild.Id, guild.Name);
            else
                guild.LeaveAsync();
            return Task.CompletedTask;
        }

        internal Task Connected()
        {
            CancellationToken.Cancel();
            CancellationToken = new CancellationTokenSource();
            LogService.Write(LogSource.CNN, "Connected to Discord", CC.BlueViolet);
            return Task.CompletedTask;
        }

        internal Task Log(LogMessage log) => Task.Run(() => LogService.Write(LogSource.EXC, log.Message ?? log.Exception.Message, CC.Crimson));

        internal Task Disconnected(Exception error)
        {
            _ = Task.Delay(EventHelper.GlobalTimeout, CancellationToken.Token).ContinueWith(async _ =>
            {
                LogService.Write(LogSource.DSN, $"Checking connection state...", CC.LightYellow);
                await EventHelper.CheckStateAsync();
            });
            return Task.CompletedTask;
        }

        internal Task LatencyUpdated(int old, int newer) => Client.SetStatusAsync((Client.ConnectionState == ConnectionState.Disconnected || newer > 500) ? UserStatus.DoNotDisturb
            : (Client.ConnectionState == ConnectionState.Connecting || newer > 250) ? UserStatus.Idle
            : (Client.ConnectionState == ConnectionState.Connected || newer < 100) ? UserStatus.Online : UserStatus.AFK);

        internal async Task JoinedGuildAsync(SocketGuild guild)
        {
            if (!ConfigHandler.Config.ServerBlacklist.Contains(guild.Id))
            {
                GuildHandler.AddGuild(guild.Id, guild.Name);
                await GuildHelper.DefaultChannel(guild.Id).SendMessageAsync(ConfigHandler.Config.JoinMessage ?? "Thank you for inviting me to your server. Guild prefix is `h!`. Type `h!help` for commands.");
            }
            else
                await guild.LeaveAsync();
        }

        internal async Task UserLeftAsync(SocketGuildUser user)
        {
            var config = GuildHandler.GetGuild(user.Guild.Id);
            string message = !config.LeaveMessages.Any() ? $"**{user.Username}** left the server. {GetEmote(EmoteType.Sad)}"
                : StringHelper.Replace(config.LeaveMessages[Random.Next(config.LeaveMessages.Count)], user.Guild.Name, user.Username);
            var channel = user.Guild.GetTextChannel(config.LeaveChannel);
            if (channel != null) await channel.SendMessageAsync(message).ConfigureAwait(false);
        }

        internal async Task UserJoinedAsync(SocketGuildUser user)
        {
            var config = GuildHandler.GetGuild(user.Guild.Id);
            string message = !config.JoinMessages.Any() ? $"Welcome **{user.Mention}** to the server. {GetEmote(EmoteType.Happy)}"
                : StringHelper.Replace(config.JoinMessages[Random.Next(config.JoinMessages.Count)], user.Guild.Name, user.Mention);
            var channel = user.Guild.GetTextChannel(config.JoinChannel);
            if (channel != null) await channel.SendMessageAsync(message).ConfigureAwait(false);
            var role = user.Guild.GetRole(config.Mod.JoinRole);
            if (role != null) await user.AddRoleAsync(role).ConfigureAwait(false);
            if (config.Mod.MutedUsers.Contains(user.Id))
            {
                var muteRole = user.Guild.GetRole(config.Mod.MuteRole);
                await user.AddRoleAsync(muteRole).ConfigureAwait(false);
            }
        }

        internal Task HandleMessage(SocketMessage message)
        {
            var guild = (message.Channel as SocketGuildChannel).Guild;
            var config = GuildHandler.GetGuild(guild.Id);
            if (!(message is SocketUserMessage userMessage) || !(message.Author is SocketGuildUser user)) return Task.CompletedTask;
            if (userMessage.Source != MessageSource.User || userMessage.Author.IsBot || ConfigHandler.Config.UserBlacklist.Contains(user.Id) ||
                ConfigHandler.Config.ServerBlacklist.Contains(guild.Id) || GuildHelper.GetProfile(guild.Id, userMessage.Author.Id).IsBlacklisted) return Task.CompletedTask;

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
            if (!(userMessage.HasStringPrefix(context.Config.Prefix, ref argPos) || userMessage.HasStringPrefix(context.Server.Prefix, ref argPos) ||
                userMessage.HasMentionPrefix(Client.CurrentUser, ref argPos)) || userMessage.Source != MessageSource.User || userMessage.Author.IsBot) return;
            if (context.Config.UserBlacklist.Contains(userMessage.Author.Id) || ConfigHandler.Config.ServerBlacklist.Contains(context.Guild.Id) || GuildHelper.GetProfile(context.Guild.Id, context.User.Id).IsBlacklisted) return;
            var result = await CommandService.ExecuteAsync(context, argPos, ServiceProvider, MultiMatchHandling.Best);
            CommandExecuted = result.IsSuccess;
            switch (result.Error)
            {
                case CommandError.Exception:
                    LogService.Write(LogSource.EXC, result.ErrorReason, CC.Crimson);
                    break;
                case CommandError.UnmetPrecondition:
                    // TODO: DM error if we can't send a message?
                    if (!result.ErrorReason.Contains("SendMessages")) await context.Channel.SendMessageAsync(result.ErrorReason);
                    return;
                case CommandError.BadArgCount:
                case CommandError.ObjectNotFound:
                case CommandError.ParseFailed:
                case CommandError.UnknownCommand:
                case CommandError.Unsuccessful:
                    await context.Channel.SendMessageAsync($"{result.Error}: {result.ErrorReason}");
                    break;
            }
            _ = Task.Run(() => EventHelper.RecordCommand(CommandService, context, argPos));
        }

        internal async Task MessageDeletedAsync(Cacheable<IMessage, ulong> cacheable, ISocketMessageChannel channel)
        {
            var config = GuildHandler.GetGuild((channel as SocketGuildChannel).Guild.Id);
            var message = await cacheable.GetOrDownloadAsync();
            if (message == null || config == null || !config.Mod.LogDeletedMessages || CommandExecuted) return;
            config.DeletedMessages.Add(new MessageWrapper
            {
                ChannelId = channel.Id,
                MessageId = message.Id,
                AuthorId = message.Author.Id,
                DateTime = message.Timestamp.DateTime,
                Content = message.Content ?? message.Attachments.FirstOrDefault()?.Url
            });
            GuildHandler.Save(config);
        }
    }
}
