using Newtonsoft.Json;

namespace Hifumi.Translation.Models
{
    public class CommandModel
    {
        [JsonProperty("summary")]
        public string Summary { get; set; }
        [JsonProperty("usage")]
        public string Usage { get; set; }
    }
}
