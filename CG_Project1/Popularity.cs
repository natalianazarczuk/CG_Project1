using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;


namespace CG_Project1
{
    public class Popularity
    {
        public static unsafe Bitmap PopularityQuantizationApply(Bitmap bitmap, int colorsNum)
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


            var myList = histogram.ToList();
            myList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

            var topColors = new List<Color>();
            int i = myList.Count() - 1;
            int counter = 0;
            while (counter < colorsNum)
            {
                topColors.Add(myList[i].Key);

                counter++;
                i--;
            }

            var dataBtm = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            var bitsPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat);
            var scan0 = (byte*)dataBtm.Scan0.ToPointer();

            for (int x = 0; x < bitmap.Width; ++x)
                for (int y = 0; y < bitmap.Height; ++y)
                {
                    byte* data = scan0 + x * dataBtm.Stride + y * bitsPerPixel / 8;

                    int r = data[2];
                    int g = data[1];
                    int b = data[0];

                    var currentColor = Color.FromArgb(r, g, b);
                    var newColor = FindNearestNeighborColor(x, y, currentColor, topColors);

                    data[2] = newColor.R;
                    data[1] = newColor.G;
                    data[0] = newColor.B;
                }

            bitmap.UnlockBits(dataBtm);

            return bitmap;
        }

        private static Color FindNearestNeighborColor(int x, int y, Color currentColor, List<Color> candidateColors)
        {
            int minDist = int.MaxValue;
            Color nearestCandidate = currentColor;

            foreach (Color candidateColor in candidateColors)
            {
                int distance = CalculateDistance(currentColor, candidateColor);
                if (distance < minDist)
                {
                    minDist = distance;
                    nearestCandidate = candidateColor;
                }
            }

            return nearestCandidate;
        }

        private static int CalculateDistance(Color currentColor, Color candidateColor)
        {
            int distR = candidateColor.R - currentColor.R;
            int distG = candidateColor.G - currentColor.G;
            int distB = candidateColor.B - currentColor.B;

            return (int)(Math.Pow(distR, 2) + Math.Pow(distG, 2) + Math.Pow(distB, 2));
        }
    }
}
