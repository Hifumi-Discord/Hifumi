using Hifumi.Enums;
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
        public List<string> SelfRoles { get; set; } = new List<string>(10);
        public XPWrapper ChatXP { get; set; } = new XPWrapper();
        public ModWrapper Mod { get; set; } = new ModWrapper();
        public List<TagWrapper> Tags { get; set; } = new List<TagWrapper>();
        public Dictionary<string, AFKWrapper> AFK { get; set; } = new Dictionary<string, AFKWrapper>();
        public string JoinChannel { get; set; }
        public string LeaveChannel { get; set; }
        public Locale Locale { get; set; } = Locale.En;
        public Dictionary<string, UserProfile> Profiles { get; set; } = new Dictionary<string, UserProfile>();
    }
}
