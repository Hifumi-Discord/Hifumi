using HGame;
using Hifumi.Enums;
using Hifumi.Models;
using Hifumi.Services;
using Raven.Client.Documents;
using System;
using System.Drawing;

namespace Hifumi.Handlers
{
    public class ConfigHandler
    {
        IDocumentStore Store { get; }
        public ConfigHandler(IDocumentStore store) => Store = store;

        public ConfigModel Config
        {
            get
            {
                using (var session = Store.OpenSession())
                    return session.Load<ConfigModel>("Config");
            }
        }

        public void ConfigCheck()
        {
            using (var session = Store.OpenSession())
            {
                if (session.Advanced.Exists("Config")) return;
                LogService.Write(LogSource.CNF, "Enter Bot's Token: ", Color.LightCoral);
                string token = Console.ReadLine();
                LogService.Write(LogSource.CNF, "Enter Bot's Prefix: ", Color.LightCoral);
                string prefix = Console.ReadLine();
                session.Store(new ConfigModel
                {
                    Id = "Config",
                    Token = token,
                    Prefix = prefix
                });
                session.SaveChanges();
            }
        }

        public void Save(ConfigModel config = null)
        {
            config = config ?? Config;
            if (config == null) return;
            using (var session = Store.OpenSession())
            {
                session.Store(config, "Config");
                session.SaveChanges();
            }
        }

        public HGameClient HGame
        {
            get => new HGameClient(new HGameConfig
            {
                OsuKey = Config.APIKeys["osu"],
                SteamKey = Config.APIKeys["Steam"],
                WowsKey = Config.APIKeys["Wows"]
            });
        }
    }
}
