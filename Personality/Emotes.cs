using System;
using System.Collections.Generic;

namespace Hifumi.Personality
{
    public class Emotes
    {
        public static string GetEmote(EmoteType emoteType)
        {
            Random r = new Random();
            switch (emoteType)
            {
                case EmoteType.Happy: return happyEmotes[r.Next(happyEmotes.Count)];
                case EmoteType.Love: return loveEmotes[r.Next(loveEmotes.Count)];
                case EmoteType.Sad: return sadEmotes[r.Next(sadEmotes.Count)];
                case EmoteType.Worried: return worriedEmotes[r.Next(worriedEmotes.Count)];
            }
            return string.Empty;
        }

        private static List<string> happyEmotes = new List<string>()
        {   // TODO: More please
            @"(\*^▽^\*)",
            @"(^▽^)",
            @"(＾▽＾)",
            @"(\*≧▽≦)",
            @"(≧∇≦)/",
            @"(ノ\*゜▽゜\*)",
            @"o(≧∇≦o)",
            @"Ｏ(≧∇≦)Ｏ",
            @"(๑꒪▿꒪)\*",
            @"(=^▽^=)",
            @"~(˘▾˘~)",
            @"(^o^)／",
            @">^_^<"
        };

        public static List<string> loveEmotes = new List<string>()
        {
            @"(●♡∀♡)",
            @"(♥ω♥ ) ~♪",
            @"(♥ω♥\*)",
            @"✿♥‿♥✿",
            @"(｡･ω･｡)ﾉ♡",
            @"♡＾▽＾♡",
            @"(♡ >ω< ♡)",
            @"（〃・ω・〃）",
            @"(´,,•ω•,,)♡",
            @"(/∇＼\*)｡o○♡",
            @"(>^_^)><(^o^<)"
        };

        private static List<string> sadEmotes = new List<string>()
        {   // TODO: More sad?
            @"( ≧Д≦)",
            @"(｡•́︿•̀｡)",
            @"(っ- ‸ – ς)",
            @"(⌯˃̶᷄ ﹏ ˂̶᷄⌯)",
            @"(;\*△\*;)",
            @"꒰๑•̥﹏•̥๑꒱",
            @"(╯︵╰,)",
            @"(ᗒᗣᗕ)՞",
            @"(◞‸◟；)"
        };

        private static List<string> worriedEmotes = new List<string>()
        {
            @"（●´･△･｀）",
            sadEmotes[3],
            @"(　ﾟдﾟ)",
            @"(๑´•д • `๑)",
            @"(((( ;°Д°))))",
            @"(╯•﹏•╰)",
            @"(๑•﹏•)",
            @"●﹏●",
            @"(●´⌓`●)",
            @"(oﾟ□ﾟ)o",
            @"(◞‸◟；)"
        };
    }

    public enum EmoteType
    {
        Happy,
        Love,
        Sad,
        Worried
    }
}
