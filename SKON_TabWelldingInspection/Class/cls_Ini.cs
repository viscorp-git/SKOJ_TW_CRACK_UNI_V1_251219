using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKON_TabWelldingInspection.Class
{
    public class cls_Ini
    {
        private string path;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public cls_Ini(string _path)
        {
            this.path = _path;
        }

        public void WriteIniValue(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, this.path);
        }

        public string ReadIniValue(string section, string key, string def)
        {
            var temp = new StringBuilder(255);
            int i = GetPrivateProfileString(section, key, def, temp, 255, this.path);
            return temp.ToString();
        }

        public bool ReadIniBool(string section, string key, string def)
        {
            return Convert.ToBoolean(ReadIniValue(section, key, def));
        }

        public double ReadIniDouble(string section, string key, string def)
        {
            return Convert.ToDouble(ReadIniValue(section, key, def));
        }

        public int ReadIniInt(string section, string key, string def)
        {
            return Convert.ToInt32(ReadIniValue(section, key, def));
        }

        public string ReadIniString(string section, string key, string def)
        {
            return Convert.ToString(ReadIniValue(section, key, def));
        }
    }
}
