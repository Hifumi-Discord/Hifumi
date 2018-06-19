using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Hifumi.Addons;
using Hifumi.Handlers;
using Hifumi.Models;
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

        public EventHelper(Random random, GuildHelper guildHelper, DiscordSocketClient client, ConfigHandler configHandler, MethodHelper methodHelper)
        {
            Client = client;
            Random = random;
            GuildHelper = guildHelper;
            ConfigHandler = configHandler;
            MethodHelper = methodHelper;
        }

        internal Task XPHandler(SocketMessage message, GuildModel config)
        {
            var user = message.Author as IGuildUser;
            var blacklistedRoles = new List<ulong>(config.ChatXP.XPBlockedRoles.Select(x => Convert.ToUInt64(x)));
            var hasRole = (user as IGuildUser).RoleIds.Intersect(blacklistedRoles).Any();
            if (hasRole || !config.ChatXP.IsEnabled) return Task.CompletedTask;
            var profile = GuildHelper.GetProfile(user.GuildId, user.Id);
            int old = 0, newer = 0;
            if (!profile.LastMessage.HasValue)
            {
                profile.LastMessage = DateTime.UtcNow;
                profile.ChatXP += Random.Next(10, 21);
            }
            else
            {
                if (DateTime.UtcNow - profile.LastMessage.Value >= TimeSpan.FromMinutes(2))
                {
                    old = profile.ChatXP;
                    profile.ChatXP += Random.Next(10, 21);
                    profile.LastMessage = DateTime.UtcNow;
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
            // TODO: afk handling
            await Task.Delay(1);
        }

        internal void RecordCommand(CommandService commandService, IContext context, int argPos)
        {
            var search = commandService.Search(context, argPos);
            if (!search.IsSuccess) return;
            var command = search.Commands.FirstOrDefault().Command;
            var profile = GuildHelper.GetProfile(context.Guild.Id, context.User.Id);
            if (!profile.Commands.ContainsKey(command.Name)) profile.Commands.Add(command.Name, DateTime.UtcNow);
            profile.Commands[command.Name] = DateTime.UtcNow;
            GuildHelper.SaveProfile(context.Guild.Id, context.User.Id, profile);
        }

        internal async Task ModeratorAsync(SocketUserMessage message, GuildModel config)
        {
            // TODO: automod
            await Task.Delay(1);
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
                await message.Channel.SendMessageAsync(StringHelper.Replace(config.ChatXP.LevelMessage, user: user.Username, level: newLevel, credits: credits));
            if (!config.ChatXP.LeveledRoles.Any()) return;
            var role = user.Guild.GetRole(Convert.ToUInt64(config.ChatXP.LeveledRoles.Where(x => x.Value == newLevel).FirstOrDefault().Key));
            if (user.Roles.Contains(role) || !user.Guild.Roles.Contains(role)) return;
            await user.AddRoleAsync(role);
            foreach (var lvlrole in config.ChatXP.LeveledRoles)
                if (lvlrole.Value < newLevel)
                    if (user.Roles.Contains(user.Guild.GetRole(Convert.ToUInt64(lvlrole.Key))))
                        await user.RemoveRoleAsync(user.Guild.GetRole(Convert.ToUInt64(lvlrole.Key)));
        }
    }
}
