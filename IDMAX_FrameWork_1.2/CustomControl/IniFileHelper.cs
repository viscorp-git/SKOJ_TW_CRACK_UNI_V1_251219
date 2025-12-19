using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Helper
{
    public class IniFileHelper
    {
        public const int MaxSectionSize = 32767; // 32 KB

        private string _path;

        #region P/Invoke declares
        [System.Security.SuppressUnmanagedCodeSecurity]
        private static class NativeMethods
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            public static extern int GetPrivateProfileSectionNames(IntPtr lpszReturnBuffer,
                                                                   uint nSize,
                                                                   string lpFileName);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            public static extern uint GetPrivateProfileString(string lpAppName,
                                                              string lpKeyName,
                                                              string lpDefault,
                                                              StringBuilder lpReturnedString,
                                                              int nSize,
                                                              string lpFileName);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            public static extern uint GetPrivateProfileString(string lpAppName,
                                                              string lpKeyName,
                                                              string lpDefault,
                                                              [In, Out] char[] lpReturnedString,
                                                              int nSize,
                                                              string lpFileName);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            public static extern int GetPrivateProfileString(string lpAppName,
                                                             string lpKeyName,
                                                             string lpDefault,
                                                             IntPtr lpReturnedString,
                                                             uint nSize,
                                                             string lpFileName);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            public static extern int GetPrivateProfileInt(string lpAppName,
                                                          string lpKeyName,
                                                          int lpDefault,
                                                          string lpFileName);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            public static extern int GetPrivateProfileSection(string lpAppName,
                                                              IntPtr lpReturnedString,
                                                              uint nSize,
                                                              string lpFileName);

            //We explicitly enable the SetLastError attribute here because
            // WritePrivateProfileString returns errors via SetLastError.
            // Failure to set this can result in errors being lost during 
            // the marshal back to managed code.
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern bool WritePrivateProfileString(string lpAppName,
                                                                string lpKeyName,
                                                                string lpString,
                                                                string lpFileName);


        }
        #endregion

        public IniFileHelper(string path)
        {
            this._path = System.IO.Path.GetFullPath(path);
        }

        public string Path
        {
            get { return this._path; }
        }

        #region Get Value Methods

        public string GetString(string sectionName, string keyName, string defaultValue)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            if (keyName == null)
                throw new ArgumentNullException("keyName");

            StringBuilder retval = new StringBuilder(IniFileHelper.MaxSectionSize);

            NativeMethods.GetPrivateProfileString(sectionName,
                                                  keyName,
                                                  defaultValue,
                                                  retval,
                                                  IniFileHelper.MaxSectionSize,
                                                  this._path);

            return retval.ToString();
        }

        public int GetInt16(string sectionName, string keyName, short defaultValue)
        {
            int retval = GetInt32(sectionName, keyName, defaultValue);

            return Convert.ToInt16(retval);
        }

        public int GetInt32(string sectionName, string keyName, int defaultValue)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            if (keyName == null)
                throw new ArgumentNullException("keyName");

            return NativeMethods.GetPrivateProfileInt(sectionName, keyName, defaultValue, _path);
        }

        public double GetDouble(string sectionName, string keyName, double defaultValue)
        {
            string retval = GetString(sectionName, keyName, "");

            if (retval == null || retval.Length == 0)
            {
                return defaultValue;
            }

            return Convert.ToDouble(retval, CultureInfo.InvariantCulture);
        }

        #endregion

        #region GetSectionValues Methods

        public List<KeyValuePair<string, string>> GetSectionValuesAsList(string sectionName)
        {
            List<KeyValuePair<string, string>> retval;
            string[] keyValuePairs;
            string key, value;
            int equalSignPos;

            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            IntPtr ptr = Marshal.AllocCoTaskMem(IniFileHelper.MaxSectionSize);

            try
            {
                int len = NativeMethods.GetPrivateProfileSection(sectionName,
                                                                 ptr,
                                                                 IniFileHelper.MaxSectionSize,
                                                                 _path);

                keyValuePairs = ConvertNullSeperatedStringToStringArray(ptr, len);
            }
            finally
            {
                Marshal.FreeCoTaskMem(ptr);
            }

            retval = new List<KeyValuePair<string, string>>(keyValuePairs.Length);

            for (int i = 0; i < keyValuePairs.Length; ++i)
            {
                equalSignPos = keyValuePairs[i].IndexOf('=');

                key = keyValuePairs[i].Substring(0, equalSignPos);

                value = keyValuePairs[i].Substring(equalSignPos + 1,
                                                   keyValuePairs[i].Length - equalSignPos - 1);

                retval.Add(new KeyValuePair<string, string>(key, value));
            }

            return retval;
        }

        public Dictionary<string, string> GetSectionValues(string sectionName)
        {
            List<KeyValuePair<string, string>> keyValuePairs;
            Dictionary<string, string> retval;

            keyValuePairs = GetSectionValuesAsList(sectionName);

            retval = new Dictionary<string, string>(keyValuePairs.Count);

            foreach (KeyValuePair<string, string> keyValuePair in keyValuePairs)
            {
                if (!retval.ContainsKey(keyValuePair.Key))
                {
                    retval.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }

            return retval;
        }

        #endregion

        #region Get Key/Section Names

        public string[] GetKeyNames(string sectionName)
        {
            int len;
            string[] retval;

            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            IntPtr ptr = Marshal.AllocCoTaskMem(IniFileHelper.MaxSectionSize);

            try
            {
                len = NativeMethods.GetPrivateProfileString(sectionName, null, null, ptr, IniFileHelper.MaxSectionSize, _path);

                retval = ConvertNullSeperatedStringToStringArray(ptr, len);
            }
            finally
            {
                Marshal.FreeCoTaskMem(ptr);
            }

            return retval;
        }

        public string[] GetSectionNames()
        {
            string[] retval;
            int len;

            IntPtr ptr = Marshal.AllocCoTaskMem(IniFileHelper.MaxSectionSize);

            try
            {
                len = NativeMethods.GetPrivateProfileSectionNames(ptr,
                    IniFileHelper.MaxSectionSize, _path);

                retval = ConvertNullSeperatedStringToStringArray(ptr, len);
            }
            finally
            {
                Marshal.FreeCoTaskMem(ptr);
            }

            return retval;
        }

        private static string[] ConvertNullSeperatedStringToStringArray(IntPtr ptr, int valLength)
        {
            string[] retval;

            if (valLength == 0)
            {
                retval = new string[0];
            }
            else
            {
                string buff = Marshal.PtrToStringAuto(ptr, valLength - 1);

                retval = buff.Split('\0');
            }

            return retval;
        }

        #endregion

        #region Write Methods

        private void WriteValueInternal(string sectionName, string keyName, string value)
        {
            if (!NativeMethods.WritePrivateProfileString(sectionName, keyName, value, _path))
            {
                throw new System.ComponentModel.Win32Exception();
            }
        }

        public void WriteValue(string sectionName, string keyName, string value)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            if (keyName == null)
                throw new ArgumentNullException("keyName");

            if (value == null)
                throw new ArgumentNullException("value");

            WriteValueInternal(sectionName, keyName, value);
        }

        public void WriteValue(string sectionName, string keyName, short value)
        {
            WriteValue(sectionName, keyName, (int)value);
        }

        public void WriteValue(string sectionName, string keyName, int value)
        {
            WriteValue(sectionName, keyName, value.ToString(CultureInfo.InvariantCulture));
        }

        public void WriteValue(string sectionName, string keyName, float value)
        {
            WriteValue(sectionName, keyName, value.ToString(CultureInfo.InvariantCulture));
        }

        public void WriteValue(string sectionName, string keyName, double value)
        {
            WriteValue(sectionName, keyName, value.ToString(CultureInfo.InvariantCulture));
        }

        #endregion

        #region Delete Methods

        public void DeleteKey(string sectionName, string keyName)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            if (keyName == null)
                throw new ArgumentNullException("keyName");

            WriteValueInternal(sectionName, keyName, null);
        }

        public void DeleteSection(string sectionName)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            WriteValueInternal(sectionName, null, null);
        }

        #endregion
    }
}
