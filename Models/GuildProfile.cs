using System;
using System.Collections.Generic;

namespace Hifumi.Models
{
    public class GuildProfile
    {
        public int ChatXP { get; set; }
        public int Warnings { get; set; }
        public bool IsBlacklisted { get; set; }
        public DateTime? LastMessage { get; set; }
        public Dictionary<string, DateTime> Commands { get; set; } = new Dictionary<string, DateTime>();
    }
}
