using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Hifumi.Addons;
using static Hifumi.Addons.Embeds;
using Hifumi.Enums;
using Hifumi.Helpers;
using Hifumi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading.Tasks;

namespace Hifumi.Modules
{
    [Name("generalmodule"), RequireBotPermission(ChannelPermission.SendMessages)]
    public class GeneralModule : Base
    {
        [Command("afk"), Summary("Set yourself as afk.")]
        public Task Afk([Remainder] string message = "AFK.")
        {
            if (Context.Server.AFK.ContainsKey(Context.User.Id))
            {
                Context.Server.AFK.Remove(Context.User.Id);
                return ReplyAsync("You are no longer afk.", document: DocumentType.Server);
            }
            else
            {
                Context.Server.AFK.Add(Context.User.Id, message);
                return ReplyAsync($"You are now AFK. If someone mentions you, the following will be sent: {message}", document: DocumentType.Server);
            }
        }

        [Command("ping"), Summary("Check network latency.")]
        public async Task PingAsync()
        {
            var embed = GetEmbed(Paint.Temporary)
                .WithTitle("Hifumi's Latency Information")
                .AddField("Gateway", $"{(Context.Client as DiscordSocketClient).Latency} ms", true)
                .AddField("Network", $"{(await new Ping().SendPingAsync("185.199.108.153")).RoundtripTime} ms", true)
                .WithCurrentTimestamp()
                .Build();
            await ReplyAsync(string.Empty, embed);
        }

        [Command("feedback"), Summary("Give feedback on Hifumi's performance, or submit a quick bug report.")]
        public async Task FeedbackAsync()
        {
            // TODO: Get this working
            await ReplyAsync($"*Please provide your feedback now (1 minute timout).*");
            var response = await ResponseWaitAsync(timeout: TimeSpan.FromSeconds(60));
            if (response == null || response.Content.Length < 20)
            {
                await ReplyAndDeleteAsync($"Hmm, I can't submit a blank feedback. Try again maybe?");
                return;
            }
            var channel = (Context.Client as DiscordSocketClient).GetChannel(Context.Config.ReportChannel) as SocketTextChannel;
            var embed = GetEmbed(Paint.Temporary)
                .WithAuthor($"Feedback from {Context.User}", Context.User.GetAvatarUrl())
                .WithDescription($"**Feedback:**\n{response.Content}")
                .WithFooter($"{Context.Guild} | {Context.Guild.Id}")
                .WithCurrentTimestamp()
                .Build();
            await channel.SendMessageAsync(string.Empty, embed: embed);
            await ReplyAsync($"Thank you for submitting your feedback.");
        }

        [Command("selfroles"), Summary("Get or remove a self assignable role")]
        public Task SelfrolesAsync(CollectionAction action, [Remainder] IRole role = null)
        {
            if (!Context.Server.SelfRoles.Any())
                return ReplyAsync($"{Context.Guild} has no self assignable roles.");
            var user = Context.User as SocketGuildUser;
            if (role != null && Context.Server.SelfRoles.Contains(role.Id))
            {
                switch (action)
                {
                    case CollectionAction.Get:
                        if (user.Roles.Contains(role)) return ReplyAsync($"You already have `{role.Name}`.");
                        return user.AddRoleAsync(role);
                    case CollectionAction.Remove:
                        if (!user.Roles.Contains(role)) return ReplyAsync($"You don't have `{role.Name}`.");
                        return user.RemoveRoleAsync(role);
                }
            }

            switch (action)
            {
                case CollectionAction.Get:
                    return RoleAddMenu();
                case CollectionAction.Remove:
                    return RoleRemoveMenu();
                case CollectionAction.List:
                    if (!Context.Server.SelfRoles.Any()) return ReplyAsync($"There are no self assignable roles.");
                    var message = $"**__Current Self Assignable Roles__**\n";
                    foreach (var r in Context.Server.SelfRoles)
                        message += $"-> {StringHelper.CheckRole(Context.Guild as SocketGuild, r)}\n";
                    return ReplyAsync(message);
            }
            return Task.CompletedTask;
        }

        private async Task RoleAddMenu()
        {
            List<IRole> selfRoles = new List<IRole>();
            foreach (var r in Context.Server.SelfRoles)
                selfRoles.Add(Context.Guild.GetRole(r));

            var roles = selfRoles.Except((Context.User as SocketGuildUser).Roles.ToList()).OrderByDescending(x => x.Position).ToList();
            if (!roles.Any())
            {
                await ReplyAsync($"You have all self assigned roles!");
                return;
            }
            var message = "**List of self roles you don't have:**\n```css\n";
            for (int i = 1; i <= roles.Count; i++)
                message += $"[{i}] {roles[i - 1].Name}\n";
            message += "```\n Type a number from the menu above.";
            var msg = await ReplyAsync(message);
            var response = await ResponseWaitAsync(timeout: TimeSpan.FromSeconds(15));
            await msg.DeleteAsync();
            if (response == null)
            {
                await ReplyAsync($"**{Context.User.Username},** the command window has closed due to inactivity.");
                return;
            }
            int.TryParse(response.Content, out int roleNum);
            if (roleNum == 0 || !(roleNum >= 1 && roleNum <= roles.Count))
            {
                await ReplyAsync("Invalid response");
                return;
            }
            roleNum--;
            await (Context.User as SocketGuildUser).AddRoleAsync(roles[roleNum]);
            await ReplyAsync($"**{Context.User.Username}**, you got {roles[roleNum].Name}!");
        }

        private async Task RoleRemoveMenu()
        {
            List<IRole> selfRoles = new List<IRole>();
            foreach (var r in Context.Server.SelfRoles)
                selfRoles.Add(Context.Guild.GetRole(r));

            var roles = selfRoles.Intersect((Context.User as SocketGuildUser).Roles.ToList()).OrderByDescending(x => x.Position).ToList();
            if (!roles.Any())
            {
                await ReplyAsync($"You have no self assigned roles!");
                return;
            }
            var message = "**List of self roles you have:**\n```css\n";
            for (int i = 1; i <= roles.Count; i++)
                message += $"[{i}] {roles[i - 1].Name}\n";
            message += "```\n Type a number from the menu above.";
            var msg = await ReplyAsync(message);
            var response = await ResponseWaitAsync(timeout: TimeSpan.FromSeconds(15));
            await msg.DeleteAsync();
            if (response == null)
            {
                await ReplyAsync($"**{Context.User.Username},** the command window has closed due to inactivity.");
                return;
            }
            int.TryParse(response.Content, out int roleNum);
            if (roleNum == 0 || !(roleNum >= 1 && roleNum <= roles.Count))
            {
                await ReplyAsync("Invalid response");
                return;
            }
            roleNum--;
            await (Context.User as SocketGuildUser).RemoveRoleAsync(roles[roleNum]);
            await ReplyAsync($"**{Context.User.Username}**, removed {roles[roleNum].Name}!");
        }
    }
}
