using Cognex.VisionPro;
using Cognex.VisionPro.Display;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SKON_TabWelldingInspection
{
    public class cls_Display
    {
        public void ViewGuideLineX(CogDisplay cogDisplay, int LineX)
        {
            Cognex.VisionPro.CogLine Line = new Cognex.VisionPro.CogLine();
            Line.X = LineX;
            Line.Y = 0;
            Line.Rotation = Math.PI / 2;
            Line.LineWidthInScreenPixels = 2;
            cogDisplay.StaticGraphics.Add(Line, "");
        }


        public void ViewGuideLineY(CogDisplay cogDisplay, int LineY)
        {
            Cognex.VisionPro.CogLine Line = new Cognex.VisionPro.CogLine();
            Line.X = 0;
            Line.Y = LineY;
            Line.Rotation = 0;
            Line.LineWidthInScreenPixels = 2;
            cogDisplay.StaticGraphics.Add(Line, "");
        }


        public void DrawRect(CogDisplay cogDisplay, int x, int y)
        {
            CogRectangle rect = new CogRectangle();

            rect.SetXYWidthHeight(x, y, 100, 100);
            rect.LineWidthInScreenPixels = 2;
            rect.Color = CogColorConstants.Red;
            rect.Interactive = true;
            rect.GraphicDOFEnable = CogRectangleDOFConstants.Position;

            cogDisplay.InteractiveGraphics.Add(rect, "bright", true);
        }

        public int GetRectX(CogDisplay cogDisplay)
        {
            CogRectangle aRect;
            int rectIX;
            int rectX;
            rectIX = cogDisplay.InteractiveGraphics.FindItem("bright", CogDisplayZOrderConstants.Front);
            aRect = cogDisplay.InteractiveGraphics[rectIX] as CogRectangle;
            rectX = (int)aRect.X;
            return rectX;
        }

        public int GetRectY(CogDisplay cogDisplay)
        {
            CogRectangle aRect;
            int rectIX;
            int rectY;
            rectIX = cogDisplay.InteractiveGraphics.FindItem("bright", CogDisplayZOrderConstants.Front);
            aRect = cogDisplay.InteractiveGraphics[rectIX] as CogRectangle;
            rectY = (int)aRect.Y;
            return rectY;
        }

        public float ViewBrightValue(CogDisplay cogDisplay, int x, int y)
        {
            // 화면 중앙으로 부터 100,100 좌표의 평균 밝기값을 측정
            // 해상도 2448 * 2048 의 가운데좌표 = 1224, 1024
            // 1224-50, 1024-50 = 1174, 974
            // 해상도가 바뀔경우 공식을 변경하던지, 전체 이미지 픽셀수를 가져오도록 수정할것
            // 100*100 으로 측정하는데 100*100의 값이 커지면 측정하는데 오래 걸린다.
            try
            {
                Bitmap bimage = cogDisplay.Image.ToBitmap();
                Rectangle rect = new Rectangle(x, y, 100, 100);
                return CalculateAverageBrightness(bimage, rect);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error : {ex.Message}");
                return 0;
            }
        }

        private static float CalculateAverageBrightness(Bitmap image, Rectangle? rect = null)
        {
            // GetPixel 함수가 너무 느려서 성능이 좋지 않다고 한다.
            // 실시간으로 사용하지 않도록 한다.

            long totalBrightness = 0;
            int count = 0;

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    if (rect == null || rect.Value.Contains(x, y))
                    {
                        Color pixelColor = image.GetPixel(x, y);
                        totalBrightness += (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                        count++;
                    }
                }
            }

            return count > 0 ? (float)totalBrightness / count : 0;
        }

        public float ViewContrastValue(CogDisplay cogDisplay, int x , int y)
        {
            try
            {
                Bitmap bimage = cogDisplay.Image.ToBitmap();
                Rectangle rect = new Rectangle(x, y, 100, 100);
                return CalculateContrast2(bimage, rect);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error : {ex.Message}");
                return 0;
            }
        }

        private static float CalculateContrast(Bitmap image, Rectangle rect)
        {
            long totalBrightness = 0;
            long totalSquaredBrightness = 0;
            int pixelCount = 0;

            for (int y = rect.Top; y < rect.Bottom; y++)
            {
                for (int x = rect.Left; x < rect.Right; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    int brightness = (pixel.R + pixel.G + pixel.B) / 3;
                    totalBrightness += brightness;
                    totalSquaredBrightness += brightness * brightness;
                    pixelCount++;
                }
            }

            if (pixelCount == 0) return 0;

            float meanBrightness = (float)totalBrightness / (float)pixelCount;
            float meanSquaredBrightness = (float)totalSquaredBrightness / (float)pixelCount;

            return (float)Math.Sqrt(meanSquaredBrightness - meanBrightness * meanBrightness);
        }

        private static float CalculateContrast2(Bitmap image, Rectangle rect)
        {
            BitmapData bmpData = image.LockBits(rect, ImageLockMode.ReadOnly, image.PixelFormat);

            int bytesPerPixel = Image.GetPixelFormatSize(image.PixelFormat) / 8;
            int byteCount = bmpData.Stride * rect.Height;
            byte[] pixels = new byte[byteCount];

            IntPtr ptrFirstPixel = bmpData.Scan0;
            Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);

            long totalBrightness = 0;
            long totalSquaredBrightness = 0;
            int pixelCount = 0;

            for (int y = 0; y < rect.Height; y++)
            {
                int currentLine = y * bmpData.Stride;
                for (int x = 0; x < rect.Width; x++)
                {
                    int xIndex = currentLine + x * bytesPerPixel;
                    byte blue = pixels[xIndex];
                    byte green = pixels[xIndex + 1];
                    byte red = pixels[xIndex + 2];

                    int brightness = (red + green + blue) / 3;
                    totalBrightness += brightness;
                    totalSquaredBrightness += brightness * brightness;
                    pixelCount++;
                }
            }

            image.UnlockBits(bmpData);

            if (pixelCount == 0) return 0;

            float meanBrightness = (float)totalBrightness / pixelCount;
            float meanSquaredBrightness = (float)totalSquaredBrightness / pixelCount;

            return (float)Math.Sqrt(meanSquaredBrightness - meanBrightness * meanBrightness);
        }



        public void CogDisplayLoadImage(CogDisplay cogDisp)
        {
            // 이미지를 불러와서 디스플레이에 뿌려주는 메서드
            using (var ofd = new OpenFileDialog())
            {
                try
                {
                    DialogResult result = ofd.ShowDialog();
                    Bitmap bimage = new Bitmap(ofd.FileName);
                    cogDisp.Image = new Cognex.VisionPro.CogImage24PlanarColor((Bitmap)bimage.Clone());
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"error : {ex.Message}");
                }
            }
        }
    }
}