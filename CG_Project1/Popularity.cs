using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CG_Project1
{
    class Popularity
    {
        public unsafe Bitmap PopularityQuantizationApply(Bitmap bitmap, int colorsNum)
        {
            Dictionary<Color, long> histogram = new();
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color c = bitmap.GetPixel(x, y);
                    if (histogram.ContainsKey(c))
                        histogram[c] = histogram[c] + 1;
                    else
                        histogram.Add(c, 1);
                }
            }


            List<KeyValuePair<Color, long>> myList = histogram.ToList();
            myList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

            List<Color> topColors = new List<Color>();
            int i = myList.Count() - 1;
            int counter = 0;
            while (counter < colorsNum)
            {
                topColors.Add(myList[i].Key);

                counter++;
                i--;
            }

            BitmapData bData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            int bitsPerPixel = Bitmap.GetPixelFormatSize(bitmap.PixelFormat);
            byte* scan0 = (byte*)bData.Scan0.ToPointer();

            for (int x = 0; x < bitmap.Width; ++x)
                for (int y = 0; y < bitmap.Height; ++y)
                {
                    int r, g, b;

                    byte* data = scan0 + x * bData.Stride + y * bitsPerPixel / 8;

                    r = data[2];
                    g = data[1];
                    b = data[0];

                    Color currentColor = Color.FromArgb(r, g, b);
                    Color newColor = FindNearestNeighborColor(x, y, currentColor, topColors);

                    data[2] = newColor.R;
                    data[1] = newColor.G;
                    data[0] = newColor.B;
                }

            bitmap.UnlockBits(bData);

            return bitmap;
        }

        private Color FindNearestNeighborColor(int x, int y, Color currentColor, List<Color> candidateColors)
        {
            int dMin = int.MaxValue;
            Color nearestCandidate = currentColor;

            foreach (Color candidateColor in candidateColors)
            {
                int distance = CalculateDistance(currentColor, candidateColor);
                if (distance < dMin)
                {
                    dMin = distance;
                    nearestCandidate = candidateColor;
                }
            }

            return nearestCandidate;
        }

        private int CalculateDistance(Color currentColor, Color candidateColor)
        {
            int distR = candidateColor.R - currentColor.R;
            int distG = candidateColor.G - currentColor.G;
            int distB = candidateColor.B - currentColor.B;

            return (int)(Math.Pow(distR, 2) + Math.Pow(distG, 2) + Math.Pow(distB, 2));
        }
    }
}
