using Discord;
using Discord.WebSocket;
using Hifumi.Addons;
using static Hifumi.Addons.Embeds;
using Hifumi.Handlers;
using Hifumi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hifumi.Helpers
{
    public class GuildHelper
    {
        GuildHandler GuildHandler { get; }
        DiscordSocketClient Client { get; }
        public GuildHelper(GuildHandler guildHandler, DiscordSocketClient client)
        {
            Client = client;
            GuildHandler = guildHandler;
        }

        string ProfanityRegex { get => @"\b(f+u+c+k+|b+i+t+c+h+|w+h+o+r+e+|c+u+n+t+|a+s+s+|n+i+g+g+|f+a+g+|g+a+y+|p+u+s+s+y+)(w+i+t+|e+r+|i+n+g+|h+o+l+e+)?\b"; }
        string InviteRegex { get => @"^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?(d+i+s+c+o+r+d+|a+p+p)+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$"; }

        public IMessageChannel DefaultChannel(ulong guildId)
        {
            var guild = Client.GetGuild(guildId);
            return guild.TextChannels.FirstOrDefault(x => x.Name.Contains("general") || x.Name.Contains("chat") || x.Id == guild.Id) ?? guild.DefaultChannel;
        }

        public UserProfile GetProfile(ulong guildId, ulong userId)
        {
            var guild = GuildHandler.GetGuild(Client.GetGuild(guildId).Id);
            if (!guild.Profiles.ContainsKey(userId))
            {
                guild.Profiles.Add(userId, new UserProfile());
                GuildHandler.Save(guild);
                return guild.Profiles[userId];
            }
            return guild.Profiles[userId];
        }

        public void SaveProfile(ulong guildId, ulong userId, UserProfile profile)
        {
            var config = GuildHandler.GetGuild(guildId);
            config.Profiles[userId] = profile;
            GuildHandler.Save(config);
        }

        public async Task LogAsync(IContext context, IUser user, CaseType caseType, string reason)
        {
            reason = reason ?? $"*Responsible moderator, please type `{context.Config.Prefix}reason {context.Server.Mod.Cases.Count + 1} <reason>`*";
            var modChannel = await context.Guild.GetTextChannelAsync(context.Server.Mod.TextChannel);
            if (modChannel == null) return;
            var embed = GetEmbed(CasePaint(caseType))
                .WithAuthor($"Case #{context.Server.Mod.Cases.Count + 1} | {caseType} | {user.Username}#{user.Discriminator}", user.GetAvatarUrl())
                .AddField("User", user.Mention, true)
                .AddField("Moderator", context.User.Mention, true)
                .AddField("Reason", reason)
                .WithFooter($"ID: {user.Id}")
                .WithCurrentTimestamp()
                .Build();
            var message = await modChannel.SendMessageAsync(string.Empty, embed: embed);
            context.Server.Mod.Cases.Add(new CaseWrapper
            {
                Reason = reason,
                UserId = user.Id,
                CaseType = caseType,
                MessageId = message.Id,
                ModId = context.User.Id,
                CaseNumber = context.Server.Mod.Cases.Count + 1
            });
            context.GuildHandler.Save(context.Server);
        }

        public async Task LogAsync(SocketGuild guild, IUser user, IUser mod, CaseType caseType, string reason)
        {
            var server = GuildHandler.GetGuild(guild.Id);
            reason = reason ?? $"*Responsible moderator, please type `{server.Prefix}reason {server.Mod.Cases.Count + 1} <reason>`*";
            var modChannel = guild.GetTextChannel(server.Mod.TextChannel);
            if (modChannel == null) return;
            var embed = GetEmbed(CasePaint(caseType))
                .WithAuthor($"Case #{server.Mod.Cases.Count + 1} | {caseType} | {user.Username}#{user.Discriminator}", user.GetAvatarUrl())
                .AddField("User", user.Mention, true)
                .AddField("Moderator", mod.Mention, true)
                .AddField("Reason", reason)
                .WithFooter($"ID: {user.Id}")
                .WithCurrentTimestamp()
                .Build();
            var message = await modChannel.SendMessageAsync(string.Empty, embed: embed);
            server.Mod.Cases.Add(new CaseWrapper
            {
                UserId = user.Id,
                ModId = mod.Id,
                Reason = reason,
                CaseType = caseType,
                MessageId = message.Id,
                CaseNumber = server.Mod.Cases.Count + 1
            });
            GuildHandler.Save(server);
        }

        public Task Purge(IEnumerable<IUserMessage> messages, ITextChannel channel, int amount)
            => amount <= 100 ? channel.DeleteMessagesAsync(messages) :
                Task.Run(() =>
                {
                    for (int i = 0; i < messages.ToArray().Length; i += 100) channel.DeleteMessagesAsync(messages.Skip(i).Take(100));
                });


        public (bool, ulong) GetChannelId(SocketGuild guild, string channel)
        {
            if (string.IsNullOrWhiteSpace(channel)) return (true, 0);
            UInt64.TryParse(channel.Replace('<', ' ').Replace('>', ' ').Replace('#', ' ').Replace(" ", ""), out ulong id);
            var getChannel = guild.GetTextChannel(id);
            if (getChannel != null) return (true, id);
            var findChannel = guild.TextChannels.FirstOrDefault(x => x.Name == channel.ToLower());
            if (findChannel != null) return (true, findChannel.Id);
            return (false, 0);
        }

        public (bool, ulong) GetRoleId(SocketGuild guild, string role)
        {
            if (string.IsNullOrWhiteSpace(role)) return (true, 0);
            UInt64.TryParse(role.Replace('<', ' ').Replace('>', ' ').Replace('@', ' ').Replace('&', ' ').Replace(" ", ""), out ulong id);
            var getRole = guild.GetRole(id);
            if (getRole != null) return (true, id);
            var findRole = guild.Roles.FirstOrDefault(x => x.Name.ToLower() == role.ToLower());
            if (findRole != null) return (true, findRole.Id);
            return (false, 0);
        }

        public bool InviteMatch(string message) => CheckMatch(InviteRegex).Match(message).Success;

        public bool ProfanityMatch(string message) => CheckMatch(ProfanityRegex).Match(message).Success;

        public Regex CheckMatch(string pattern) => new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public (bool, string) ListCheck<T>(List<T> collection, object value, string objectName, string collectionName)
        {
            var check = collection.Contains((T)value);
            if (check) return (false, $"`{objectName}` already exists in {collectionName}.");
            if (collection.Count == collection.Capacity) return (false, $"Reached max number of entries.");
            return (true, $"`{objectName}` has been added to {collectionName}.");
        }

        public Paint CasePaint(CaseType type)
        {
            switch (type)
            {
                case CaseType.Warning:
                    return Paint.Yellow;
                default:
                    return Paint.Temporary;
            }
        }
    }
}
