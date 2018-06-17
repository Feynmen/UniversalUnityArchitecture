using System;
using System.Linq;
using Tools.Patterns;
using UnityEngine;

namespace Tools
{
    public class ImageFilters : Singleton<ImageFilters>
    {
        public Texture2D Median(Texture2D image)
        {
            var arrR = new int[8];
            var arrG = new int[8];
            var arrB = new int[8];
            var outImage = new Texture2D(image.width, image.height);

            for (var x = 0; x < image.width; x++)
                for (var y = 0; y < image.height; y++)
                {
                    for (var x1 = 0; x1 < 2; x1++)
                        for (var y1 = 0; y1 < 2; y1++)
                        {
                            var p = image.GetPixel(x + x1 - 1, y + y1 - 1);

                            arrR[x1 * 3 + y1] = ((ToByte(p.r) + ToByte(p.g) + ToByte(p.b)) / 3) & 0xff;
                            arrG[x1 * 3 + y1] = ((ToByte(p.r) + ToByte(p.g) + ToByte(p.b)) / 3) >> 8 & 0xff;
                            arrB[x1 * 3 + y1] = ((ToByte(p.r) + ToByte(p.g) + ToByte(p.b)) / 3) >> 16 & 0xff;
                        }
                    Array.Sort(arrR);
                    Array.Sort(arrG);
                    Array.Sort(arrB);

                    //outImage.SetPixel(i, j, Color.FromArgb(arrR[3], arrG[4], arrB[5]));
                    outImage.SetPixel(x, y, new Color(arrR[3] / 255f, arrG[4] / 255f, arrB[5] / 255f));
                }
            outImage.Apply();
            return outImage;
        }

        public int GetMonochromeLevel(Texture2D image)
        {
            var level = image.GetPixels().Sum(pixel => ToByte(pixel.r) + ToByte(pixel.g) + ToByte(pixel.b) / 3);
            return level / (image.height * image.width);
        }

        public Texture2D Monochrome(Texture2D image, int level)
        {
            var outImage = new Texture2D(image.width, image.height);
            for (var j = 0; j < outImage.height; j++)
            {
                for (var i = 0; i < outImage.width; i++)
                {
                    var color = image.GetPixel(i, j);
                    //Debug.Log(color.r + " "+color.g + " "+ color.b);
                    var sr = ToByte(color.r) + ToByte(color.g) + ToByte(color.b) / 3;
                    if (level < 50)
                    {
                        outImage.SetPixel(i, j, (sr < level ? Color.white : Color.black));
                    }
                    else
                    {
                        outImage.SetPixel(i, j, (sr < level ? Color.black : Color.white));
                    }
                }
            }
            outImage.Apply();
            return outImage;
        }

        private static byte ToByte(float f)
        {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }
    }
}
