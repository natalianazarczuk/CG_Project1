using Microsoft.Win32;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        }

        // later change ImageViewer to FilteredImage everywhere below
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

                var rtb = new RenderTargetBitmap((int)ImageViewer.ActualWidth, (int)ImageViewer.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                rtb.Render(ImageViewer);

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));
                using (var stm = File.Create(filename))
                {
                    encoder.Save(stm);
                }

            }
        }

        private void InversionButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BrightnessButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ContrastButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GammaButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
