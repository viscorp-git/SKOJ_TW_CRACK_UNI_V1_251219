using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionTool;
using Cognex.VisionPro.CalibFix;

namespace Class
{
    public static class cls_Vision_Info
    {
        [Serializable]
        public class Run_Parameter
        {
            public cls_Blob Cathode_Blob = new cls_Blob();
            public cls_Blob Anode_Blob = new cls_Blob();
            public cls_Pattern Cathode_Pattern = new cls_Pattern();
            public cls_Pattern Anode_Pattern = new cls_Pattern();
        }

        public class Run_Result
        {
            public Run_Result()
            {

            }
        }
    }
}
