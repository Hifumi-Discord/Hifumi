using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Hifumi.Addons;
using Hifumi.Addons.Preconditions;
using Hifumi.Enums;
using Hifumi.Helpers;
using Hifumi.Models;
using Hifumi.Translation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Hifumi.Addons.Embeds;

namespace Hifumi.Modules
{
    [Name("adminmodule"), RequirePermission(AccessLevel.Administrator), RequireBotPermission(ChannelPermission.SendMessages)]
    public class AdminModule : Base
    {
        [Command("blockxp")]
        public Task BlockXP(AdminCollectionAction action, [Remainder] IRole role)
        {
            if (role == Context.Guild.EveryoneRole) return ReplyAsync(Translator.AdminValues("everyone", Context.Server.Locale));
            var check = Context.GuildHelper.ListCheck(Context.Server.ChatXP.XPBlockedRoles, role.Id, role.Name, "XP-blocked roles");
            switch (action)
            {
                case AdminCollectionAction.Add:
                    if (!check.Item1) return ReplyAsync(check.Item2);
                    Context.Server.ChatXP.XPBlockedRoles.Add(role.Id);
                    return ReplyAsync(check.Item2, document: DocumentType.Server);
                case AdminCollectionAction.Remove:
                    if (!Context.Server.ChatXP.XPBlockedRoles.Contains(role.Id)) return ReplyAsync($"{role} isn't blocked from gaining XP.");
                    Context.Server.ChatXP.XPBlockedRoles.Remove(role.Id);
                    return ReplyAsync($"`{role}` has been removed from the XP-blocked roles.", document: DocumentType.Server);
            }
            return Task.CompletedTask;
        }

        [Command("blockxp")]
        public Task BlockXP()
        {
            if (!Context.Server.ChatXP.XPBlockedRoles.Any())
                return ReplyAsync($"{Context.Guild} has no XP blocked roles.");
            string message = $"**__XP Blocked Roles__**";
            foreach (var role in Context.Server.ChatXP.XPBlockedRoles)
                message += $"\n{StringHelper.CheckRole(Context.Guild as SocketGuild, role)}";
            return ReplyAsync(message);
        }

        [Command("export"), Summary("Exports your server settings as a json file.")]
        public async Task ExportAsync()
        {
            var owner = await (Context.Guild as SocketGuild).Owner.GetOrCreateDMChannelAsync();
            if (Context.Guild.OwnerId != Context.User.Id)
            {
                await ReplyAsync(Translator.AdminValues("require_owner", Context.Server.Locale));
                return;
            }
            var serialize = JsonConvert.SerializeObject(Context.Server, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include
            });
            await owner.SendFileAsync(new MemoryStream(Encoding.Unicode.GetBytes(serialize)), $"{Context.Guild.Id}-config.json");
        }

        [Command("joinmessage")]
        public Task JoinMessage(AdminCollectionAction action, [Remainder] string message = null)
        {
            var check = Context.GuildHelper.ListCheck(Context.Server.JoinMessages, message, $"```{message}```", "join messages");
            switch (action)
            {
                case AdminCollectionAction.Add:
                    if (!check.Item1 || message == null) return ReplyAsync(check.Item2);
                    Context.Server.JoinMessages.Add(message);
                    return ReplyAsync("Join message has been added.", document: DocumentType.Server);
                case AdminCollectionAction.Remove:
                    return RemoveMenu(Context.Server.JoinMessages, "join message");
            }
            return Task.CompletedTask;
        }

        [Command("joinmessage")]
        public Task JoinMessage()
        {
            if (!Context.Server.JoinMessages.Any())
                return ReplyAsync($"{Context.Guild} doesn't have any user join messages.");
            string ret = $"**__Join Messages__**";
            foreach (string message in Context.Server.JoinMessages)
                ret += $"\n-> {message}";
            return ReplyAsync(ret);
        }

