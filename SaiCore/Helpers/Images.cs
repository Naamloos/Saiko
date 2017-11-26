using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Brushes;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SaiCore.Helpers
{
    public class Images
    {
    }

    internal class TicTacToe
    {
        Image<Rgba32> i;
        string p1;
        string p2;
        Font f;
        Font fbig;
        Font fmedium;

        public TicTacToe(string player1, string player2)
        {
            i = new Image<Rgba32>(300, 320);
            p1 = player1;
            p2 = player2;
            var coll = new FontCollection();
            f = coll.Install("font.ttf").CreateFont(10);
            fmedium = coll.Install("font.ttf").CreateFont(25);
            fbig = coll.Install("font.ttf").CreateFont(50);

            PointF[] line1 = new PointF[2] { new PointF(100, 20), new PointF(100, 320) };
            PointF[] line2 = new PointF[2] { new PointF(200, 20), new PointF(200, 320) };
            PointF[] line3 = new PointF[2] { new PointF(0, 120), new PointF(300, 120) };
            PointF[] line4 = new PointF[2] { new PointF(0, 220), new PointF(300, 220) };

            i.Mutate(x => x.BackgroundColor(Rgba32.White)
                            .DrawLines(Rgba32.Black, 2f, line1)
                            .DrawLines(Rgba32.Black, 2f, line2)
                            .DrawLines(Rgba32.Black, 2f, line3)
                            .DrawLines(Rgba32.Black, 2f, line4)
                            .DrawText($"{p1} vs {p2}", f, Rgba32.DarkRed, new PointF(0, 0))
                            .DrawText("1", fmedium, Rgba32.LimeGreen, new PointF(35, 30))
                            .DrawText("2", fmedium, Rgba32.LimeGreen, new PointF(135, 30))
                            .DrawText("3", fmedium, Rgba32.LimeGreen, new PointF(235, 30))
                            .DrawText("4", fmedium, Rgba32.LimeGreen, new PointF(35, 130))
                            .DrawText("5", fmedium, Rgba32.LimeGreen, new PointF(135, 130))
                            .DrawText("6", fmedium, Rgba32.LimeGreen, new PointF(235, 130))
                            .DrawText("7", fmedium, Rgba32.LimeGreen, new PointF(35, 230))
                            .DrawText("8", fmedium, Rgba32.LimeGreen, new PointF(135, 230))
                            .DrawText("9", fmedium, Rgba32.LimeGreen, new PointF(235, 230)));
        }

        public Stream SetValue(int index, Players player)
        {
            int x = 0;
            int y = 0;
            #region index to location
            switch (index + 1)
            {
                case 1:
                    x = 0;
                    y = 0;
                    break;
                case 2:
                    x = 1;
                    y = 0;
                    break;
                case 3:
                    x = 2;
                    y = 0;
                    break;
                case 4:
                    x = 0;
                    y = 1;
                    break;
                case 5:
                    x = 1;
                    y = 1;
                    break;
                case 6:
                    x = 2;
                    y = 1;
                    break;
                case 7:
                    x = 0;
                    y = 2;
                    break;
                case 8:
                    x = 1;
                    y = 2;
                    break;
                case 9:
                    x = 2;
                    y = 2;
                    break;
            }

            #endregion
            var rx = x * 100 + 35;
            var ry = y * 100 + 30;

            i.Mutate(xx => xx.DrawText(player == Players.one ? "X" : "O", fbig, Rgba32.DarkRed, new PointF(rx, ry)));
            return GetImage();
        }

        public Stream GetImage()
        {
            MemoryStream ms = new MemoryStream();
            i.SaveAsPng(ms);
            ms.Position = 0;
            return ms;
        }
    }

    internal enum Players
    {
        one,
        two
    }
}
