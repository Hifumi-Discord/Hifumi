namespace Hifumi.Models
{
    public class GuildActionModel
    {
        public ulong TextChannel { get; set; }
        public bool AutoRoleAdd { get; set; }
        public bool ChannelCreate { get; set; }
        public bool ChannelDelete { get; set; }
        public bool SelfroleAdd { get; set; }
        public bool SelfroleRemove { get; set; }
        public bool UserJoin { get; set; }
        public bool UserLeave { get; set; }
    }
}
