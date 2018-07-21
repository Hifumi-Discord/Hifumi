using System.Net.Http;
using System.Threading.Tasks;

namespace Hifumi.GameApi.Helpers
{
    public class HttpHelper
    {
        public static async Task<string> GetContentAsync(string url)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url).ConfigureAwait(false);
            var content = response.Content;
            var data = await content.ReadAsStringAsync().ConfigureAwait(false);
            client.Dispose();
            response.Dispose();
            content.Dispose();
            return data;
        }

        public static HttpClient Client = new HttpClient();
    }
}
