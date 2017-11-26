using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaiCore.Helpers
{
    class RandomBitmap
    {
        public static Bitmap randombitmap()
        {
            Random rand = new Random();
            Bitmap result = new Bitmap(200, 200);
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

    }
}