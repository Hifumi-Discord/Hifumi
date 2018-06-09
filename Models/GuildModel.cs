using System.Collections.Generic;

namespace Hifumi.Models
{
    public class GuildModel
    {
        public string Id { get; set; }
        public string Prefix { get; set; }
        public bool IsConfigured { get; set; }
        public List<string> JoinMessages { get; set; } = new List<string>(5);
        public List<string> LeaveMessages { get; set; } = new List<string>(5);
        public List<ulong> SelfRoles { get; set; } = new List<ulong>(10);
        public XPWrapper ChatXP { get; set; } = new XPWrapper();
        public ModWrapper Mod { get; set; } = new ModWrapper();
        public RedditWrapper Reddit { get; set; } = new RedditWrapper();
        public List<TagWrapper> Tags { get; set; } = new List<TagWrapper>();
        public Dictionary<ulong, AFKWrapper> AFK { get; set; } = new Dictionary<ulong, AFKWrapper>();
        public ulong JoinChannel { get; set; }
        public ulong LeaveChannel { get; set; }
        public Locale Locale { get; set; } = Locale.En;
        public GuildActionModel GuildActions { get; set; } = new GuildActionModel();
        public List<MessageWrapper> DeletedMessages { get; set; } = new List<MessageWrapper>();
        public Dictionary<ulong, UserProfile> Profiles { get; set; } = new Dictionary<ulong, UserProfile>();
    }
}
