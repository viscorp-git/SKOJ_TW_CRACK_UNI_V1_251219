using Newtonsoft.Json;  // 누겟으로 설치
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace SKON_TabWelldingInspection.Class
{
    public class cls_Json
    {
        private List<ImageData> imageList = new List<ImageData>();        

        private string jsonSavePath = @"D:\JSON"; // 임시 사용. INI등에서 받아오도록 수정할 것.
        private string oldCellId = "";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cellId"></param>
        /// <param name="cornerId"></param>
        /// <param name="imagePath"></param>
        /// <param name="fileName"></param>
        /// <param name="cornerJudge"></param>
        /// <param name="imageJudgeTime"></param>
        /// <param name="roi_x1"></param>
        /// <param name="roi_x2"></param>
        /// <param name="roi_x3"></param>
        /// <param name="roi_y"></param>
        /// <param name="roi_w"></param>
        /// <param name="roi_h"></param>
        /// <param name="roiJudge1"></param>
        /// <param name="roiJudgeCode1"></param>
        /// <param name="roiJudgeScore1"></param>
        /// <param name="roiJudge2"></param>
        /// <param name="roiJudgeCode2"></param>
        /// <param name="roiJudgeScore2"></param>
        /// <param name="roiJudge3"></param>
        /// <param name="roiJudgeCode3"></param>
        /// <param name="roiJudgeScore3"></param>
        public void Save(string cellId, string cornerId, string imagePath, string fileName, string cornerJudge, string imageJudgeTime
            , int roi_x1, int roi_x2, int roi_x3, int roi_y, int roi_w, int roi_h
            , string roiJudge1, string roiJudgeCode1, string roiJudgeScore1
            , string roiJudge2, string roiJudgeCode2, string roiJudgeScore2
            , string roiJudge3, string roiJudgeCode3, string roiJudgeScore3
    )
        {
            cls_File.CreateFolder(jsonSavePath);

            if (cellId != oldCellId)
            {
                oldCellId = cellId;
                imageList.Clear();
            }

            // 저장기능만 처리하도록 분리하였으나 내용상 동일할 경우 1개로 합쳐야 한다.
            // 전달되는 파라미터가 너무 많아서 조정이 필요하다.
            SaveJSON_VPDL(cellId, cornerId, imagePath, fileName, cornerJudge, imageJudgeTime
                , roi_x1, roi_x2, roi_x3, roi_y, roi_w, roi_h
                , roiJudge1, roiJudgeCode1, roiJudgeScore1
                , roiJudge2, roiJudgeCode2, roiJudgeScore2
                , roiJudge3, roiJudgeCode3, roiJudgeScore3
                );

            ImageList jsonList = new ImageList { IMAGE_LIST = imageList };            
            string json = JsonConvert.SerializeObject(jsonList, Formatting.Indented);

            try
            {
                File.WriteAllText(Path.Combine(jsonSavePath, cellId + ".json"), json);
                Thread.Sleep(50);
            }
            catch (System.Exception)
            {
                //
            }
    

            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cellId"></param>
        /// <param name="cornerId"></param>
        /// <param name="imagePath"></param>
        /// <param name="fileName"></param>
        /// <param name="cornerJudge"></param>
        /// <param name="imageJudgeTime"></param>
        /// <param name="roi_x1"></param>
        /// <param name="roi_x2"></param>
        /// <param name="roi_x3"></param>
        /// <param name="roi_y"></param>
        /// <param name="roi_w"></param>
        /// <param name="roi_h"></param>
        /// <param name="roiJudge1"></param>
        /// <param name="roiJudgeCode1"></param>
        /// <param name="roiJudgeScore1"></param>
        /// <param name="roiJudge2"></param>
        /// <param name="roiJudgeCode2"></param>
        /// <param name="roiJudgeScore2"></param>
        /// <param name="roiJudge3"></param>
        /// <param name="roiJudgeCode3"></param>
        /// <param name="roiJudgeScore3"></param>
        public void SaveJSON_VPDL(string cellId, string cornerId, string imagePath, string fileName, string cornerJudge, string imageJudgeTime
            , int roi_x1, int roi_x2, int roi_x3, int roi_y, int roi_w, int roi_h
            , string roiJudge1, string roiJudgeCode1, string roiJudgeScore1
            , string roiJudge2, string roiJudgeCode2, string roiJudgeScore2
            , string roiJudge3, string roiJudgeCode3, string roiJudgeScore3
            )
        {
            ImageData imageData = new ImageData
            {
                CELL_ID = cellId,
                CORNER_ID = cornerId,
                IMAGE_PATH = imagePath,
                IMAGE_NAME = fileName,
                CORNER_JUDGE = cornerJudge,
                IMAGE_JUDGE_TIME = imageJudgeTime,
                INDEX_1_LABEL = string.Format("{0},{1},{2},{3},{4},{5},{6}", roi_x1.ToString(), roi_y.ToString(), roi_w.ToString(), roi_h.ToString(), roiJudge1, roiJudgeCode1, roiJudgeScore1),
                INDEX_2_LABEL = string.Format("{0},{1},{2},{3},{4},{5},{6}", roi_x2.ToString(), roi_y.ToString(), roi_w.ToString(), roi_h.ToString(), roiJudge2, roiJudgeCode2, roiJudgeScore2),
                INDEX_3_LABEL = string.Format("{0},{1},{2},{3},{4},{5},{6}", roi_x3.ToString(), roi_y.ToString(), roi_w.ToString(), roi_h.ToString(), roiJudge3, roiJudgeCode3, roiJudgeScore3),
            };

            imageList.Add(imageData);
        }

        /// <summary>
        /// 
        /// </summary>
        private class ImageData
        {
            public string CELL_ID { get; set; }
            public string CORNER_ID { get; set; }
            public string IMAGE_PATH { get; set; }
            public string IMAGE_NAME { get; set; }
            public string CORNER_JUDGE { get; set; }
            public string IMAGE_JUDGE_TIME { get; set; }

            // public ImageInfo IMAGE_INFO { get; set; }

            public string INDEX_1_LABEL { get; set; }
            public string INDEX_2_LABEL { get; set; }
            public string INDEX_3_LABEL { get; set; }
        }

        private class ImageList
        {
            public List<ImageData> IMAGE_LIST { get; set; }
        }
    }
}