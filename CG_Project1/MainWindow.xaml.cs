using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using Point = System.Drawing.Point;

namespace CG_Project1
{

    public partial class MainWindow : Window
    {
        private static bool isGrayScale;

        public MainWindow()
        {
            InitializeComponent();
        }


        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new()
            {
                InitialDirectory = "c:\\",
                Filter = "Image files (*.jpg)|*.jpg|All Files (*.*)|*.*",
                RestoreDirectory = true
            };

            if (dlg.ShowDialog() == true)
            {
                var selectedFileName = dlg.FileName;
                BitmapImage bitmap = new BitmapImage(new Uri(selectedFileName));
                ;
                ImageViewer.Source = bitmap;
            }

            if (FilteredImage.Source != null)
            {
                FilteredImage.Source = null;
            }

            isGrayScale = false;
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new()
            {
                FileName = "Image",
                DefaultExt = ".png",
                Filter = "PNG File (.png)|*.png"
            };


            if (dlg.ShowDialog() == true)
            {
                PngBitmapEncoder encoder = new();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)FilteredImage.Source));
                using FileStream stream = new (dlg.FileName, FileMode.Create);
                encoder.Save(stream);
            }
        }


        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            if (FilteredImage.Source == null)
            {
                return;
            }

            FilteredImage.Source = ImageViewer.Source;
            isGrayScale = false;
        }


        public static Bitmap Invert(BitmapSource source)
        {
            int stride = (source.PixelWidth * source.Format.BitsPerPixel + 7) / 8;
            int length = stride * source.PixelHeight;

            var pixels = new byte[length];
            source.CopyPixels(pixels, stride, 0);

            for (int i = 0; i < length; i += 4)
            {
                pixels[i] = (byte)(255 - pixels[i]);
                pixels[i + 1] = (byte)(255 - pixels[i + 1]);
                pixels[i + 2] = (byte)(255 - pixels[i + 2]);
            }

            var src = BitmapSource.Create(source.PixelWidth, source.PixelHeight, source.DpiX, source.DpiY, source.Format, null, pixels, stride);


            Bitmap bmp = new Bitmap( src.PixelWidth, src.PixelHeight, PixelFormat.Format32bppPArgb);
            BitmapData data = bmp.LockBits( new Rectangle(Point.Empty, bmp.Size), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
            src.CopyPixels( Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }


        public static Bitmap Brighten(BitmapSource source, int brightness_factor)
        {
            int stride = (source.PixelWidth * source.Format.BitsPerPixel + 7) / 8;
            int length = stride * source.PixelHeight;

            byte[] pixels = new byte[length];
            source.CopyPixels(pixels, stride, 0);

            for (int i = 0; i < length; i += 4)
            {
                pixels[i] = (byte)((pixels[i] + brightness_factor > 255)
                    ? 255
                    : ((pixels[i] + brightness_factor < 0) ? 0 : pixels[i] + brightness_factor));
                pixels[i + 1] = (byte)((pixels[i + 1] + brightness_factor > 255)
                    ? 255
                    : ((pixels[i + 1] + brightness_factor < 0) ? 0 : pixels[i + 1] + brightness_factor));
                pixels[i + 2] = (byte)((pixels[i + 2] + brightness_factor > 255)
                    ? 255
                    : ((pixels[i + 2] + brightness_factor < 0) ? 0 : pixels[i + 2] + brightness_factor));

            }

            var src = BitmapSource.Create(source.PixelWidth, source.PixelHeight, source.DpiX, source.DpiY, source.Format,
                null, pixels, stride);


            Bitmap bmp = new Bitmap(src.PixelWidth, src.PixelHeight, PixelFormat.Format32bppPArgb);
            BitmapData data = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
            src.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }

        public static Bitmap MatrixMult(BitmapSource source, double[,] filterMatrix)
        {
            int stride = (source.PixelWidth * source.Format.BitsPerPixel + 7) / 8;
            int length = stride * source.PixelHeight;

            byte[] pixels = new byte[length];
            source.CopyPixels(pixels, stride, 0);

            double temp;
            double[] new_pixels = new double[3];


            for (int i = 0; i < length; i += 4)
            {
                byte[] new_vector = new[] { pixels[i], pixels[i + 1], pixels[i + 2] };

                for (int x = 0; x < 3; x++)
                {
                    temp = 0;
                    for (int k = 0; k < 3; k++)
                    {
                        temp += filterMatrix[x, k] * new_vector[k];
                    }

                    new_pixels[x] = temp;
                }


                pixels[i] = (byte)new_pixels[0];
                pixels[i + 1] = (byte)new_pixels[1];
                pixels[i + 2] = (byte)new_pixels[2];

            }

            var src = BitmapSource.Create(source.PixelWidth, source.PixelHeight, source.DpiX, source.DpiY, source.Format,
                null, pixels, stride);

            Bitmap bmp = new Bitmap(src.PixelWidth, src.PixelHeight, PixelFormat.Format32bppPArgb);
            BitmapData data = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
            src.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bmp.UnlockBits(data);
            return bmp;

        }


        public static Bitmap Contrast(BitmapSource source, int contrast)
        {
            int stride = (source.PixelWidth * source.Format.BitsPerPixel + 7) / 8;
            int length = stride * source.PixelHeight;

            byte[] pixels = new byte[length];
            source.CopyPixels(pixels, stride, 0);

            int factor = 259 * (contrast + 255) / (255 * (259 - contrast));

            for (int i = 0; i < length; i += 4)
            {
                pixels[i] = (byte)((factor * (pixels[i] - 128) + 128 > 255)
                    ? 255
                    : ((factor * (pixels[i] - 128) + 128 < 0) ? 0 : factor * (pixels[i] - 128) + 128));
                pixels[i + 1] = (byte)((factor * (pixels[i + 1] - 128) + 128 > 255)
                    ? 255
                    : ((factor * (pixels[i + 1] - 128) + 128 < 0) ? 0 : factor * (pixels[i + 1] - 128) + 128));
                pixels[i + 2] = (byte)((factor * (pixels[i + 2] - 128) + 128 > 255)
                    ? 255
                    : ((factor * (pixels[i + 2] - 128) + 128 < 0) ? 0 : factor * (pixels[i + 2] - 128) + 128));
            }

            var src = BitmapSource.Create(source.PixelWidth, source.PixelHeight, source.DpiX, source.DpiY, source.Format,
                null, pixels, stride);


            Bitmap bmp = new Bitmap(src.PixelWidth, src.PixelHeight, PixelFormat.Format32bppPArgb);
            BitmapData data = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
            src.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }

        public static Bitmap Gamma(BitmapSource source, double gamma)
        {
            int stride = (source.PixelWidth * source.Format.BitsPerPixel + 7) / 8;
            int length = stride * source.PixelHeight;

            byte[] pixels = new byte[length];
            source.CopyPixels(pixels, stride, 0);

            for (int i = 0; i < length; i += 4)
            {
                pixels[i] = (byte)(Math.Pow(pixels[i] / 255.0f, 1 / gamma) * 255);
                pixels[i + 1] = (byte)(Math.Pow(pixels[i + 1] / 255.0f, 1 / gamma) * 255);
                pixels[i + 2] = (byte)(Math.Pow(pixels[i + 2] / 255.0f, 1 / gamma) * 255);
            }

            var src = BitmapSource.Create(source.PixelWidth, source.PixelHeight, source.DpiX, source.DpiY, source.Format,
                null, pixels, stride);


            Bitmap bmp = new Bitmap(src.PixelWidth, src.PixelHeight, PixelFormat.Format32bppPArgb);
            BitmapData data = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
            src.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }


        public static Bitmap GreyScale(BitmapSource source)
        {
            int stride = (source.PixelWidth * source.Format.BitsPerPixel + 7) / 8;
            int length = stride * source.PixelHeight;

            byte[] pixels = new byte[length];
            source.CopyPixels(pixels, stride, 0);

            for (int i = 0; i < length; i += 4)
            {
                int red = pixels[i];
                int green = pixels[i + 1];
                int blue = pixels[i + 1];

                pixels[i] = pixels[i + 1] = pixels[i + 2] = (byte)((red + green + blue) / 3);

            }

            isGrayScale = true;

            var src = BitmapSource.Create(source.PixelWidth, source.PixelHeight, source.DpiX, source.DpiY, source.Format,
                null, pixels, stride);

            Bitmap bmp = new Bitmap(src.PixelWidth, src.PixelHeight, PixelFormat.Format32bppPArgb);
            BitmapData data = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
            src.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }


        private void InversionButton_Click(object sender, RoutedEventArgs e)
        {
            if (FilteredImage.Source == null)
            {
                FilteredImage.Source = ImageViewer.Source;
            }

            FilteredImage.Source = Bitmap2BitmapImage(Invert((BitmapSource)FilteredImage.Source));
        }

        private void BrightnessButton_Click(object sender, RoutedEventArgs e)
        {
            if (FilteredImage.Source == null)
            {
                FilteredImage.Source = ImageViewer.Source;
            }

            FilteredImage.Source = Bitmap2BitmapImage(Brighten((BitmapSource)FilteredImage.Source, 10));
        }

        private void ContrastButton_Click(object sender, RoutedEventArgs e)
        {
            if (FilteredImage.Source == null)
            {
                FilteredImage.Source = ImageViewer.Source;
            }

            FilteredImage.Source = Bitmap2BitmapImage(Contrast((BitmapSource)FilteredImage.Source, 128));

        }

        private void GammaButton_Click(object sender, RoutedEventArgs e)
        {
            if (FilteredImage.Source == null)
            {
                FilteredImage.Source = ImageViewer.Source;
            }

            FilteredImage.Source = Bitmap2BitmapImage(Gamma((BitmapSource)FilteredImage.Source, 20));
        }

        private void Blur_Click(object sender, RoutedEventArgs e)
        {
            if (FilteredImage.Source == null)
            {
                FilteredImage.Source = ImageViewer.Source;
            }

            var blur_filter = new BlurFilter();
            FilteredImage.Source = Bitmap2BitmapImage(((BitmapSource)FilteredImage.Source).ConvolutionFilter(blur_filter));
        }

        private void GaussianBlur_Click(object sender, RoutedEventArgs e)
        {
            if (FilteredImage.Source == null)
            {
                FilteredImage.Source = ImageViewer.Source;
            }

            var gauss_filter = new GaussianBlurFilter();
            FilteredImage.Source = Bitmap2BitmapImage(((BitmapSource)FilteredImage.Source).ConvolutionFilter(gauss_filter));
        }

        private void Sharpen_Click(object sender, RoutedEventArgs e)
        {
            if (FilteredImage.Source == null)
            {
                FilteredImage.Source = ImageViewer.Source;
            }

            var sharp_filter = new SharpenFilter();
            FilteredImage.Source = Bitmap2BitmapImage(((BitmapSource)FilteredImage.Source).ConvolutionFilter(sharp_filter));

        }

        private void EdgeDetection_Click(object sender, RoutedEventArgs e)
        {
            if (FilteredImage.Source == null)
            {
                FilteredImage.Source = ImageViewer.Source;
            }

            var edge_filter = new EdgeDetectionFilter();
            FilteredImage.Source = Bitmap2BitmapImage(((BitmapSource)FilteredImage.Source).ConvolutionFilter(edge_filter));
        }


        private void Emboss_Click(object sender, RoutedEventArgs e)
        {
            if (FilteredImage.Source == null)
            {
                FilteredImage.Source = ImageViewer.Source;
            }

            var emboss_filter = new EmbossFilter();
            FilteredImage.Source = Bitmap2BitmapImage(((BitmapSource)FilteredImage.Source).ConvolutionFilter(emboss_filter));
        }

        private void Matrix_Click(object sender, RoutedEventArgs e)
        {
            if (FilteredImage.Source == null)
            {
                FilteredImage.Source = ImageViewer.Source;
            }

            FilteredImage.Source = Bitmap2BitmapImage(MatrixMult((BitmapSource)FilteredImage.Source,
                new double[,] { { 0, 0, 1, }, { 0, 1, 0, }, { 1, 0, 0, }, }));

        }

        private void GreySc_OnClick(object sender, RoutedEventArgs e)
        {
            if (FilteredImage.Source == null)
            {
                FilteredImage.Source = ImageViewer.Source;
            }

            FilteredImage.Source = Bitmap2BitmapImage(GreyScale((BitmapSource)FilteredImage.Source));
        }


        private static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using MemoryStream outStream = new MemoryStream();
            BitmapEncoder enc = new BmpBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(bitmapImage));
            enc.Save(outStream);

            return new Bitmap(outStream);
        }

        public static BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            using MemoryStream memory = new MemoryStream();
            bitmap.Save(memory, ImageFormat.Png);
            memory.Position = 0;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }


        private void ApplyErr_OnClick(object sender, RoutedEventArgs e)
        {
            if (FilteredImage.Source == null)
            {
                FilteredImage.Source = ImageViewer.Source;
            }

            Bitmap bitmap = BitmapImage2Bitmap((BitmapImage)FilteredImage.Source);
            byte[,,] array = ErrorDiffusion.Make(ErrorDiffusion.GetImageArray(bitmap), 2, isGrayScale, ChooseKernel.SelectedItem.ToString());
            FilteredImage.Source = Bitmap2BitmapImage(ErrorDiffusion.GetBitmapFromArray(array));
        }


        private void ApplyPopul_OnClick(object sender, RoutedEventArgs e)
        {
            if (FilteredImage.Source == null)
            {
                FilteredImage.Source = ImageViewer.Source;
            }

            Bitmap bitmap = BitmapImage2Bitmap((BitmapImage)FilteredImage.Source);
            FilteredImage.Source = Bitmap2BitmapImage(Popularity.PopularityQuantizationApply(bitmap, int.Parse(ColorsNumber.Text)));
        }

    }

}
