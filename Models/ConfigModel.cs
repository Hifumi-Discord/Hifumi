using System.Collections.Generic;

namespace Hifumi.Models
{
    public class ConfigModel
    {
        public string Id { get; set; }
        public string Token { get; set; }
        public string Prefix { get; set; }
        public List<string> UserBlacklist { get; set; } = new List<string>();
        public List<string> ServerBlacklist { get; set; } = new List<string>();
        public Dictionary<string, string> APIKeys { get; set; } = new Dictionary<string, string>();
    }
}
