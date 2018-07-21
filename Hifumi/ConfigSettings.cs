using Hifumi.Models;
using Newtonsoft.Json;
using System.IO;

namespace Hifumi.Handlers
{
    public class ConfigSettings
    {
        public static HifumiModel Configuration
        {
            get
            {
                var configPath = $"{Directory.GetCurrentDirectory()}/config.json";
                if (!File.Exists(configPath))
                    File.WriteAllText(configPath, JsonConvert.SerializeObject(new HifumiModel(), Formatting.Indented));

                return JsonConvert.DeserializeObject<HifumiModel>(File.ReadAllText(configPath));
            }
        }
    }
}