        [Command("leavemessage")]
        public Task LeaveMessage(AdminCollectionAction action, [Remainder] string message = null)
        {
            var check = Context.GuildHelper.ListCheck(Context.Server.LeaveMessages, message, $"```{message}```", "leave messages");
            switch (action)
            {
                case AdminCollectionAction.Add:
                    if (!check.Item1 || message == null) return ReplyAsync(check.Item2);
                    Context.Server.LeaveMessages.Add(message);
                    return ReplyAsync("Leave message has been added.", document: DocumentType.Server);
                case AdminCollectionAction.Remove:
                    return RemoveMenu(Context.Server.LeaveMessages, "leave message");
            }
            return Task.CompletedTask;
        }

        [Command("leavemessage")]
        public Task LeaveMessage()
        {
            if (!Context.Server.LeaveMessages.Any())
                return ReplyAsync($"{Context.Guild} doesn't have any user leave messages.");
            string ret = $"**__Leave Messages__**";
            foreach (string message in Context.Server.LeaveMessages)
                ret += $"\n-> {message}";
            return ReplyAsync(ret);
        }

        [Command("levelrole")]
        public Task LevelRole(AdminCollectionAction action, IRole role, int level = 10)
        {
            if (role == Context.Guild.EveryoneRole) return ReplyAsync(Translator.AdminValues("everyone", Context.Server.Locale));
            if (role.IsManaged) return ReplyAsync("Can't assign managed roles.");
            switch (action)
            {
                case AdminCollectionAction.Add:
                    if (Context.Server.ChatXP.LeveledRoles.Count == 20) return ReplyAsync("You have reached the max number of leveled roles.");
                    else if (Context.Server.ChatXP.LeveledRoles.ContainsKey(role.Id)) return ReplyAsync($"{role} is already a leveled role.");
                    Context.Server.ChatXP.LeveledRoles.Add(role.Id, level);
                    return ReplyAsync($"Added `{role}` role as a leveled role.", document: DocumentType.Server);
                case AdminCollectionAction.Remove:
                    if (!Context.Server.ChatXP.LeveledRoles.ContainsKey(role.Id)) return ReplyAsync($"{role} isn't a leveled role.");
                    Context.Server.ChatXP.LeveledRoles.Remove(role.Id);
                    return ReplyAsync($"Removed `{role}` role from leveled roles.", document: DocumentType.Server);
                case AdminCollectionAction.Modify:
                    if (!Context.Server.ChatXP.LeveledRoles.ContainsKey(role.Id)) return ReplyAsync($"{role} isn't a leveled role.");
                    Context.Server.ChatXP.LeveledRoles[role.Id] = level;
                    return ReplyAsync($"Modified leveled role `{role}`.", document: DocumentType.Server);

            }
            return Task.CompletedTask;
        }

        [Command("levelrole")]
        public Task LevelRole()
        {
            if (!Context.Server.ChatXP.LeveledRoles.Any())
                return ReplyAsync($"{Context.Guild} has no leveled roles.");
            string message = "**__Leveled Roles__**";
            foreach (var role in Context.Server.ChatXP.LeveledRoles.Keys)
                message += $"\n{Context.Server.ChatXP.LeveledRoles[role]} | {StringHelper.CheckRole(Context.Guild as SocketGuild, role)}";
            return ReplyAsync(message);
        }

        [Command("messagelog")]
        public Task MessageLog(int old = -1)
        {
            if (!Context.Server.DeletedMessages.Any())
                return ReplyAsync("Failed to retrieve deleted messages.");
            var getMessage = old < 0 ? Context.Server.DeletedMessages.LastOrDefault() : Context.Server.DeletedMessages[old];
            var user = StringHelper.CheckUser(Context.Client, getMessage.AuthorId);
            var getUser = (Context.Client as DiscordSocketClient).GetUser(getMessage.AuthorId);
            var embed = GetEmbed(Paint.Temporary)
                .WithAuthor($"{user} - {getMessage.DateTime}", getUser != null ? getUser.GetAvatarUrl() : Context.Client.CurrentUser.GetAvatarUrl())
                .WithDescription(getMessage.Content)
                .WithFooter($"Channel: {StringHelper.CheckChannel(Context.Guild as SocketGuild, getMessage.ChannelId)} | Message Id: {getMessage.MessageId}")
                .Build();
            return ReplyAsync(string.Empty, embed);
        }

