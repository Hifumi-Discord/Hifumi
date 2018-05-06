using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Hifumi.Models
{
    public class DatabaseModel
    {
        public string DatabaseName { get; set; } = "Hifumi";

        public string[] DatabaseUrls { get; set; } = { "http://localhost:8080" };

        [JsonProperty("X509CertificatePath")]
        public string CertificatePath { get; set; }

        public string BackupLocation
        {
            get => Directory.Exists($"{Directory.GetCurrentDirectory()}/backup") ? $"{Directory.GetCurrentDirectory()}/backup"
                : Directory.CreateDirectory($"{Directory.GetCurrentDirectory()}/backup").FullName;
        }

        [JsonIgnore]
        public X509Certificate2 Certificate { get => !string.IsNullOrWhiteSpace(CertificatePath) ? new X509Certificate2(CertificatePath) : null; }
    }
}
