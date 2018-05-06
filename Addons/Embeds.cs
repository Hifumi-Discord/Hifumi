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
                case Paint.Aqua:
                    embed.Color = new Color(39, 255, 255);
                    break;
                case Paint.Rose:
                    embed.Color = new Color(255, 153, 153);
                    break;
                case Paint.Lime:
                    embed.Color = new Color(217, 255, 207);
                    break;
                case Paint.Crimson:
                    embed.Color = new Color(220, 20, 60);
                    break;
                case Paint.Yellow:
                    embed.Color = new Color(255, 250, 131);
                    break;
                case Paint.Magenta:
                    embed.Color = new Color(245, 207, 255);
                    break;
                case Paint.PaleYellow:
                    embed.Color = new Color(255, 245, 207);
                    break;
                case Paint.Pink:
                    embed.Color = new Color(237, 116, 170);
                    break;
            }
            return embed;
        }

        public enum Paint
        {
            Lime,
            Aqua,
            Rose,
            Yellow,
            Crimson,
            Magenta,
            PaleYellow,
            // Final Colors
            Pink
        }
    }
}
