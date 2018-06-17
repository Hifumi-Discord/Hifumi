using Hifumi.Enums;
using System;
using System.Drawing;
using System.IO;
using Console = Colorful.Console;

namespace Hifumi.Services
{
    public class LogService
    {
        readonly static object lockObject = new object();

        static void FileLog(string message)
        {
            lock (lockObject)
                using (var writer = File.AppendText($"{Directory.GetCurrentDirectory()}/log.txt"))
                    writer.WriteLine(message);
        }

        static void Append(string text, Color color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }

        public static void Write(LogSource source, string text, Color color)
        {
            string date = DateTime.Now.ToShortTimeString().Length <= 4 ?
                $"0{DateTime.Now.ToShortTimeString()}" : DateTime.Now.ToShortTimeString();
            Console.Write(Environment.NewLine);
            Append($"-> {date} ", Color.DarkGray);
            Append($"[{source}]", color);
            Append($" {text}", Color.WhiteSmoke);
            FileLog($"[{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}] [{source}] {text}");
        }

        public void PrintApplicationInformation()
        {
            string[] header =
            {
                @"",
                @" ██╗  ██╗██╗███████╗██╗   ██╗███╗   ███╗██╗",
                @" ██║  ██║██║██╔════╝██║   ██║████╗ ████║██║",
                @" ███████║██║█████╗  ██║   ██║██╔████╔██║██║",
                @" ██╔══██║██║██╔══╝  ██║   ██║██║╚██╔╝██║██║",
                @" ██║  ██║██║██║     ╚██████╔╝██║ ╚═╝ ██║██║",
                @" ╚═╝  ╚═╝╚═╝╚═╝      ╚═════╝ ╚═╝     ╚═╝╚═╝",
                @""
            };

            foreach (string line in header)
                Append($"{line}\n", Color.DarkMagenta);
            Append("-> INFORMATION\n", Color.Crimson);
            Append("\tAuthor: vic485\n\tVersion: 2018-Beta-06-16\n", Color.Bisque);
            Append("-> PACKAGES\n", Color.Crimson);
            Append($"\tDiscord: {Discord.DiscordConfig.Version}\n\tRavenDB: {Raven.Client.Properties.RavenVersionAttribute.Instance.FullVersion}\n", Color.Bisque);
            FileLog($"\n\n=================================[ {DateTime.Now.ToString("MM/dd/yyy HH:mm:ss")} ]=================================\n\n");
        }
    }
}
