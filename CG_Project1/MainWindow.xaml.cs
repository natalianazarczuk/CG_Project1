using Microsoft.Win32;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CG_Project1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = "c:\\";
            dlg.Filter = "Image files (*.jpg)|*.jpg|All Files (*.*)|*.*";
            dlg.RestoreDirectory = true;

            if(dlg.ShowDialog() == true) {
                var selectedFileName = dlg.FileName;
                BitmapImage bitmap = new BitmapImage(new Uri(selectedFileName));;
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
                var filename = dlg.FileName;

                var rtb = new RenderTargetBitmap((int)FilteredImage.ActualWidth, (int)FilteredImage.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                rtb.Render(FilteredImage);

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));
                using (var stm = File.Create(filename))
                {
                    encoder.Save(stm);
                }

            }
        }


        public static BitmapSource Invert(BitmapSource source)
        {
            int stride = (source.PixelWidth * source.Format.BitsPerPixel + 7)/8 ;
            int length = stride * source.PixelHeight;

            // array that holds pixel data of the source image
            byte[] pixels = new byte[length];   
            source.CopyPixels(pixels, stride, 0);

            for (int i = 0; i < length; i+=4)
            {
                pixels[i] = (byte)(255 - pixels[i]);
                pixels[i+1] = (byte)(255 - pixels[i+1]);
                pixels[i+2] = (byte)(255 - pixels[i+2]);
            }

            return BitmapSource.Create(source.PixelWidth, source.PixelHeight, source.DpiX, source.DpiY, source.Format, null, pixels, stride);
        }


        public static BitmapSource Brighten(BitmapSource source, int brightness_factor)
        {
            int stride = (source.PixelWidth * source.Format.BitsPerPixel + 7) / 8;
            int length = stride * source.PixelHeight;

            byte[] pixels = new byte[length];
            source.CopyPixels(pixels, stride, 0);
            
            for (int i = 0; i < length; i +=4)
            {
                pixels[i] = (byte)((pixels[i] + brightness_factor > 255) ? 255 : ((pixels[i] + brightness_factor < 0) ? 0 : pixels[i] + brightness_factor));
                pixels[i+1] = (byte)((pixels[i+1] + brightness_factor > 255) ? 255 : ((pixels[i+1] + brightness_factor < 0) ? 0 : pixels[i+1] + brightness_factor));
                pixels[i+2] = (byte)((pixels[i+2] + brightness_factor > 255) ? 255 : ((pixels[i+2] + brightness_factor < 0) ? 0 : pixels[i+2] + brightness_factor));

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
                pixels[i+1] = (byte)((factor * (pixels[i+1] - 128) + 128 > 255) ? 255 : ((factor * (pixels[i+1] - 128) + 128 < 0) ? 0 : factor * (pixels[i+1] - 128) + 128));
                pixels[i+2] = (byte)((factor * (pixels[i+2] - 128) + 128 > 255) ? 255 : ((factor * (pixels[i+2] - 128) + 128 < 0) ? 0 : factor * (pixels[i+2] - 128) + 128));
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
            if(FilteredImage.Source == null)
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

    }
}
