using Hifumi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace Hifumi.Helpers
{
    public class MethodHelper
    {
        HttpClient HttpClient { get; }
        public MethodHelper(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public List<Type> GetNamespaces(string name)
            => Assembly.GetExecutingAssembly().GetTypes().Where(x => String.Equals(x.Namespace, name, StringComparison.Ordinal)).ToList();

        public DateTime UnixDateTime(double unix)
            => new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unix).ToLocalTime();

        public DateTime EasternTime
            => TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, TimeZoneInfo.Local.Id, "Eastern Standard Time");

        public IEnumerable<Assembly> GetAssemblies
        {
            get
            {
                var assemblies = Assembly.GetEntryAssembly().GetReferencedAssemblies();
                foreach (var a in assemblies) yield return Assembly.Load(a);
                yield return Assembly.GetEntryAssembly();
                yield return typeof(ILookup<string, string>).GetTypeInfo().Assembly;
            }
        }

        public async Task<IReadOnlyCollection<GitHubModel>> GetCommitsAsync()
        {
            HttpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            var request = await HttpClient.GetAsync("https://api.github.com/repos/vic485/hifumi/commits");
            if (!request.IsSuccessStatusCode) return null;
            var content = JsonConvert.DeserializeObject<IReadOnlyCollection<GitHubModel>>(await request.Content.ReadAsStringAsync());
            HttpClient.DefaultRequestHeaders.Clear();
            return content;
        }
    }
}
