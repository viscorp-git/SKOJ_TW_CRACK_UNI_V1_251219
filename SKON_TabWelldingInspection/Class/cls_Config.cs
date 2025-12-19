using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Class
{
    public class cls_Config
    {
        public static string Model_Path = "";
        public static string Image_Save_Path = "";
        public static int Image_Save_Day = 0;
        public static string Image_Save_Type = "";
        public static double Vision_Exposure = 0.3;
        public static double Vision_Gain = 0.1;

        public static string PLC_IP = "10.10.1.20";
        public static int PLC_Port = 2004;

        public static List<MODEL_CHECK_DATA> MODEL_CHECK_LIST = new List<MODEL_CHECK_DATA>();

        //public enum Image_Save_Type
        //{
        //    ALL,
        //    PASS,
        //    FAIL
        //}

        public static string GetDirectory()
        {
            FileInfo exeFileInfo = new FileInfo(Application.ExecutablePath);

            return exeFileInfo.Directory.FullName;
        }
    }

    public class MODEL_CHECK_DATA
    {
        public bool Checked = false;
        public string num = "";
        public string Name = "";
    }
}
