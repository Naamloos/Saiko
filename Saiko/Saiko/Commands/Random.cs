using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using System.Net;
using System.IO;

namespace Saiko.Commands
{
    [Group("random"), Aliases("rnd", "r"), Description("Randomized Commands")]
    public class RandomCommands
    {
        // ancient, almost untouched commands i made long ago
        [Command("cat"), Aliases("c"), Description("Gets a random cat image")]
        public async Task CatAsync(CommandContext ctx)
        {
            using (WebClient webclient = new WebClient())
            {
                var p = webclient.DownloadString("http://random.cat/meow");
                int pFrom = p.IndexOf("\\/i\\/") + "\\/i\\/".Length;
                int pTo = p.LastIndexOf("\"}");
                string cat = p.Substring(pFrom, pTo - pFrom);
                await ctx.RespondAsync(":cat: Nya! http://random.cat/i/" + cat);
            }
        }

        [Command("dog"), Aliases("d"), Description("Gets a random dog image")]
        public async Task DogAsync(CommandContext ctx)
        {
            using (WebClient webclient = new WebClient())
            {
                var p = webclient.DownloadString("http://random.dog/woof");
                await ctx.RespondAsync(":dog: Bork! http://random.dog/" + p);
            }
        }

        [Command("bitmap"), Aliases("bm", "b", "bmp"), Description("Gets a random bitmap")]
        public async Task BitmapAsync(CommandContext ctx)
        {
            var s = new MemoryStream();
            Helpers.RandomBitmap.randombitmap().Save(s, System.Drawing.Imaging.ImageFormat.Png);
            s.Position = 0;
            await ctx.RespondWithFileAsync(s, "r.png", "Random bitmap, comin up!");
        }
    }
}
