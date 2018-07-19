using System;
using System.Collections.Generic;

namespace Hifumi.Models
{
    public class XPWrapper
    {
        public bool IsEnabled { get; set; }
        public string LevelMessage { get; set; }
        public List<ulong> XPBlockedRoles { get; set; } = new List<ulong>(20);
        public Dictionary<ulong, int> LeveledRoles { get; set; } = new Dictionary<ulong, int>();
    }

    public class TagWrapper
    {
        public int Uses { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public string Content { get; set; }
        public bool AutoRespond { get; set; }
        public DateTime CreationTime { get; set; }
    }

    public class ModWrapper
    {
        public ulong JoinRole { get; set; }
        public ulong MuteRole { get; set; }
        public bool AntiInvite { get; set; }
        public int MaxWarnings { get; set; }
        public ulong TextChannel { get; set; }
        public bool AntiProfanity { get; set; }
        public bool LogDeletedMessages { get; set; }
        public List<ulong> MutedUsers { get; set; } = new List<ulong>();
        public List<CaseWrapper> Cases { get; set; } = new List<CaseWrapper>();
    }

    public class CaseWrapper
    {
        public ulong ModId { get; set; }
        public ulong UserId { get; set; }
        public string Reason { get; set; }
        public int CaseNumber { get; set; }
        public ulong MessageId { get; set; }
        public CaseType CaseType { get; set; }
    }

    public enum CaseType
    {
        AutoMod,
        Ban,
        Kick,
        Mute,
        UnBan,
        UnMute,
        Warning
    }

    public class RedditWrapper
    {
        public bool IsEnabled { get; set; }
        public List<string> Subreddits { get; set; } = new List<string>(5);
        public ulong TextChannel { get; set; }
    }

    public class UserProfile
    {
        public int ChatXP { get; set; }
        public int Credits { get; set; }
        public int Warnings { get; set; }
        public int DailyStreak { get; set; }
        public bool IsBlacklisted { get; set; }
        public DateTime? LastMessage { get; set; }
        public DateTime? DailyReward { get; set; }
        public Dictionary<string, DateTime> Commands { get; set; } = new Dictionary<string, DateTime>();
    }

    public class MessageWrapper
    {
        public string Content { get; set; }
        public ulong AuthorId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }
        public DateTime DateTime { get; set; }
    }

    public class AFKWrapper
    {
        public string Message { get; set; }
        public string ImageURL { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
