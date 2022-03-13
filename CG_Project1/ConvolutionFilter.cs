using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CG_Project1
{
    public abstract class ConvolutionFilterBase
    {
        public abstract double Factor { get; }
        public abstract double Bias { get; }
        public abstract double[,] FilterMatrix { get; }
    }

    public class BlurFilter : ConvolutionFilterBase
    {
        private double factor = 1.0 / 9.0;
        public override double Factor { get { return factor; } }

        private double bias = 0.0;
        public override double Bias { get { return bias; } }

        private double[,] filterMatrix =
            new double[,] { { 1.0, 1.0, 1.0, }, 
                            { 1.0, 1.0, 1.0, },
                            { 1.0, 1.0, 1.0, }, };


        public override double[,] FilterMatrix { get { return filterMatrix; } }
    }

    public class GaussianBlurFilter : ConvolutionFilterBase
    {
        private double factor = 1.0 / 8.0;
        public override double Factor { get { return factor; } }

        private double bias = 0.0;
        public override double Bias { get { return bias; } }


        private double[,] filterMatrix =
            new double[,] { { 0, 1.0, 0, },
                            { 1.0, 4.0, 1.0, },
                            { 0, 1.0, 0, }, };


        public override double[,] FilterMatrix { get { return filterMatrix; } }
    }


    public class SharpenFilter : ConvolutionFilterBase
    {
        private double factor = 1.0;
        public override double Factor { get { return factor; } }

        private double bias = 0.0;
        public override double Bias { get { return bias; } }

        private double[,] filterMatrix =
            new double[,] { { 0, -1.0, 0, },
                            { -1.0, 5.0, -1.0, },
                            { 0, -1.0, 0, }, };


        public override double[,] FilterMatrix { get { return filterMatrix; } }
    }

    public class EdgeDetectionFilter : ConvolutionFilterBase
    {
        private double factor = 1.0;
        public override double Factor { get { return factor; } }

        private double bias = 0.0;
        public override double Bias { get { return bias; } }

        private double[,] filterMatrix =
            new double[,] { { -1.0, 0, 0, },
                            { 0, 1.0, 0, },
                            { 0, 0, 0, }, };


        public override double[,] FilterMatrix { get { return filterMatrix; } }
    }

    public class EmbossFilter : ConvolutionFilterBase
    {
        private double factor = 1.0;
        public override double Factor { get { return factor; } }

        private double bias = 10.0;
        public override double Bias { get { return bias; } }

        private double[,] filterMatrix =
            new double[,] { { -1.0, 0, 1.0, },
                            { -1.0, 1.0, 1.0, },
                            { -1.0, 0, 1.0, }, };


        public override double[,] FilterMatrix { get { return filterMatrix; } }
    }

    public static class EndBitmap
    {
        public static BitmapSource ConvolutionFilter<T>(this BitmapSource source, T filter) where T : ConvolutionFilterBase
        {
            int stride = (source.PixelWidth * source.Format.BitsPerPixel + 7) / 8;
            int length = stride * source.PixelHeight;

            byte[] pixels = new byte[length];
            byte[] result_pixels = new byte[length];

            source.CopyPixels(pixels, stride, 0);
            double blue, green, red;

            int calcOffset, byteOffset;
            int filterOffset = (filter.FilterMatrix.GetLength(1) - 1) / 2;

            for (int offsetY = filterOffset; offsetY < source.PixelHeight - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX < source.PixelWidth - filterOffset; offsetX++)
                {
                    blue = 0;
                    green = 0;
                    red = 0;

                    byteOffset = offsetY * stride + offsetX * 4;

                    for (int filterY = -filterOffset; filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset; filterX <= filterOffset; filterX++)
                        {
                            calcOffset = byteOffset + (filterX * 4) + (filterY * stride);

                            blue += pixels[calcOffset] * filter.FilterMatrix[filterY + filterOffset, filterX + filterOffset];
                            green += pixels[calcOffset + 1] * filter.FilterMatrix[filterY + filterOffset, filterX + filterOffset];
                            red += pixels[calcOffset + 2] * filter.FilterMatrix[filterY + filterOffset, filterX + filterOffset];
                        }
                    }

                    result_pixels[byteOffset] = (byte)((filter.Factor * blue + filter.Bias) > 255 ? 255 : ((filter.Factor * blue + filter.Bias) < 0 ? 0 : (filter.Factor * blue + filter.Bias)));
                    result_pixels[byteOffset + 1] = (byte)((filter.Factor * green + filter.Bias) > 255 ? 255 : ((filter.Factor * green + filter.Bias) < 0 ? 0 : (filter.Factor * green + filter.Bias)));
                    result_pixels[byteOffset + 2] = (byte)((filter.Factor * red + filter.Bias) > 255 ? 255 : ((filter.Factor * red + filter.Bias) < 0 ? 0 : (filter.Factor * red + filter.Bias)));
                    result_pixels[byteOffset + 3] = 255;
                }
            }

            return BitmapSource.Create(source.PixelWidth, source.PixelHeight, source.DpiX, source.DpiY, source.Format, null, result_pixels, stride);
        }

    }
}
