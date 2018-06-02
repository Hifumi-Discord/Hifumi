using Discord;

namespace Hifumi.Addons
{
    public class Embeds
    {
        public static EmbedBuilder GetEmbed(Paint paint)
        {
            var embed = new EmbedBuilder();
            switch (paint)
            {
                // Aqua
                case Paint.Temporary:
                    embed.Color = new Color(39, 255, 255);
                    break;
                case Paint.Yellow:
                    embed.Color = new Color(255, 250, 131);
                    break;
                case Paint.Pink:
                    embed.Color = new Color(237, 116, 170);
                    break;
            }
            return embed;
        }

        public enum Paint
        {
            Temporary,
            // Final Colors
            Pink,
            Yellow
        }
    }
}
