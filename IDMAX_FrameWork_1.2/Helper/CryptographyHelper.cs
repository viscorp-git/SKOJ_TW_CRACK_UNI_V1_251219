using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace IDMAX_FrameWork
{
    /// <summary>
    /// Rijndael 알고리즘을 이용하여 암호화, 복호화를 합니다.
    /// </summary>
    /// <remarks>
    /// 2009/02/26          김지호          [L 00]
    /// </remarks>
    public class CryptographyHelper
    {
        #region Encrypt

        /// <summary>
        /// 문자열을 암호화하여 리턴합니다.
        /// </summary>
        /// <param name="value">암호화할 문자열</param>
        /// <returns>암호화한 문자열</returns>
        /// <remarks>
        /// 2009/02/26          김지호          [L 00]
        /// </remarks>
        public static string EncryptString(string value)
        {
            try
            {
                // Rihndael class를 선언하고, 초기화 합니다
                RijndaelManaged rijndaelCipher = new RijndaelManaged();

                // 입력받은 문자열을 바이트 배열로 변환
                byte[] plainText = System.Text.Encoding.Unicode.GetBytes(value);

                // 딕셔너리 공격을 대비해서 키를 더 풀기 어렵게 만들기 위해서
                // Salt를 사용합니다.
                byte[] salt = Encoding.ASCII.GetBytes(value.Length.ToString());

                // PasswordDeriveBytes 클래스를 사용해서 SecretKey를 얻습니다.

                PasswordDeriveBytes secretKey = new PasswordDeriveBytes(value, salt);

                // Create a encryptor from the existing SecretKey bytes.
                // encryptor 객체를 SecretKey로부터 만듭니다.
                // Secret Key에는 32바이트
                // (Rijndael의 디폴트인 256bit가 바로 32바이트입니다)를 사용하고,
                // Initialization Vector로 16바이트
                // (역시 디폴트인 128비트가 바로 16바이트입니다)를 사용합니다
                ICryptoTransform encryptor = rijndaelCipher.CreateEncryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));

                // 메모리스트림 객체를 선언,초기화
                MemoryStream memoryStream = new MemoryStream();

                // CryptoStream객체를 암호화된 데이터를 쓰기 위한 용도로 선언
                CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

                // 암호화 프로세스가 진행됩니다.
                cryptoStream.Write(plainText, 0, plainText.Length);

                // 암호화 종료
                cryptoStream.FlushFinalBlock();

                // 암호화된 데이터를 바이트 배열로 담습니다.
                byte[] cipherBytes = memoryStream.ToArray();

                // 스트림 해제
                memoryStream.Close();
                cryptoStream.Close();

                // 암호화된 데이터를 Base64 인코딩된 문자열로 변환합니다.
                string encryptedData = Convert.ToBase64String(cipherBytes);

                // 최종 결과를 리턴
                return encryptedData;
            }
            catch
            {
                return string.Empty;
                throw;
            }
        }

        #endregion Encrypt

        #region Decrypt

        /// <summary>
        /// 문자열을 복호화하여 리턴합니다.
        /// </summary>
        /// <param name="encryptionString">복호화할 문자열</param>
        /// <returns>복호화한 문자열</returns>
        /// <remarks>
        /// 2009/02/26          김지호          [L 00]
        /// </remarks>
        public static string DecryptString(string key, string encryptionString)
        {
            try
            {
                RijndaelManaged rijndaelCipher = new RijndaelManaged();
                byte[] encryptedData = Convert.FromBase64String(encryptionString);
                byte[] salt = Encoding.ASCII.GetBytes(key.Length.ToString());
                PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(key, salt);

                // Decryptor 객체를 만듭니다.
                ICryptoTransform decryptor = rijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
                MemoryStream memoryStream = new MemoryStream(encryptedData);

                // 데이터 읽기(복호화이므로) 용도로 cryptoStream객체를 선언, 초기화
                CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

                // 복호화된 데이터를 담을 바이트 배열을 선언합니다.
                // 길이는 알 수 없지만, 일단 복호화되기 전의 데이터의 길이보다는
                // 길지 않을 것이기 때문에 그 길이로 선언합니다

                byte[] plainText = new byte[encryptedData.Length];

                // 복호화 시작
                int decryptedCount = cryptoStream.Read(plainText, 0, plainText.Length);
                memoryStream.Close();
                cryptoStream.Close();

                // 복호화된 데이터를 문자열로 바꿉니다.
                string decryptedData = Encoding.Unicode.GetString(plainText, 0, decryptedCount);

                // 최종 결과 리턴
                return decryptedData;
            }
            catch
            {
                return string.Empty;
                throw;
            }
        }

        #endregion Decrypt
    }
}