using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CG_Project1
{
    public class ErrorDiffusion
    {
        public static byte[,,] Make(byte[,,] original, int factor, bool isGrayScale, string kernel)
        {
            if (original != null)
            {
                byte[,,] clonedArray = (byte[,,])original.Clone();
                if (isGrayScale)
                {
                    clonedArray = ConvertToGrayScale(clonedArray);
                }
                return Dither(clonedArray, factor, kernel);
            }
            else
            {
                throw new NullReferenceException();
            }
        }

       private static byte[,,] Dither(byte[,,] ditherImage, int factor, string kernel)
       {
            switch(kernel)
            {
                case "Floyd-Steinberg":
                    for (int y = 0; y < ditherImage.GetLength(0) - 1; y++)
                    {
                        for (int x = 1; x < ditherImage.GetLength(1) - 1; x++)
                        {
                            byte oldR = ditherImage[y, x, 0]; // R
                            byte oldG = ditherImage[y, x, 1]; // G
                            byte oldB = ditherImage[y, x, 2]; // B
                            //
                            byte newR = (byte)(Math.Round(factor * oldR / 255.0) * (255 / factor));
                            byte newG = (byte)(Math.Round(factor * oldG / 255.0) * (255 / factor));
                            byte newB = (byte)(Math.Round(factor * oldB / 255.0) * (255 / factor));
                            //
                            ditherImage[y, x, 0] = newR;
                            ditherImage[y, x, 1] = newG;
                            ditherImage[y, x, 2] = newB;
                            //
                            Error err = new Error(oldR - newR, oldG - newG, oldB - newB);
                            //
                            SetPx(ditherImage, x + 1, y, 7 / 16d, err);
                            SetPx(ditherImage, x - 1, y + 1, 3 / 16d, err);
                            SetPx(ditherImage, x, y + 1, 5 / 16d, err);
                            SetPx(ditherImage, x + 1, y + 1, 1 / 16d, err);
                        }
                    }
                    break;
                case "Burkes":
                    for (int y = 0; y < ditherImage.GetLength(0) - 1; y++)
                    {
                        for (int x = 2; x < ditherImage.GetLength(1) - 2; x++)
                        {
                            byte oldR = ditherImage[y, x, 0]; // R
                            byte oldG = ditherImage[y, x, 1]; // G
                            byte oldB = ditherImage[y, x, 2]; // B
                            //
                            byte newR = (byte)(Math.Round(factor * oldR / 255.0) * (255 / factor));
                            byte newG = (byte)(Math.Round(factor * oldG / 255.0) * (255 / factor));
                            byte newB = (byte)(Math.Round(factor * oldB / 255.0) * (255 / factor));
                            //
                            ditherImage[y, x, 0] = newR;
                            ditherImage[y, x, 1] = newG;
                            ditherImage[y, x, 2] = newB;
                            //
                            Error err = new Error(oldR - newR, oldG - newG, oldB - newB);
                            //
                            SetPx(ditherImage, x + 1, y, 8 / 32d, err);
                            SetPx(ditherImage, x + 2, y, 4 / 32d, err);

                            SetPx(ditherImage, x - 2, y + 1, 2 / 32d, err);
                            SetPx(ditherImage, x - 1, y + 1, 4 / 32d, err);
                            SetPx(ditherImage, x , y + 1, 8 / 32d, err);
                            SetPx(ditherImage, x + 1, y + 1, 4 / 32d, err);
                            SetPx(ditherImage, x + 2, y + 1, 2 / 32d, err);
                        }
                    }
                    break;
                case "Stucky":
                    for (int y = 0; y < ditherImage.GetLength(0) - 2; y++)
                    {
                        for (int x = 2; x < ditherImage.GetLength(1) - 2; x++)
                        {
                            byte oldR = ditherImage[y, x, 0]; // R
                            byte oldG = ditherImage[y, x, 1]; // G
                            byte oldB = ditherImage[y, x, 2]; // B
                            //
                            byte newR = (byte)(Math.Round(factor * oldR / 255.0) * (255 / factor));
                            byte newG = (byte)(Math.Round(factor * oldG / 255.0) * (255 / factor));
                            byte newB = (byte)(Math.Round(factor * oldB / 255.0) * (255 / factor));
                            //
                            ditherImage[y, x, 0] = newR;
                            ditherImage[y, x, 1] = newG;
                            ditherImage[y, x, 2] = newB;
                            //
                            Error err = new Error(oldR - newR, oldG - newG, oldB - newB);
                            //
                            SetPx(ditherImage, x + 1, y, 8 / 42d, err);
                            SetPx(ditherImage, x + 2, y, 4 / 42d, err);

                            SetPx(ditherImage, x - 2, y + 1, 2 / 42d, err);
                            SetPx(ditherImage, x - 1, y + 1, 4 / 42d, err);
                            SetPx(ditherImage, x, y + 1, 8 / 42d, err);
                            SetPx(ditherImage, x+1, y + 1, 4 / 42d, err);
                            SetPx(ditherImage, x +2, y + 1, 2 / 42d, err);

                            SetPx(ditherImage, x - 2, y + 2, 1 / 42d, err);
                            SetPx(ditherImage, x - 1, y + 2, 2 / 42d, err);
                            SetPx(ditherImage, x , y + 2, 4 / 42d, err);
                            SetPx(ditherImage, x + 1, y + 2, 2 / 42d, err);
                            SetPx(ditherImage, x + 2, y + 2, 1 / 42d, err);
                        }
                    }
                    break;
                case "Sierra":
                    for (int y = 0; y < ditherImage.GetLength(0) - 2; y++)
                    {
                        for (int x = 2; x < ditherImage.GetLength(1) - 2; x++)
                        {
                            byte oldR = ditherImage[y, x, 0]; // R
                            byte oldG = ditherImage[y, x, 1]; // G
                            byte oldB = ditherImage[y, x, 2]; // B
                            //
                            byte newR = (byte)(Math.Round(factor * oldR / 255.0) * (255 / factor));
                            byte newG = (byte)(Math.Round(factor * oldG / 255.0) * (255 / factor));
                            byte newB = (byte)(Math.Round(factor * oldB / 255.0) * (255 / factor));
                            //
                            ditherImage[y, x, 0] = newR;
                            ditherImage[y, x, 1] = newG;
                            ditherImage[y, x, 2] = newB;
                            //
                            Error err = new Error(oldR - newR, oldG - newG, oldB - newB);
                            //
                            SetPx(ditherImage, x + 1, y, 5 / 32d, err);
                            SetPx(ditherImage, x + 2, y, 3 / 32d, err);

                            SetPx(ditherImage, x - 2, y + 1, 2 / 32d, err);
                            SetPx(ditherImage, x - 1, y + 1, 4 / 32d, err);
                            SetPx(ditherImage, x , y + 1, 5 / 32d, err);
                            SetPx(ditherImage, x + 1, y + 1, 4 / 32d, err);
                            SetPx(ditherImage, x + 2, y + 1, 2 / 32d, err);

                            SetPx(ditherImage, x - 2, y + 2, 0, err);
                            SetPx(ditherImage, x - 1, y + 2, 2 / 32d, err);
                            SetPx(ditherImage, x , y + 2, 3 / 32d, err);
                            SetPx(ditherImage, x + 1, y + 2, 2 / 32d, err);
                            SetPx(ditherImage, x + 2, y + 2, 0, err);
                        }
                    }
                    break;
                case "Atkinson":
                    for (int y = 0; y < ditherImage.GetLength(0) - 2; y++)
                    {
                        for (int x = 2; x < ditherImage.GetLength(1) - 2; x++)
                        {
                            byte oldR = ditherImage[y, x, 0]; // R
                            byte oldG = ditherImage[y, x, 1]; // G
                            byte oldB = ditherImage[y, x, 2]; // B
                            //
                            byte newR = (byte)(Math.Round(factor * oldR / 255.0) * (255 / factor));
                            byte newG = (byte)(Math.Round(factor * oldG / 255.0) * (255 / factor));
                            byte newB = (byte)(Math.Round(factor * oldB / 255.0) * (255 / factor));
                            //
                            ditherImage[y, x, 0] = newR;
                            ditherImage[y, x, 1] = newG;
                            ditherImage[y, x, 2] = newB;
                            //
                            Error err = new Error(oldR - newR, oldG - newG, oldB - newB);
                            //
                            SetPx(ditherImage, x + 1, y, 1 / 8d, err);
                            SetPx(ditherImage, x + 2, y, 1 / 8d, err);

                            SetPx(ditherImage, x - 2, y + 1, 0, err);
                            SetPx(ditherImage, x - 1, y + 1, 1 / 8d, err);
                            SetPx(ditherImage, x , y + 1, 1 / 8d, err);
                            SetPx(ditherImage, x + 1, y + 1, 1 / 8d, err);
                            SetPx(ditherImage, x + 2, y + 1, 0, err);

                            SetPx(ditherImage, x - 2, y + 2, 0, err);
                            SetPx(ditherImage, x - 1, y + 2, 0, err);
                            SetPx(ditherImage, x, y + 2, 1 / 8d, err);
                            SetPx(ditherImage, x + 1, y + 2, 0, err);
                            SetPx(ditherImage, x + 2, y + 2, 0, err);
                        }
                    }
                    break;
            }

            return ditherImage;
        }


       private static void SetPx(byte[,,] img, int x, int y, double div, Error err)
       {
           double r = img[y, x, 0]; // R
           double g = img[y, x, 1]; // G
           double b = img[y, x, 2]; // B
           //
           r += err.r * div;
           g += err.g * div;
           b += err.b * div;
           //
           img[y, x, 0] = CheckSize(r);
           img[y, x, 1] = CheckSize(g);
           img[y, x, 2] = CheckSize(b);
       }

       private static byte[,,] ConvertToGrayScale(byte[,,] imgArr)
       {
           if (imgArr != null)
           {
               byte[,,] gray = new byte[imgArr.GetLength(0), imgArr.GetLength(1), 3];
               for (int i = 0; i < imgArr.GetLength(0); i++)
               {
                   for (int j = 0; j < imgArr.GetLength(1); j++)
                   {
                       byte grayScale = CheckSize(0.299 * imgArr[i, j, 0] + 0.587 * imgArr[i, j, 1] + 0.114 * imgArr[i, j, 2]);
                       gray[i, j, 0] = grayScale;
                       gray[i, j, 1] = grayScale;
                       gray[i, j, 2] = grayScale;
                   }
               }
               return gray;
           }
           return null;
       }

       private static byte CheckSize(double input) => CheckSize((int)Math.Round(input));

       private static byte CheckSize(int input) => (input < 0) ? (byte)0 : ((input > 255) ? (byte)255 : (byte)input);


       public static byte[,,] GetImageArray(Bitmap bmp)
       {
           Bitmap cloneBMP = (Bitmap)bmp.Clone();
           if (cloneBMP != null)
           {
               int _bmpHeight = cloneBMP.Height;
               int _bmpWidth = cloneBMP.Width;
               if (_bmpWidth > 10 && _bmpHeight > 10)
               {
                   byte[,,] arr = new byte[_bmpHeight, _bmpWidth, 3];
                   for (int i = 0; i < _bmpHeight; i++)
                   {
                       for (int j = 0; j < _bmpWidth; j++)
                       {
                           // For every pixel
                           arr[i, j, 0] = cloneBMP.GetPixel(j, i).R;    // Red
                           arr[i, j, 1] = cloneBMP.GetPixel(j, i).G;    // Green
                           arr[i, j, 2] = cloneBMP.GetPixel(j, i).B;    // Blue
                       }
                   }
                   //while (t1.IsAlive) ;
                   return arr;
               }
           }
           return null;
       }

       public static Bitmap GetBitmapFromArray(byte[,,] imgArray)
       {
           if (imgArray != null)
           {
               int _height = imgArray.GetLength(0);
               int _width = imgArray.GetLength(1);
               if (_height > 1 && _width > 1 && imgArray.GetLength(2) == 3)
               {
                   Bitmap bmp = new Bitmap(_width, _height);
                   for (int y = 0; y < _height; y++)
                   {
                       for (int x = 0; x < _width; x++)
                       {
                           bmp.SetPixel(x, y, Color.FromArgb(255, imgArray[y, x, 0], imgArray[y, x, 1], imgArray[y, x, 2]));
                       }
                   }
                   return bmp;
               }
           }
           throw new NullReferenceException("Array is Null!");
       }
    }

    class Error
    {
        public int r;
        public int g;
        public int b;

        public Error(int r, int g, int b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }
    }


}