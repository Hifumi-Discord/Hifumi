using Hifumi.Enums;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;

namespace Hifumi.Translation
{
    public class Translator
    {
        static string translationPath = "../translation";

        public static string GetMessage(string key, Locale locale, string user = null, string guild = null)
        {
            string message;
            try
            {
                message = JToken.Parse(File.ReadAllText($"{translationPath}/{locale.ToString().ToLower()}/messages.json"))[key].ToString();
            }
            catch
            {
                message = JToken.Parse(File.ReadAllText($"{translationPath}/en/messages.json"))[key].ToString();
            }
            return Replacements(message, user, guild);
        }

        static string Replacements(string message, string user = null, string guild = null)
        {
            StringBuilder builder = new StringBuilder(message);
            #region Emotes
            // TODO: personality
            #endregion
            #region Discord/Hifumi Stuff
            builder.Replace("{user}", user);
            builder.Replace("{guild}", guild);
            #endregion
            return builder.ToString();
        }
    }
}
