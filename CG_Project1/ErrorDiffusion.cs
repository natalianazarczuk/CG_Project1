using System;
using System.Drawing;

namespace CG_Project1
{
    public class ErrorDiffusion
    {
        public static byte[,,] Make(byte[,,] original, int factor, bool isGrayScale, string kernel)
        {
            if (original != null)
            {
                var clonedArray = (byte[,,]) original.Clone();
                if (isGrayScale)
                {
                    clonedArray = ConvertToGrayScale(clonedArray);
                }

                return Dither(clonedArray, factor, kernel);
            }

            throw new NullReferenceException();

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
                            byte[] oldCol = new byte[3] { ditherImage[y, x, 0], ditherImage[y, x, 1], ditherImage[y, x, 2] };

                            byte[] newCol = new byte[3] { (byte)(Math.Round(factor * oldCol[0] / 255.0) * (255 / factor)),
                                (byte)(Math.Round(factor * oldCol[1] / 255.0) * (255 / factor)),
                                (byte)(Math.Round(factor * oldCol[2] / 255.0) * (255 / factor))};

                            ditherImage[y, x, 0] = newCol[0];
                            ditherImage[y, x, 1] = newCol[1];
                            ditherImage[y, x, 2] = newCol[2];

                            int[] err = new int[3] { oldCol[0] - newCol[0], oldCol[1] - newCol[1], oldCol[2] - newCol[2] };

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
                            byte[] oldCol = new byte[3] { ditherImage[y, x, 0], ditherImage[y, x, 1], ditherImage[y, x, 2] };

                            byte[] newCol = new byte[3] { (byte)(Math.Round(factor * oldCol[0] / 255.0) * (255 / factor)),
                                (byte)(Math.Round(factor * oldCol[1] / 255.0) * (255 / factor)),
                                (byte)(Math.Round(factor * oldCol[2] / 255.0) * (255 / factor))};

                            ditherImage[y, x, 0] = newCol[0];
                            ditherImage[y, x, 1] = newCol[1];
                            ditherImage[y, x, 2] = newCol[2];

                            int[] err = new int[3] { oldCol[0] - newCol[0], oldCol[1] - newCol[1], oldCol[2] - newCol[2] };

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
                            byte[] oldCol = new byte[3] { ditherImage[y, x, 0], ditherImage[y, x, 1], ditherImage[y, x, 2] };

                            byte[] newCol = new byte[3] { (byte)(Math.Round(factor * oldCol[0] / 255.0) * (255 / factor)),
                                (byte)(Math.Round(factor * oldCol[1] / 255.0) * (255 / factor)),
                                (byte)(Math.Round(factor * oldCol[2] / 255.0) * (255 / factor))};

                            ditherImage[y, x, 0] = newCol[0];
                            ditherImage[y, x, 1] = newCol[1];
                            ditherImage[y, x, 2] = newCol[2];

                            int[] err = new int[3] { oldCol[0] - newCol[0], oldCol[1] - newCol[1], oldCol[2] - newCol[2] };
                            
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
                            byte[] oldCol = new byte[3] { ditherImage[y, x, 0], ditherImage[y, x, 1], ditherImage[y, x, 2] };

                            byte[] newCol = new byte[3] { (byte)(Math.Round(factor * oldCol[0] / 255.0) * (255 / factor)),
                                (byte)(Math.Round(factor * oldCol[1] / 255.0) * (255 / factor)),
                                (byte)(Math.Round(factor * oldCol[2] / 255.0) * (255 / factor))};

                            ditherImage[y, x, 0] = newCol[0];
                            ditherImage[y, x, 1] = newCol[1];
                            ditherImage[y, x, 2] = newCol[2];

                            int[] err = new int[3] { oldCol[0] - newCol[0], oldCol[1] - newCol[1], oldCol[2] - newCol[2] };

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
                            byte[] oldCol = new byte[3] { ditherImage[y, x, 0], ditherImage[y, x, 1], ditherImage[y, x, 2] };

                            byte[] newCol = new byte[3] { (byte)(Math.Round(factor * oldCol[0] / 255.0) * (255 / factor)),
                                (byte)(Math.Round(factor * oldCol[1] / 255.0) * (255 / factor)),
                                (byte)(Math.Round(factor * oldCol[2] / 255.0) * (255 / factor))};

                            ditherImage[y, x, 0] = newCol[0];
                            ditherImage[y, x, 1] = newCol[1];
                            ditherImage[y, x, 2] = newCol[2];

                            int[] err = new int[3] { oldCol[0] - newCol[0], oldCol[1] - newCol[1], oldCol[2] - newCol[2] };
                            
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


       private static void SetPx(byte[,,] img, int x, int y, double div, int[] err)
       {
           double r = img[y, x, 0];
           double g = img[y, x, 1]; 
           double b = img[y, x, 2]; 
           
           r += err[0] * div;
           g += err[1] * div;
           b += err[2] * div;
           
           img[y, x, 0] = CheckSize(r);
           img[y, x, 1] = CheckSize(g);
           img[y, x, 2] = CheckSize(b);
       }

       private static byte[,,] ConvertToGrayScale(byte[,,] imgArr)
       {
           if (imgArr == null)
           {
               return null;
           }

           var gray = new byte[imgArr.GetLength(0), imgArr.GetLength(1), 3];
           for (int i = 0; i < imgArr.GetLength(0); i++)
           {
               for (int j = 0; j < imgArr.GetLength(1); j++)
               {
                   var grayScale =
                       CheckSize(0.299 * imgArr[i, j, 0] + 0.587 * imgArr[i, j, 1] + 0.114 * imgArr[i, j, 2]);

                   gray[i, j, 0] = grayScale;
                   gray[i, j, 1] = grayScale;
                   gray[i, j, 2] = grayScale;
               }
           }

           return gray;
       }

       private static byte CheckSize(double input) => CheckSize((int)Math.Round(input));

       private static byte CheckSize(int input) => (input < 0) ? (byte)0 : ((input > 255) ? (byte)255 : (byte)input);


       public static byte[,,] GetImageArray(Bitmap bmp)
       {
           var cloneBmp = (Bitmap) bmp.Clone();
           if (cloneBmp.Width < 10 || cloneBmp.Height < 10)
           {
               return null;
           }

           var arr= new byte[cloneBmp.Height, cloneBmp.Width, 3];
           for (int i = 0; i < cloneBmp.Height; i++)
           {
               for (int j = 0; j < cloneBmp.Width; j++)
               {
                   arr[i, j, 0] = cloneBmp.GetPixel(j, i).R; 
                   arr[i, j, 1] = cloneBmp.GetPixel(j, i).G; 
                   arr[i, j, 2] = cloneBmp.GetPixel(j, i).B; 
               }
           }

           return arr;
       }

       public static Bitmap GetBitmapFromArray(byte[,,] imgArray)
       {
           if (imgArray == null || imgArray.GetLength(0) < 2 || imgArray.GetLength(1) < 2 ||
               imgArray.GetLength(2) != 3)
           {
               return null;
           }

           var bmp = new Bitmap(imgArray.GetLength(1), imgArray.GetLength(0));
           for (int y = 0; y < imgArray.GetLength(0); y++)
           {
               for (int x = 0; x < imgArray.GetLength(1); x++)
               {
                   bmp.SetPixel(x, y, Color.FromArgb(255, imgArray[y, x, 0], imgArray[y, x, 1], imgArray[y, x, 2]));
               }
           }

           return bmp;
       }
    }
}