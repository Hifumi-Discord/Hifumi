using Hifumi.Models;
using Hifumi.Personality;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Text;
using static Hifumi.Personality.Emotes;

namespace Hifumi.Translation
{
    public class Translator
    {
        public static string GetTranslation(string key, string lang)
        {
            string data = JToken.Parse(File.ReadAllText($"./Translation/json/{lang}.json"))[key].ToString();
            return data;
        }

        public static string GetCommand(string command, Locale locale)
        {
            // FIXME: Temporary until everything else is done
            string data;
            try
            {
                data = JToken.Parse(File.ReadAllText($"./Translation/json/{locale.ToString().ToLower()}/commands.json"))[command].ToString();
            }
            catch
            {
                data = command;
            }
            return StringHelper(data);
        }

        static string StringHelper(string message)
        {
            StringBuilder builder = new StringBuilder(message);
            #region Emotes
            builder.Replace("{{happyemote}}", GetEmote(EmoteType.Happy));
            builder.Replace("{{loveemote}}", GetEmote(EmoteType.Love));
            builder.Replace("{{sademote}}", GetEmote(EmoteType.Sad));
            builder.Replace("{{worriedemote}}", GetEmote(EmoteType.Worried));
            #endregion
            return builder.ToString();
        }
    }
}