        [Command("messagelog")]
        public Task MessageLog(SocketGuildUser user = null, int recent = -1)
        {
            user = user ?? Context.User as SocketGuildUser;
            if (!Context.Server.DeletedMessages.Any(x => x.AuthorId == user.Id)) return ReplyAsync($"Couldn't find any deleted messages from {user.Username}.");
            var getMessage = recent < 0 ? Context.Server.DeletedMessages.Where(x => x.AuthorId == user.Id).LastOrDefault()
                : Context.Server.DeletedMessages.Where(x => x.AuthorId == user.Id).ToList()[recent];
            var getUser = (Context.Client as DiscordSocketClient).GetUser(getMessage.AuthorId);
            var embed = GetEmbed(Paint.Temporary)
                .WithAuthor($"{user} - {getMessage.DateTime}", getUser != null ? getUser.GetAvatarUrl() : Context.Client.CurrentUser.GetAvatarUrl())
                .WithDescription(getMessage.Content)
                .WithFooter($"Channel: {StringHelper.CheckChannel(Context.Guild as SocketGuild, getMessage.ChannelId)} | Message Id: {getMessage.MessageId}")
                .Build();
            return ReplyAsync(string.Empty, embed);
        }

        [Command("reset")]
        public async Task Reset()
        {
            if (Context.Guild.OwnerId != Context.User.Id)
            {
                await ReplyAsync(Translator.AdminValues("require_owner", Context.Server.Locale));
                return;
            }
            string pin = StringHelper.PinCode();
            var msg = await ReplyAsync($"**WARNING!**\nThis will completely wipe all settings, chat xp, and user profiles for this server!\n*If you are sure about this type* `{pin}` (Anything else will cancel)");
            var response = await ResponseWaitAsync(timeout: TimeSpan.FromSeconds(10));
            await msg.DeleteAsync();
            if (response == null || response.Content != pin) return;

            var properties = Context.Server.GetType().GetProperties();
            foreach (var property in properties.Where(x => x.Name != "Id" && x.Name != "Prefix"))
            {
                if (property.PropertyType == typeof(bool)) property.SetValue(Context.Server, false);
                if (property.PropertyType == typeof(string)) property.SetValue(Context.Server, null);
                if (property.PropertyType == typeof(List<string>)) property.SetValue(Context.Server, new List<string>());
                if (property.PropertyType == typeof(List<ulong>)) property.SetValue(Context.Server, new List<ulong>());
                if (property.PropertyType == typeof(XPWrapper)) property.SetValue(Context.Server, new XPWrapper());
                if (property.PropertyType == typeof(ModWrapper)) property.SetValue(Context.Server, new ModWrapper());
                if (property.PropertyType == typeof(RedditWrapper)) property.SetValue(Context.Server, new RedditWrapper());
                if (property.PropertyType == typeof(List<TagWrapper>)) property.SetValue(Context.Server, new List<TagWrapper>());
                if (property.PropertyType == typeof(Dictionary<ulong, string>)) property.SetValue(Context.Server, new Dictionary<ulong, string>());
                if (property.PropertyType == typeof(List<MessageWrapper>)) property.SetValue(Context.Server, new List<MessageWrapper>());
                if (property.PropertyType == typeof(Dictionary<ulong, UserProfile>)) property.SetValue(Context.Server, new Dictionary<ulong, UserProfile>());
            }
            await ReplyAsync("Guild config has been recreated.", document: DocumentType.Server);
        }

