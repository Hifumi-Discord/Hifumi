using System.Text;

namespace Hifumi.Helpers
{
    public class StringHelper
    {
        public static string Replace(string message, string guild = null, string user = null, int level = 0, int credits = 0)
        {
            StringBuilder builder = new StringBuilder(message);
            builder.Replace("{guild}", guild);
            builder.Replace("{user}", user);
            builder.Replace("{level}", $"{level}");
            builder.Replace("{credits}", $"{credits}");
            return builder.ToString();
        }
    }
}
