using Discord;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.Primitives;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hifumi.Helpers
{
    public class ImageHelper
    {
        public static async Task<string> GraveAsync(IGuildUser user, HttpClient httpClient, bool ayana)
        {
            string respects = (ayana) ? "=f to pay respects." : "Type F to pay respects.";
            int xLoc = (ayana) ? 150 : 120;
            using (var grave = SixLabors.ImageSharp.Image.Load("./resources/grave/base.png"))
            {
                var userImage = SixLabors.ImageSharp.Image.Load(await StringHelper.DownloadImageAsync(httpClient, user.GetAvatarUrl(ImageFormat.Png, 256)));
                userImage.Mutate(x => x.Grayscale());
                grave.Mutate(x =>
                {
                    var font = new FontCollection().Install("../resources/grave/font.ttf");
                    x.DrawImage(userImage, 1, new Size(170, 170), new Point(150, 180));
                    x.DrawText($"{user.JoinedAt.Value.Date.Year} - {DateTime.Now.Year}", font.CreateFont(25), Rgba32.Black, new PointF(180, 350));
                    x.DrawText(respects, font.CreateFont(25), Rgba32.Black, new PointF(xLoc, 380));
                });
                grave.Save("../resources/grave/user.png");
            }
            return "../resources/grave/user.png";
        }

        public static string WowsImage(HGame.Wows.Models.PlayerData player)
        {
            using (var background = SixLabors.ImageSharp.Image.Load("../resources/wows/base.png"))
            {
                // TODO: image editing
            }
            return "../resources/wows/user.png";
        }
    }
}
