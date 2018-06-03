using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Hifumi.Helpers
{
    public class CacheHelper
    {
        static Timer cacheTimer;

        public static string CacheFolder
        {
            get
            {
                if (!Directory.Exists("../cache"))
                    Directory.CreateDirectory("../cache");
                return "../cache";
            }
        }

        public static string CacheFile(string prepend, string extension) => $"{prepend}-{new Guid().ToString("n").Substring(0, 8)}.{extension}";

        public static void Initialize()
        {
            cacheTimer = new Timer(_ =>
            {
                foreach (string f in Directory.GetFiles(CacheFolder))
                {
                    if ((DateTime.Now - File.GetCreationTime(f)) > TimeSpan.FromMinutes(2))
                    {
                        File.Delete(f);
                    }
                }
            }, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30));
        }
    }
}
