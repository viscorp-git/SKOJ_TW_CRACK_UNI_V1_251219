using Cognex.VisionPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SKON_TabWelldingInspection
{
    public class cls_RunResult
    {
        public string totalResult = "";
        public short totalValue = 0;
        public string imageJudgeTime;
        public string imageFilename;
        public string imageFileDirPath;

        public int roi_x1;
        public int roi_x2;
        public int roi_x3;
        public int roi_y;
        public int roi_w;
        public int roi_h;

        public string roiJudge1, roiJudgeCode1, roiJudgeScore1;
        public string roiJudge2, roiJudgeCode2, roiJudgeScore2;
        public string roiJudge3, roiJudgeCode3, roiJudgeScore3;

        public cls_RunResult()
        {
            totalResult = "OK";
            totalValue = 1;
            imageJudgeTime = "";
            imageFilename = "";
            imageFileDirPath = "";

            roi_x1 = 0;
            roi_x2 = 0;
            roi_x3 = 0;
            roi_y = 0;
            roi_w = 0;
            roi_h = 0;

            roiJudge1 = "";
            roiJudgeCode1 = "";
            roiJudgeScore1 = "";
            roiJudge2 = "";
            roiJudgeCode2 = "";
            roiJudgeScore2 = "";

            roiJudge3 = "";
            roiJudgeCode3 = "";
            roiJudgeScore3 = "";
        }


    }
}
