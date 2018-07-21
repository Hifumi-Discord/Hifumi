using Discord;
using Discord.WebSocket;
using Hifumi.Handlers;
using Hifumi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hifumi.Helpers
{
    public class GuildHelper
    {
        GuildHandler GuildHandler {get;}
        DiscordSocketClient Client {get;}

        public GuildHelper(GuildHandler guildHandler, DiscordSocketClient client)
        {
            Client = client;
            GuildHandler = guildHandler;
        }

        public IMessageChannel DefaultChannel(ulong guildId)
        {
            var guild = Client.GetGuild(guildId);
            return guild.TextChannels.FirstOrDefault(x => x.Name.Contains("general") || x.Name.Contains("chat") || x.Id == guildId) ?? guild.DefaultChannel;
        }

        public UserProfile GetProfile(ulong guildId, ulong userId)
        {
            var guild = GuildHandler.GetGuild(Client.GetGuild(guildId).Id);
            if (!guild.Profiles.ContainsKey(userId))
            {
                guild.Profiles.Add(userId, new UserProfile());
                GuildHandler.Save(guild);
            }
            return guild.Profiles[userId];
        }

        public void SaveProfile(ulong guildId, ulong userId, UserProfile profile)
        {
            var config = GuildHandler.GetGuild(guildId);
            config.Profiles[userId] = profile;
            GuildHandler.Save(config);
        }
    }
}
