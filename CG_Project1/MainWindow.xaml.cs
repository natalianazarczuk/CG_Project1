using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using LiveCharts.Configurations;
using System.ComponentModel;


namespace CG_Project1
{

    public partial class MainWindow : Window
    {
        public SeriesCollection MySeries { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            PlotGraph();
        }


        private void PlotGraph()
        {
            MySeries = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<double> { 0, 50, 100, 150 , 200, 255 }
                }
            };

            DataContext = this;
        }



        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = "c:\\";
            dlg.Filter = "Image files (*.jpg)|*.jpg|All Files (*.*)|*.*";
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == true)
            {
                var selectedFileName = dlg.FileName;
                BitmapImage bitmap = new BitmapImage(new Uri(selectedFileName)); ;
                ImageViewer.Source = bitmap;
            }

            if (FilteredImage.Source != null)
            {
                FilteredImage.Source = null;
            }
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog
            {
                FileName = "Image",
                DefaultExt = ".png",
                Filter = "PNG File (.png)|*.png"
            };


            if (dlg.ShowDialog() == true)
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)FilteredImage.Source));
                using (FileStream stream = new FileStream(dlg.FileName, FileMode.Create))
                    encoder.Save(stream);
            }
        }


        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            if (FilteredImage.Source == null) return;

            FilteredImage.Source = ImageViewer.Source;
        }

        public static BitmapSource Invert(BitmapSource source)
        {
            int stride = (source.PixelWidth * source.Format.BitsPerPixel + 7) / 8;
            int length = stride * source.PixelHeight;

            // array that holds pixel data of the source image
            byte[] pixels = new byte[length];
            source.CopyPixels(pixels, stride, 0);

            for (int i = 0; i < length; i += 4)
            {
                pixels[i] = (byte)(255 - pixels[i]);
                pixels[i + 1] = (byte)(255 - pixels[i + 1]);
                pixels[i + 2] = (byte)(255 - pixels[i + 2]);
            }

            return BitmapSource.Create(source.PixelWidth, source.PixelHeight, source.DpiX, source.DpiY, source.Format, null, pixels, stride);
        }


        public static BitmapSource Brighten(BitmapSource source, int brightness_factor)
        {
            int stride = (source.PixelWidth * source.Format.BitsPerPixel + 7) / 8;
            int length = stride * source.PixelHeight;

            byte[] pixels = new byte[length];
            source.CopyPixels(pixels, stride, 0);

            for (int i = 0; i < length; i += 4)
            {
                pixels[i] = (byte)((pixels[i] + brightness_factor > 255) ? 255 : ((pixels[i] + brightness_factor < 0) ? 0 : pixels[i] + brightness_factor));
                pixels[i + 1] = (byte)((pixels[i + 1] + brightness_factor > 255) ? 255 : ((pixels[i + 1] + brightness_factor < 0) ? 0 : pixels[i + 1] + brightness_factor));
                pixels[i + 2] = (byte)((pixels[i + 2] + brightness_factor > 255) ? 255 : ((pixels[i + 2] + brightness_factor < 0) ? 0 : pixels[i + 2] + brightness_factor));

            }

            return BitmapSource.Create(source.PixelWidth, source.PixelHeight, source.DpiX, source.DpiY, source.Format, null, pixels, stride);
        }

        public static BitmapSource MatrixMult(BitmapSource source, double[,] filterMatrix)
        {
            int stride = (source.PixelWidth * source.Format.BitsPerPixel + 7) / 8;
            int length = stride * source.PixelHeight;

            byte[] pixels = new byte[length];
            source.CopyPixels(pixels, stride, 0);

            double temp;
            double[] new_pixels = new double[3];


            for (int i = 0; i < length; i += 4)
            {
                var new_vector = new double[] { pixels[i], pixels[i + 1], pixels[i + 2] };

                for (int x = 0; x < 3; x++)
                {
                    temp = 0;
                    for (int k = 0; k < 3; k++)
                    {
                        temp += filterMatrix[x, k] * new_vector[k];
                    }
                    new_pixels[x] = temp;
                }


                pixels[i] = (byte)(new_pixels[0]);
                pixels[i + 1] = (byte)(new_pixels[1]);
                pixels[i + 2] = (byte)(new_pixels[2]);

            }

            return BitmapSource.Create(source.PixelWidth, source.PixelHeight, source.DpiX, source.DpiY, source.Format, null, pixels, stride);
        }


        public static BitmapSource Contrast(BitmapSource source, int contrast)
        {
            int stride = (source.PixelWidth * source.Format.BitsPerPixel + 7) / 8;
            int length = stride * source.PixelHeight;

            byte[] pixels = new byte[length];
            source.CopyPixels(pixels, stride, 0);

            int factor = (259 * (contrast + 255)) / (255 * (259 - contrast));

            for (int i = 0; i < length; i += 4)
            {
                pixels[i] = (byte)((factor * (pixels[i] - 128) + 128 > 255) ? 255 : ((factor * (pixels[i] - 128) + 128 < 0) ? 0 : factor * (pixels[i] - 128) + 128));
                pixels[i + 1] = (byte)((factor * (pixels[i + 1] - 128) + 128 > 255) ? 255 : ((factor * (pixels[i + 1] - 128) + 128 < 0) ? 0 : factor * (pixels[i + 1] - 128) + 128));
                pixels[i + 2] = (byte)((factor * (pixels[i + 2] - 128) + 128 > 255) ? 255 : ((factor * (pixels[i + 2] - 128) + 128 < 0) ? 0 : factor * (pixels[i + 2] - 128) + 128));
            }

            return BitmapSource.Create(source.PixelWidth, source.PixelHeight, source.DpiX, source.DpiY, source.Format, null, pixels, stride);
        }

        public static BitmapSource Gamma(BitmapSource source, double gamma)
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

            return BitmapSource.Create(source.PixelWidth, source.PixelHeight, source.DpiX, source.DpiY, source.Format, null, pixels, stride);
        }



        private void InversionButton_Click(object sender, RoutedEventArgs e)
        {
            if (FilteredImage.Source == null)
            {
                FilteredImage.Source = ImageViewer.Source;
            }

            FilteredImage.Source = Invert((BitmapSource)FilteredImage.Source);

        }

        private void BrightnessButton_Click(object sender, RoutedEventArgs e)
        {
            if (FilteredImage.Source == null)
            {
                FilteredImage.Source = ImageViewer.Source;
            }

            FilteredImage.Source = Brighten((BitmapSource)FilteredImage.Source, 10);

        }

        private void ContrastButton_Click(object sender, RoutedEventArgs e)
        {
            if (FilteredImage.Source == null)
            {
                FilteredImage.Source = ImageViewer.Source;
            }

            FilteredImage.Source = Contrast((BitmapSource)FilteredImage.Source, 128);

        }

        private void GammaButton_Click(object sender, RoutedEventArgs e)
        {
            if (FilteredImage.Source == null)
            {
                FilteredImage.Source = ImageViewer.Source;
            }

            FilteredImage.Source = Gamma((BitmapSource)FilteredImage.Source, 20);
        }

        private void Blur_Click(object sender, RoutedEventArgs e)
        {
            if (FilteredImage.Source == null)
            {
                FilteredImage.Source = ImageViewer.Source;
            }

            var blur_filter = new BlurFilter();
            FilteredImage.Source = ((BitmapSource)FilteredImage.Source).ConvolutionFilter(blur_filter);
        }
        private void GaussianBlur_Click(object sender, RoutedEventArgs e)
        {
            if (FilteredImage.Source == null)
            {
                FilteredImage.Source = ImageViewer.Source;
            }

            var gauss_filter = new GaussianBlurFilter();
            FilteredImage.Source = ((BitmapSource)FilteredImage.Source).ConvolutionFilter(gauss_filter);
        }

        private void Sharpen_Click(object sender, RoutedEventArgs e)
        {
            if (FilteredImage.Source == null)
            {
                FilteredImage.Source = ImageViewer.Source;
            }

            var sharp_filter = new SharpenFilter();
            FilteredImage.Source = ((BitmapSource)FilteredImage.Source).ConvolutionFilter(sharp_filter);

        }

        private void EdgeDetection_Click(object sender, RoutedEventArgs e)
        {
            if (FilteredImage.Source == null)
            {
                FilteredImage.Source = ImageViewer.Source;
            }

            var edge_filter = new EdgeDetectionFilter();
            FilteredImage.Source = ((BitmapSource)FilteredImage.Source).ConvolutionFilter(edge_filter);
        }


        private void Emboss_Click(object sender, RoutedEventArgs e)
        {
            if (FilteredImage.Source == null)
            {
                FilteredImage.Source = ImageViewer.Source;
            }

            var emboss_filter = new EmbossFilter();
            FilteredImage.Source = ((BitmapSource)FilteredImage.Source).ConvolutionFilter(emboss_filter);
        }

        private void Matrix_Click(object sender, RoutedEventArgs e)
        {
            if (FilteredImage.Source == null)
            {
                FilteredImage.Source = ImageViewer.Source;
            }

            FilteredImage.Source = MatrixMult((BitmapSource)FilteredImage.Source, new double[,] { { 0, 0, 1, }, { 0, 1, 0, }, { 1, 0, 0, }, });

        }
    }

}