        [Command("selfrolelist")]
        public Task SelfRolesAsync(AdminCollectionAction action, [Remainder] IRole role)
        {
            if (role == Context.Guild.EveryoneRole) return ReplyAsync("Role can't be everyone role.");
            if (role.IsManaged) return ReplyAsync("Can't assign managed roles.");
            var check = Context.GuildHelper.ListCheck(Context.Server.SelfRoles, role.Id, role.Name, "assignable roles");
            switch (action)
            {
                case AdminCollectionAction.Add:
                    if (!check.Item1) return ReplyAsync(check.Item2);
                    Context.Server.SelfRoles.Add(role.Id);
                    return ReplyAsync(check.Item2, document: DocumentType.Server);
                case AdminCollectionAction.Remove:
                    if (!Context.Server.SelfRoles.Contains(role.Id)) return ReplyAsync($"{role.Name} isn't an assignable role.");
                    Context.Server.SelfRoles.Remove(role.Id);
                    return ReplyAsync($"`{role.Name}` is no longer an assignable role.", document: DocumentType.Server);
            }
            return Task.CompletedTask;
        }

        [Command("setting"), Alias("set")]
        public Task SettingAsync(SettingType setting, [Remainder] string value = null)
        {
            value = value ?? string.Empty;
            var intCheck = int.TryParse(value, out int result);
            var channelCheck = Context.GuildHelper.GetChannelId(Context.Guild as SocketGuild, value);
            var roleCheck = Context.GuildHelper.GetRoleId(Context.Guild as SocketGuild, value);
            switch (setting)
            {
                case SettingType.Prefix:
                    Context.Server.Prefix = value;
                    break;
                case SettingType.JoinChannel:
                    if (!channelCheck.Item1) return ReplyAsync($"{setting} value was incorrect. Try mentioning the channel?");
                    Context.Server.JoinChannel = channelCheck.Item2;
                    break;
                case SettingType.LeaveChannel:
                    if (!channelCheck.Item1) return ReplyAsync($"{setting} value was incorrect. Try mentioning the channel?");
                    Context.Server.LeaveChannel = channelCheck.Item2;
                    break;
                case SettingType.ModChannel:
                    if (!channelCheck.Item1) return ReplyAsync($"{setting} value was incorrect. Try mentioning the channel?");
                    Context.Server.Mod.TextChannel = channelCheck.Item2;
                    break;
                case SettingType.RedditChannel:
                    if (!channelCheck.Item1) return ReplyAsync($"{setting} value was incorrect. Try mentioning the channel?");
                    Context.Server.Reddit.TextChannel = channelCheck.Item2;
                    break;
                case SettingType.JoinRole:
                    if (!roleCheck.Item1) return ReplyAsync($"{setting} value was incorrect. Try mentioning the role?");
                    Context.Server.Mod.JoinRole = roleCheck.Item2;
                    break;
                case SettingType.MuteRole:
                    if (!roleCheck.Item1) return ReplyAsync($"{setting} value was incorrect. Try mentioning the role?");
                    Context.Server.Mod.MuteRole = roleCheck.Item2;
                    break;
                case SettingType.MaxWarnings:
                    if (!intCheck) return ReplyAsync($"{setting} value was incorrect. Value must be an integer.");
                    Context.Server.Mod.MaxWarnings = (result > 0) ? result : 0;
                    break;
                case SettingType.LevelUpMessage:
                    Context.Server.ChatXP.LevelMessage = value;
                    break;
                case SettingType.Locale:
                    Enum.TryParse(value, true, out Locale l);
                    if (!Enum.IsDefined(typeof(Locale), l)) return ReplyAsync("Invalid locale");
                    Context.Server.Locale = l;
                    break;
            }
            return ReplyAsync($"{setting} has been updated.", document: DocumentType.Server);
        }

