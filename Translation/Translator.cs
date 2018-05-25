using Hifumi.Models;
using Hifumi.Personality;
using Hifumi.Translation.Models;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Text;
using static Hifumi.Personality.Emotes;

namespace Hifumi.Translation
{
    public class Translator
    {
        static string translationPath = "../translation";

        public static string GetTranslation(string key, string lang)
        {
            string data = JToken.Parse(File.ReadAllText($"./Translation/json/{lang}.json"))[key].ToString();
            return data;
        }

        #region Help related
        public static CommandModel GetCommand(string command, Locale locale)
        {
            JToken data;
            try
            {
                data = JToken.Parse(File.ReadAllText($"{translationPath}/{locale.ToString().ToLower()}/commands.json"))[command];
            }
            catch
            {
                data = JToken.Parse(File.ReadAllText($"{translationPath}/en/commands.json"))[command].ToString();
            }
            return data.ToObject<CommandModel>();
        }

        public static string GetModule(string moduleName, Locale locale)
        {
            string data;
            try
            {
                data = JToken.Parse(File.ReadAllText($"{translationPath}/{locale.ToString().ToLower()}/commands.json"))[moduleName].ToString();
            }
            catch
            {
                data = JToken.Parse(File.ReadAllText($"{translationPath}/en/commands.json"))[moduleName].ToString();
            }
            return data;
        }
        #endregion

        #region Settings
        public static string AdminValues(string key, Locale locale)
        {
            JToken data;
            try
            {
                data = JToken.Parse(File.ReadAllText($"{translationPath}/{locale.ToString().ToLower()}/administration.json"))[key];
            }
            catch
            {
                data = JToken.Parse(File.ReadAllText($"{translationPath}/en/administration.json"))[key];
            }
            return data.ToString();
        }
        #endregion

        #region Various
        public static string Collection(string key, Locale locale)
        {
            return null;
        }
        #endregion

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
