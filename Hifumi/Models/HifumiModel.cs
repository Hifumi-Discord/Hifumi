using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Hifumi.Models
{
    public class HifumiModel
    {
        public int Shard { get; set; } = 0;
        public int TotalShards { get; set; } = 1;
        public string DatabaseName { get; set; } = "Hifumi";
        public string[] DatabaseUrls { get; set; } = { "http://localhost:8080" };
        [JsonProperty("X509CertificatePath")]
        public string CertificatePath { get; set; }

        [JsonIgnore]
        public X509Certificate2 Certificate { get => !string.IsNullOrWhiteSpace(CertificatePath) ? new X509Certificate2(CertificatePath) : null; }
    }
}
