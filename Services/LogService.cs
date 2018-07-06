using System;
using System.IO;

namespace Hifumi.Services
{
    public class LogService
    {
        readonly static object lockObject = new object();

        static void FileLog(string message)
        {
            using (var writer = File.AppendText($"{Directory.GetCurrentDirectory()}/log.txt"))
                writer.WriteLine(message);
        }

        public static void Write(string source, string text, ConsoleColor color)
        {
            lock (lockObject)
            {
                if (!Hifumi.Headless)
                {
                    Console.WriteLine();
                    string date = DateTime.Now.ToShortTimeString().Length <= 4 ?
                        $"0{DateTime.Now.ToShortTimeString()}" : DateTime.Now.ToShortTimeString();
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write($"-> {date} ");
                    Console.ForegroundColor = color;
                    Console.Write($"[{source}]");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($" {text}");
                    Console.ResetColor();
                }
                FileLog($"[{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}] [{source}] {text}");
            }
        }

        public void Initialize()
        {
            if (!Hifumi.Headless)
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

                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                foreach (string line in header)
                    Console.WriteLine(line);
                Console.ResetColor();
                Console.WriteLine("\nVersion: 2018-Beta-06-16");
            }
            FileLog($"\n\n=================================[ {DateTime.Now.ToString("MM/dd/yyy HH:mm:ss")} ]=================================\n\n");
        }
    }
}
