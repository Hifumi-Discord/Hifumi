using Hifumi.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;

namespace Hifumi.Translation
{
    public class Translator
    {
        static string translationPath = "../translation";

        public static string GetMessage(string key, Locale locale, string user = null, string guild = null, string command = null, string access = null)
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
            StringBuilder sb = new StringBuilder(message);
            sb.Replace("{user}", user);
            sb.Replace("{guild}", guild);
            sb.Replace("{command}", command);
            sb.Replace("{access}", access);
            return Replacements(sb.ToString());
        }

        public static string GetCooldown(Locale locale, TimeSpan cooldown)
        {
            string message;
            try
            {
                message = JToken.Parse(File.ReadAllText($"{translationPath}/{locale.ToString().ToLower()}/usererrors.json"))["cool-down"].ToString();
            }
            catch
            {
                message = JToken.Parse(File.ReadAllText($"{translationPath}/en/usererrors.json"))["cool-down"].ToString();
            }
            StringBuilder sb = new StringBuilder(message);
            sb.Replace("{minute}", $"{cooldown.Minutes}");
            sb.Replace("{second}", $"{cooldown.Seconds}");
            return Replacements(sb.ToString());
        }

        public static string GetUserCommandError(string key, Locale locale, string command)
        {
            string message;
            try
            {
                message = JToken.Parse(File.ReadAllText($"{translationPath}/{locale.ToString().ToLower()}/usererrors.json"))[key].ToString();
            }
            catch
            {
                message = JToken.Parse(File.ReadAllText($"{translationPath}/en/usererrors.json"))[key].ToString();
            }
            StringBuilder sb = new StringBuilder(message);
            sb.Replace("{command}", command);
            return Replacements(sb.ToString());
        }

        static string Replacements(string message)
        {
            StringBuilder builder = new StringBuilder(message);
            // TODO: personality
            return builder.ToString();
        }
    }
}
