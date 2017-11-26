using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Helpers;
using SixLabors.Fonts;

namespace SaiCore.Helpers
{
    class RandomBitmap
    {
        public static System.Drawing.Bitmap randombitmap()
        {
            Random rand = new Random();
            System.Drawing.Bitmap result = new System.Drawing.Bitmap(200, 200);
            for (int i = 0; i < 200 * 200; ++i)
            {
                int r = rand.Next(1, 255);
                int g = rand.Next(1, 255);
                int b = rand.Next(1, 255);
                int row = i / 200;
                int col = i % 200;
                if (row % 2 != 0) col = 200 - col - 1;
                result.SetPixel(row, col, System.Drawing.Color.FromArgb(r, g, b));
            }
            return result;
        }

        public static Stream GenerateWithText(string txt)
        {
            
            var i = new SixLabors.ImageSharp.Image<Rgba32>(500, 50);
            var coll = new FontCollection();
            var fontf = coll.Install("font.ttf");
            var font = fontf.CreateFont(25);
            i.Mutate(x => x.DrawText(txt, font, Rgba32.Cyan, new SixLabors.Primitives.PointF(5, 5)));
            var ms = new MemoryStream();
            i.SaveAsPng(ms);
            ms.Position = 0;
            return ms;
        }

    }
}