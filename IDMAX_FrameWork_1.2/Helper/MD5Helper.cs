using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace IDMAX_FrameWork
{
    public class MD5Helper
    {
        public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                if (!File.Exists(fileName)) return string.Empty;

                FileStream file = new FileStream(fileName, FileMode.Open);

                MD5 md5 = new MD5CryptoServiceProvider();

                byte[] retVal = md5.ComputeHash(file);

                file.Close();

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
