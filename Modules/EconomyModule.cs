using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Hifumi.Addons;
using static Hifumi.Addons.Embeds;
using Hifumi.Personality;
using static Hifumi.Personality.Emotes;
using Hifumi.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hifumi.Modules
{
    [Name("economy"), RequireBotPermission(ChannelPermission.SendMessages)]
    public class EconomyModule : Base
    {
        [Command("daily"), Summary("Get your daily credits")]
        public Task Daily()
        {
            var profile = Context.GuildHelper.GetProfile(Context.Guild.Id, Context.User.Id);
            int creditsEarned;
            if (profile.DailyReward.HasValue)
            {
                var oldTime = profile.DailyReward.Value;
                var passed = Context.MethodHelper.EasternTime - oldTime;
                if (passed.TotalHours > 48)
                {
                    profile.DailyReward += TimeSpan.FromDays(passed.Days);
                    profile.DailyStreak = 0;
                    profile.Credits += creditsEarned = 200;
                }
                else if (passed.TotalHours > 24)
                {
                    profile.DailyReward += TimeSpan.FromDays(1);
                    profile.DailyStreak++;
                    profile.Credits += creditsEarned = 200 * profile.DailyStreak;
                }
                else
                {
                    var wait = TimeSpan.FromHours(24).Subtract(passed);
                    string hour = string.Empty;
                    string min = string.Empty;
                    string sec = string.Empty;
                    if (wait.Hours != 0)
                    {
                        if (wait.Hours > 1) hour = $"{wait.Hours} hours,";
                        else hour = $"{wait.Hours} hour,";
                    }
                    if (wait.Minutes != 0)
                    {
                        if (wait.Minutes > 1) min = $"{wait.Minutes} minutes,";
                        else min = $"{wait.Minutes} minute,";
                    }
                    if (wait.Seconds != 0)
                    {
                        if (wait.Seconds > 1) sec = $"{wait.Seconds} seconds";
                        else sec = $"{wait.Seconds} second";
                    }
                    return ReplyAsync($"Hold on {GetEmote(EmoteType.Worried)} Credits refresh in {hour} {min} {sec}.");
                }
            }
            else
            {
                profile.DailyReward = Context.MethodHelper.EasternTime;
                profile.Credits += creditsEarned = 200;
            }

            Context.GuildHelper.SaveProfile(Context.Guild.Id, Context.User.Id, profile);
            return ReplyAsync($"You recieved {creditsEarned} daily credits.");
        }

        [Command("rank"), Summary("Shows a user's server's rank.")]
        public Task Rank(SocketGuildUser user = null)
        {
            user = user ?? Context.User as SocketGuildUser;
            var profile = Context.GuildHelper.GetProfile(Context.Guild.Id, user.Id);
            var embed = GetEmbed(Paint.Temporary)
                .WithAuthor($"{user.Username} server rank", user.GetAvatarUrl())
                .WithThumbnailUrl(user.Guild.IconUrl)
                .AddField("Rank", $"{IntHelper.GetGuildRank(Context, user.Id)} / {Context.Server.Profiles.Count}", true)
                .AddField("Level", IntHelper.GetLevel(profile.ChatXP), true)
                .AddField("Current XP", $"{profile.ChatXP} XP", true)
                .AddField("Next Level XP", IntHelper.NextLevelXP(IntHelper.GetLevel(profile.ChatXP)), true)
                .Build();
            return ReplyAsync(string.Empty, embed);
        }

        [Command("top"), Summary("Shows the server scoreboard")]
        public Task Top()
        {
            if (!Context.Server.Profiles.Any() || !Context.Server.ChatXP.IsEnabled) return ReplyAsync($"Leaderboard for {Context.Guild} is empty.");
            var embed = GetEmbed(Paint.Temporary)
                .WithTitle($"Leaderboard for {Context.Guild}");

            var ordered = Context.Server.Profiles.OrderByDescending(x => x.Value.ChatXP).Where(y => y.Value.ChatXP != 0).Take(10).ToList();
            if (ordered.Count > 3)
            {
                embed.AddField($"🥇: {StringHelper.CheckUser(Context.Client, ordered[0].Key)}", $"**Total XP:** {ordered[0].Value.ChatXP}");
                embed.AddField($"🥈: {StringHelper.CheckUser(Context.Client, ordered[1].Key)}", $"**Total XP:** {ordered[1].Value.ChatXP}");
                embed.AddField($"🥉: {StringHelper.CheckUser(Context.Client, ordered[2].Key)}", $"**Total XP:** {ordered[2].Value.ChatXP}");
                for (int i = 3; i < ordered.Count; i++)
                    embed.AddField($"[{i + 1}] {StringHelper.CheckUser(Context.Client, ordered[i].Key)}", $"**Total XP:** {ordered[i].Value.ChatXP}");
                embed.WithFooter($"Rank: {IntHelper.GetGuildRank(Context, Context.User.Id)} Total XP: {Context.GuildHelper.GetProfile(Context.Guild.Id, Context.User.Id).ChatXP}", Context.User.GetAvatarUrl());
            }
            else
                foreach (var rank in ordered)
                    embed.AddField($"{StringHelper.CheckUser(Context.Client, rank.Key)}", $"**Total XP:** {rank.Value.ChatXP}");

            return ReplyAsync(string.Empty, embed.Build());
        }
    }
}
