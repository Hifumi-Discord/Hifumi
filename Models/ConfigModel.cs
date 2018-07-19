using System.Collections.Generic;

namespace Hifumi.Models
{
    public class ConfigModel
    {
        public string Id { get; set; }
        public string Token { get; set; }
        public string Prefix { get; set; }
        public List<ulong> UserBlacklist { get; set; } = new List<ulong>();
        public List<ulong> ServerBlacklist { get; set; } = new List<ulong>();
        public Dictionary<string, string> APIKeys { get; set; } = new Dictionary<string, string>();
    }
}