        [Command("setting"), Alias("set")]
        public Task SettingAsync()
        {
            string xp = Context.Server.ChatXP.IsEnabled ? "Enabled." : "Disabled.";
            string feed = Context.Server.Reddit.IsEnabled ? "Enabled." : "Disabled.";
            string antiInvite = Context.Server.Mod.AntiInvite ? "Enabled." : "Disabled.";
            string antiProfanity = Context.Server.Mod.AntiProfanity ? "Enabled." : "Disabled.";
            string messageLog = Context.Server.Mod.LogDeletedMessages ? "Enabled." : "Disabled.";

            var embed = GetEmbed(Paint.Temporary)
                .WithAuthor($"{Context.Guild.Name} Settings", Context.Guild.IconUrl)
                .AddField("General Information", "```ebnf\n" +
                    $"Prefix                : {Context.Server.Prefix}\n" +
                    $"Log Channel           : {StringHelper.CheckChannel(Context.Guild as SocketGuild, Context.Server.Mod.TextChannel)}\n" +
                    $"Join Channel          : {StringHelper.CheckChannel(Context.Guild as SocketGuild, Context.Server.JoinChannel)}\n" +
                    $"Leave Channel         : {StringHelper.CheckChannel(Context.Guild as SocketGuild, Context.Server.LeaveChannel)}\n" +
                    $"Reddit Channel        : {StringHelper.CheckChannel(Context.Guild as SocketGuild, Context.Server.Reddit.TextChannel)}\n" +
                    $"Join Messages         : {Context.Server.JoinMessages.Count}\n" +
                    $"Leave Messages        : {Context.Server.LeaveMessages.Count}\n" +
                    $"AFK Users             : {Context.Server.AFK.Count}\n" +
                    $"Self Roles            : {Context.Server.SelfRoles.Count}```", false)
                .AddField("Admin Information", "```diff\n" +
                    $"+ Reddit Feed         : {feed}\n" +
                    $"+ Chat XP             : {xp}\n" +
                    $"+ Join Role           : {StringHelper.CheckRole(Context.Guild as SocketGuild, Context.Server.Mod.JoinRole)}\n" +
                    $"+ Mute Role           : {StringHelper.CheckRole(Context.Guild as SocketGuild, Context.Server.Mod.MuteRole)}\n" +
                    $"+ Subreddits          : {Context.Server.Reddit.Subreddits.Count}\n" +
                    $"+ Profanity Check     : {antiProfanity}\n" +
                    $"+ Invite Check        : {antiInvite}\n" +
                    $"+ Deleted Message Log : {messageLog}\n" +
                    $"+ Max Warnings        : {Context.Server.Mod.MaxWarnings}\n" +
                    $"+ Leveled Roles       : {Context.Server.ChatXP.LeveledRoles.Count}\n" +
                    $"+ Blacklisted Users   : {Context.Server.Profiles.Where(x => x.Value.IsBlacklisted).Count()}\n" +
                    $"+ XP-Blocked Roles    : {Context.Server.ChatXP.XPBlockedRoles.Count}```", false)
                .AddField("Guild Statistics", "```diff\n" +
                    //$"- Locale              : {Context.Server.Locale}\n" +
                    $"- Users Banned        : {Context.Server.Mod.Cases.Where(x => x.CaseType == CaseType.Ban).Count()}\n" +
                    $"- Users Kicked        : {Context.Server.Mod.Cases.Where(x => x.CaseType == CaseType.Kick).Count()}\n" +
                    $"- Total Chat XP       : {Context.Server.Profiles.Sum(x => x.Value.ChatXP)}\n" +
                    $"- Total Tags          : {Context.Server.Tags.Count}\n" +
                    $"- Total Credits       : {Context.Server.Profiles.Sum(x => x.Value.Credits)}\n" +
                    $"- Total Mod Cases     : {Context.Server.Mod.Cases.Count}```", false)
                    .Build();
            return ReplyAsync(string.Empty, embed);
        }

