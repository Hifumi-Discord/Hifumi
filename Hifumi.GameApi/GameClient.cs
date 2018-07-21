using Hifumi.GameApi.Steam;
//using Hifumi.GameApi.Osu;
//using Hifumi.GameApi.Wows;

namespace Hifumi.GameApi
{
    public class GameClient
    {
        public SteamClient Steam { get; }
        //public OsuClient Osu { get; }
        //public WowsClient Wows { get; }

        public GameClient(ClientConfig config)
        {
            Steam = new SteamClient(config.SteamKey);
            //Osu = new OsuClient(config.OsuKey);
            //Wows = new WowsClient(config.WowsKey);
        }
    }
}
