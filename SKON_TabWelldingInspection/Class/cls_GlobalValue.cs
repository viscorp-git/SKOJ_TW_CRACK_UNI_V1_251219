using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKON_TabWelldingInspection
{
    public class cls_GlobalValue
    {

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        private static string iniPath = Path.Combine(System.Windows.Forms.Application.StartupPath, "Main_Information.ini");
        
        public static cls_Model Model { get; set; }
        public static string ModelPath { get; set; }
        public static string LastModelPath { get; set; }
        public static bool ChangeToolblock_Cathode { get; set; }
        public static bool ChangeToolblock_Anode { get; set; }

        public static string LastModelFileName { get { return Path.GetFileName(LastModelPath); } }

        public static void WriteIniValue(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, iniPath);
        }

        public static string ReadIniValue(string section, string key, string def)
        {
            var temp = new StringBuilder(255);
            int i = GetPrivateProfileString(section, key, def, temp, 255, iniPath);
            return temp.ToString();
        }
        public static string ReadIniString(string section, string key, string def)
        {
            return Convert.ToString(ReadIniValue(section, key, def));
        }

    }
}
