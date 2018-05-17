using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Hifumi.Addons;
using Hifumi.Handlers;
using Hifumi.Models;
using Hifumi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hifumi.Helpers
{
    public class EventHelper
    {
        Random Random { get; }
        GuildHelper GuildHelper { get; }
        DiscordSocketClient Client { get; }
        ConfigHandler ConfigHandler { get; }
        MethodHelper MethodHelper { get; }
        public static readonly TimeSpan GlobalTimeout = TimeSpan.FromSeconds(30);

        public EventHelper(Random random, GuildHelper guildHelper, DiscordSocketClient client, ConfigHandler configHandler, MethodHelper methodHelper)
        {
            Client = client;
            Random = random;
            GuildHelper = guildHelper;
            ConfigHandler = configHandler;
            MethodHelper = methodHelper;
        }

        internal async Task CheckStateAsync()
        {
            if (Client.ConnectionState == ConnectionState.Connected) return;

            var timeout = Task.Delay(GlobalTimeout);
            var connect = Client.StartAsync();
            var localTask = await Task.WhenAny(timeout, connect);

            if (localTask == timeout || connect.IsFaulted) Environment.Exit(1);
            else if (connect.IsCompletedSuccessfully)
            {
                LogService.Write(Enums.LogSource.DSD, "Client reconnected.", System.Drawing.Color.ForestGreen);
                return;
            }
            else Environment.Exit(1);
        }

        internal Task XPHandler(SocketMessage message, GuildModel config)
        {
            var user = message.Author as IGuildUser;
            var blacklistedRoles = new List<ulong>(config.ChatXP.XPBlockedRoles.Select(x => x));
            var hasRole = (user as IGuildUser).RoleIds.Intersect(blacklistedRoles).Any();
            if (hasRole || !config.ChatXP.IsEnabled) return Task.CompletedTask;
            var profile = GuildHelper.GetProfile(user.GuildId, user.Id);
            int old = 0, newer = 0;
            if (!profile.LastMessage.HasValue)
            {
                profile.LastMessage = MethodHelper.EasternTime;
                profile.ChatXP += Random.Next(10, 21);
            }
            else
            {
                if (MethodHelper.EasternTime - profile.LastMessage >= TimeSpan.FromMinutes(2))
                {
                    old = profile.ChatXP;
                    profile.ChatXP += Random.Next(10, 21);
                    newer = profile.ChatXP;
                }
            }
            GuildHelper.SaveProfile(Convert.ToUInt64(config.Id), user.Id, profile);
            return LevelUpHandlerAsync(message, config, old, newer);
        }

        internal Task ExecuteTag(SocketMessage message, GuildModel config)
        {
            if (!config.Tags.Any(x => x.AutoRespond == true)) return Task.CompletedTask;
            var tags = config.Tags.Where(x => x.AutoRespond == true);
            var content = tags.FirstOrDefault(x => message.Content.StartsWith(x.Name));
            if (content != null) return message.Channel.SendMessageAsync(content.Content);
            return Task.CompletedTask;
        }

        internal async Task AFKHandlerAsync(SocketMessage message, GuildModel config)
        {
            if (!message.MentionedUsers.Any(x => config.AFK.ContainsKey(x.Id))) return;
            string reason = null;
            var user = message.MentionedUsers.FirstOrDefault(u => config.AFK.TryGetValue(u.Id, out reason));
            if (user != null) await message.Channel.SendMessageAsync($"{user.Username} is afk: {reason}");
        }

        internal void RecordCommand(CommandService commandService, IContext context, int argPos)
        {
            var search = commandService.Search(context, argPos);
            if (!search.IsSuccess) return;
            var command = search.Commands.FirstOrDefault().Command;
            var profile = GuildHelper.GetProfile(context.Guild.Id, context.User.Id);
            if (!profile.Commands.ContainsKey(command.Name)) profile.Commands.Add(command.Name, MethodHelper.EasternTime);
            profile.Commands[command.Name] = MethodHelper.EasternTime;
            GuildHelper.SaveProfile(context.Guild.Id, context.User.Id, profile);
        }

        internal async Task ModeratorAsync(SocketUserMessage message, GuildModel config)
        {
            if (GuildHelper.ProfanityMatch(message.Content) && config.Mod.AntiProfanity)
                await WarnUserAsync(message, config, $"{message.Author.Mention}, refrain from using profanity. You have been warned.");
            if (GuildHelper.InviteMatch(message.Content) && config.Mod.AntiInvite)
                await WarnUserAsync(message, config, $"{message.Author.Mention}, invite links are not allowed. You have been warned.");
        }

        async Task WarnUserAsync(SocketUserMessage message, GuildModel config, string warning)
        {
            var guild = (message.Author as SocketGuildUser).Guild;
            if (config.Mod.MaxWarnings == 0 || message.Author.Id == guild.OwnerId) return;
            await message.DeleteAsync();
            var profile = GuildHelper.GetProfile(guild.Id, message.Author.Id);
            if (profile.Warnings >= config.Mod.MaxWarnings)
            {
                await (message.Author as SocketGuildUser).KickAsync("Kicked By AutoMod.");
                await guild.GetTextChannel(config.Mod.TextChannel).SendMessageAsync(
                    $"**Kick** | Case {config.Mod.Cases.Count + 1}\n**User:** {message.Author} ({message.Author.Id})\n**Reason:** Reached max warnings.\n" +
                    $"**Responsible Moderator:** {Client.CurrentUser}"
                );
            }
            else
            {
                profile.Warnings++;
                GuildHelper.SaveProfile(guild.Id, message.Author.Id, profile);
            }
            await message.Channel.SendMessageAsync(warning);
        }

        async Task LevelUpHandlerAsync(SocketMessage message, GuildModel config, int oldXp, int newXp)
        {
            var user = message.Author as SocketGuildUser;
            int oldLevel = IntHelper.GetLevel(oldXp);
            int newLevel = IntHelper.GetLevel(newXp);
            if (!(newLevel > oldLevel)) return;
            int credits = (int)Math.Sqrt(Math.PI * newXp);
            var profile = GuildHelper.GetProfile(user.Guild.Id, user.Id);
            profile.Credits += credits;
            GuildHelper.SaveProfile(user.Guild.Id, user.Id, profile);
            if (!string.IsNullOrWhiteSpace(config.ChatXP.LevelMessage))
                await message.Channel.SendMessageAsync(StringHelper.Replace(config.ChatXP.LevelMessage, user: $"{user}", level: newLevel, credits: credits));
            if (!config.ChatXP.LeveledRoles.Any()) return;
            var role = user.Guild.GetRole(config.ChatXP.LeveledRoles.Where(x => x.Value == newLevel).FirstOrDefault().Key);
            if (user.Roles.Contains(role) || !user.Guild.Roles.Contains(role)) return;
            await user.AddRoleAsync(role);
            foreach (var lvlrole in config.ChatXP.LeveledRoles)
                if (lvlrole.Value < newLevel)
                    if (user.Roles.Contains(user.Guild.GetRole(lvlrole.Key)))
                        await user.RemoveRoleAsync(user.Guild.GetRole(lvlrole.Key));
        }
    }
}
