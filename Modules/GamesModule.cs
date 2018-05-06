using Discord;
using Discord.Commands;
using Hifumi.Addons;
using static Hifumi.Addons.Embeds;
using Hifumi.Addons.Preconditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hifumi.Modules
{
    [Name("Game Commands"), RequireBotPermission(ChannelPermission.SendMessages)]
    public class GamesModule : Base
    {
        [Command("osu"), RequireCooldown(30), Summary("Show a user's osu stats.")]
        public async Task OsuAsync()
        {
            int mode;
            var msg = await ReplyAsync("Type the number of a game mode:\n" +
                "```css\n" +
                "[1] osu!\n" +
                "[2] osu!taiko\n" +
                "[3] osu!catch the beat\n" +
                "[4] osu!mania```");
            var temp = await ResponseWaitAsync(timeout: TimeSpan.FromSeconds(15));
            await msg.DeleteAsync();
            if (temp != null)
            {
                int.TryParse(temp.Content, out int i);
                if (i > 0 && i < 5) mode = i - 1;
                else
                {
                    await ReplyAsync("Invalid game mode.");
                    return;
                }
            }
            else
            {
                await ReplyAsync($"**{Context.User.Username}**, the window has closed due to inactivity.");
                return;
            }

            int commandMode;
            msg = await ReplyAsync("Type the number of the information you wish to view:\n" +
                "```css\n" +
                "[1] User Signature\n" +
                "```");
            temp = await ResponseWaitAsync(timeout: TimeSpan.FromSeconds(15));
            await msg.DeleteAsync();
            if (temp != null)
            {
                int.TryParse(temp.Content, out int i);
                if (i > 0 && i < 2) commandMode = i;
                else
                {
                    await ReplyAsync("Invalid response.");
                    return;
                }
            }
            else
            {
                await ReplyAsync($"**{Context.User.Username}**, the window has closed due to inactivity.");
                return;
            }

            msg = await ReplyAsync("Type a player's username:");
            temp = await ResponseWaitAsync(timeout: TimeSpan.FromSeconds(15));
            await msg.DeleteAsync();
            if (temp == null)
            {
                await ReplyAsync($"**{Context.User.Username}**, the window has closed due to inactivity.");
                return;
            }

            var user = await Context.ConfigHandler.HGame.Osu.GetUserAsync(temp.Content, (HGame.Osu.GameMode)mode);
            if (user == null)
            {
                await ReplyAsync($"Could not find osu! user {temp.Content}");
                return;
            }
            switch (commandMode)
            {
                case 1:
                    string id = user[0].UserId;
                    var embed = GetEmbed(Paint.Aqua)
                        .WithColor(237, 116, 170) // TODO: need more colors to avoid this override
                        .WithAuthor($"{user[0].UserName}'s osu! Signature", url: $"https://osu.ppy.sh/users/{user[0].UserId}")
                        .WithImageUrl($"https://lemmmy.pw/osusig/sig.php?colour=hexbb1177&uname={user[0].UserName}&mode={mode}&pp=1&removeavmargin&flagstroke&onlineindicator=undefined&xpbar&xpbarhex")
                        .Build();
                    await ReplyAsync(string.Empty, embed);
                    break;
            }
        }

        [Command("steam"), RequireCooldown(15), Summary("Show a user's steam profile.")]
        public async Task SteamAsync(string userId)
        {
            var userInfo = await Context.ConfigHandler.HGame.Steam.GetUsersInfoAsync(new[] { userId }.ToList());
            var userGames = await Context.ConfigHandler.HGame.Steam.OwnedGamesAsync(userId);
            var userRecent = await Context.ConfigHandler.HGame.Steam.RecentGamesAsync(userId);
            var info = userInfo.PlayersInfo.Players.FirstOrDefault();
            string state;
            if (info.ProfileState == 0) state = "Offline";
            else if (info.ProfileState == 1) state = "Online";
            else if (info.ProfileState == 2) state = "Busy";
            else if (info.ProfileState == 3) state = "Away";
            else if (info.ProfileState == 4) state = "Snooze";
            else if (info.ProfileState == 5) state = "Looking to trade";
            else state = "Looking to play";

            var embed = GetEmbed(Paint.Aqua)
                .WithAuthor(info.RealName ?? "Steam", "https://png.icons8.com/material/256/e5e5e5/steam.png", info.ProfileLink)
                .WithThumbnailUrl(info.AvatarFullUrl)
                .AddField("Display Name", $"{info.Name}", true)
                .AddField("Location", $"{info.State ?? "No State"}, {info.Country ?? "No Country"}", true)
                .AddField("Person State", state, true)
                .AddField("Profile Created", Context.MethodHelper.UnixDateTime(info.TimeCreated), true)
                .AddField("Last Online", Context.MethodHelper.UnixDateTime(info.LastLogOff), true)
                .AddField("Primary clan ID", info.PrimaryClanId ?? "None", true)
                .AddField("Owned Games", userGames.OwnedGames.GamesCount, true)
                .AddField("Recently Played Games", userRecent.RecentGames.TotalCount, true);
            await ReplyAsync(string.Empty, embed.Build());
        }

        [Command("wows"), RequireCooldown(30), Summary("Show a user's profile for World of Warships.")]
        public async Task WowsAsync() // TODO: image sharp
        {
            HGame.Wows.Region region;
            var msg = await ReplyAsync("Type the number of a region below:\n" +
                "```css\n" +
                "[1] Russia\n" +
                "[2] Europe\n" +
                "[3] North America\n" +
                "[4] Asia```");
            var temp = await ResponseWaitAsync(timeout: TimeSpan.FromSeconds(15));
            await msg.DeleteAsync();
            if (temp != null)
            {
                int.TryParse(temp.Content, out int i);
                if (i > 0 && i < 5) region = (HGame.Wows.Region)(i - 1);
                else
                {
                    await ReplyAsync("Invalid region");
                    return;
                }
            }
            else
            {
                await ReplyAsync($"**{Context.User.Username}**, the window has closed due to inactivity.");
                return;
            }
            msg = await ReplyAsync("Type the username of a player");
            temp = await ResponseWaitAsync(timeout: TimeSpan.FromSeconds(15));
            await msg.DeleteAsync();
            if (temp == null)
            {
                await ReplyAsync($"**{Context.User.Username}**, the window has closed due to inactivity.");
                return;
            }
            var user = await Context.ConfigHandler.HGame.Wows.GetPlayersAsync(region, new[] { temp.Content });
            if (user == null)
            {
                await ReplyAsync($"Couldn't find a user matching {temp.Content}");
                return;
            }
            var data = await Context.ConfigHandler.HGame.Wows.GetPlayerDataAsync(region, new[] { user.PlayerList[0].AccountId.ToString() });
            var embed = GetEmbed(Paint.Aqua)
                .WithTitle($"{user.PlayerList[0].Nickname}'s Profile")
                .AddField("Wins", data[0].Statistics.PVP.Wins, true)
                .AddField("Losses", data[0].Statistics.PVP.Losses, true)
                .AddField("Total Battles", data[0].Statistics.Battles, true)
                .AddField("XP", data[0].Statistics.PVP.XP)
                .AddField("Distance travelled", $"{data[0].Statistics.Distance} miles", true)
                .AddField("Main battery hits", data[0].Statistics.PVP.MainBattery.Hits, true)
                .AddField("Main battery shots", data[0].Statistics.PVP.MainBattery.Shots, true)
                .AddField("Warships destroyed with Main Battery", data[0].Statistics.PVP.MainBattery.Frags, true)
                .WithFooter($"Last Online: {Context.MethodHelper.UnixDateTime(data[0].LogoutAt).ToString()}")
                .Build();
            await ReplyAsync(string.Empty, embed);
        }
    }
}
