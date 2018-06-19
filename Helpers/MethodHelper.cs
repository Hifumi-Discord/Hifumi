using System.Net.Http;

namespace Hifumi.Helpers
{
    public class MethodHelper
    {
        HttpClient HttpClient { get; }

        public MethodHelper(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }
    }
}
