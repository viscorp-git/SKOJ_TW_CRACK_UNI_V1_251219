using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace SKON_TabWelldingInspection.Class
{
    public static class BitmapExtensions
    {
        /// <summary>
        /// Image Byte to Bitmap
        /// </summary>
        /// <param name="jpgBytes"></param>
        /// <returns></returns>
        public static Bitmap getBitmap(this byte[] jpgBytes)
        {
            MemoryStream ms = null;
            try
            {
                ms = new MemoryStream(jpgBytes);
                return new Bitmap(ms);  // Bitmap 객체 생성
            }
            catch { return null; }
            finally
            {
                // 메모리 스트림을 명시적으로 해제
                //if (ms != null)
                //{
                //    ms.Dispose();
                //}
            }
        }
    }

    internal class cls_Image
    {
        /// <summary>
        /// JPEG 이미지 Converter
        /// </summary>
        /// <param name="bimage">변환 대상 이미지 (bitmap)</param>
        /// <param name="jpgQuality">설정 이미지 퀄리티 - Default:80L</param>
        /// <returns></returns>
        public byte[] ConvertJpeg(Bitmap bimage, System.Int64 jpgQuality = 80L)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                ImageCodecInfo jpegEncoder = GetEncoder(System.Drawing.Imaging.ImageFormat.Jpeg);
                EncoderParameters encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, jpgQuality); // JPEG 품질 설정 (0~100 범위)

                bimage.Save(memoryStream, jpegEncoder, encoderParameters);

                return memoryStream.ToArray();
            }
        }

        private ImageCodecInfo GetEncoder(System.Drawing.Imaging.ImageFormat format)
        {
            // 모든 이미지 인코더를 가져옵니다.
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // 지정한 이미지 포맷과 일치하는 인코더를 찾습니다.
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }

            return null;
        }

        public void FileStreamSave(Bitmap bitmap, string filePath)
        {
            byte[] jpgBytes = ConvertJpeg(bitmap);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                fileStream.Write(jpgBytes, 0, jpgBytes.Length);
                fileStream.Close();
            }
        }
    }
}