        [Command("subreddit")]
        public Task Subreddit(AdminCollectionAction action, string subreddit)
        {
            var check = Context.GuildHelper.ListCheck(Context.Server.Reddit.Subreddits, subreddit, subreddit, "server's subreddits");
            switch (action)
            {
                case AdminCollectionAction.Add:
                    if (!check.Item1) return ReplyAsync(check.Item2);
                    if (Context.RedditService.SubredditAsync(subreddit).Result == null) return ReplyAsync($"{subreddit} is an invalid subreddit.");
                    Context.Server.Reddit.Subreddits.Add(subreddit);
                    return ReplyAsync(check.Item2, document: DocumentType.Server);
                case AdminCollectionAction.Remove:
                    if (!Context.Server.Reddit.Subreddits.Contains(subreddit)) return ReplyAsync($"You aren't subbed to {subreddit}.");
                    Context.Server.Reddit.Subreddits.Remove(subreddit);
                    return ReplyAsync($"Removed {subreddit} from server's subreddits.", document: DocumentType.Server);
            }
            return Task.CompletedTask;
        }

        [Command("subreddit")]
        public Task Subreddit()
            => ReplyAsync(!Context.Server.Reddit.Subreddits.Any() ? "This server isn't subscribed to any subreddits." :
                $"**__Server Subreddits__**\n{string.Join("\n", Context.Server.Reddit.Subreddits)}");

        [Command("toggle"), Summary("Changes server setting values.")]
        public Task Toggle(ToggleType toggle)
        {
            string state = null;
            switch (toggle)
            {
                case ToggleType.ChatXP:
                    Context.Server.ChatXP.IsEnabled = !Context.Server.ChatXP.IsEnabled;
                    state = Context.Server.ChatXP.IsEnabled ? "enabled" : "disabled";
                    break;
                case ToggleType.AntiInvite:
                    Context.Server.Mod.AntiInvite = !Context.Server.Mod.AntiInvite;
                    state = Context.Server.Mod.AntiInvite ? "enabled" : "disabled";
                    break;
                case ToggleType.AntiProfanity:
                    Context.Server.Mod.AntiProfanity = !Context.Server.Mod.AntiProfanity;
                    state = Context.Server.Mod.AntiProfanity ? "enabled" : "disabled";
                    break;
                case ToggleType.MessageLog:
                    Context.Server.Mod.LogDeletedMessages = !Context.Server.Mod.LogDeletedMessages;
                    state = Context.Server.Mod.LogDeletedMessages ? "enabled" : "disabled";
                    break;
                case ToggleType.RedditFeed:
                    if (Context.Server.Reddit.IsEnabled)
                    {
                        Context.Server.Reddit.IsEnabled = false;
                        Context.RedditService.Stop(Context.Server.Reddit.TextChannel);
                        state = "disabled";
                    }
                    else
                    {
                        Context.Server.Reddit.IsEnabled = true;
                        Context.RedditService.Start(Context.Guild.Id);
                        state = "enabled";
                    }
                    break;
            }
            return ReplyAsync($"{toggle} has been {state}.", document: DocumentType.Server);
        }

        private async Task RemoveMenu(List<string> list, string collection)
        {
            if (!list.Any())
            {
                await ReplyAsync($"{Context.Guild} has no {collection}s");
                return;
            }
            string menu = $"Please choose a {collection} to remove:\n```css\n";
            for (int i = 1; i <= list.Count; i++)
                menu += $"[{i}] {list[i - 1]}\n";
            menu += "```";

            var msg = await ReplyAsync(menu);
            var choice = await ResponseWaitAsync(timeout: TimeSpan.FromSeconds(15));
            await msg.DeleteAsync();
            if (choice == null)
            {
                await ReplyAsync($"**{Context.User.Username},** the command window has closed due to inactivity.");
                return;
            }
            int.TryParse(choice.Content, out int index);
            if (!(index > 0 && index <= list.Count))
            {
                await ReplyAsync($"Invalid response.");
                return;
            }
            index--;
            if (collection.Equals("join message"))
                Context.Server.JoinMessages.RemoveAt(index);
            else
                Context.Server.LeaveMessages.RemoveAt(index);
            await ReplyAsync("Message deleted!", document: DocumentType.Server);
        }
    }
}
