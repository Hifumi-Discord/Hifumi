using Newtonsoft.Json;

namespace Hifumi.Models
{
    public partial class GitHubModel
    {
        [JsonProperty("commit")]
        public GitHubModel Commit { get; set; }

        [JsonProperty("html_url")]
        public string HtmlUrl { get; set; }

        [JsonProperty("sha")]
        public string Sha { get; set; }
    }

    public partial class GitHubModel
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
