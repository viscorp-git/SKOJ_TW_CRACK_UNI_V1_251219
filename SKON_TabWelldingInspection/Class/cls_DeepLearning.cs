using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using SKON_TabWelldingInspection.Class;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using ViDi2;
using ViDi2.Local;
using static Class.cls_Vision_Info;
using static SKON_TabWelldingInspection.cls_DeepLearning;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Exception = System.Exception;
using IControl = ViDi2.Runtime.IControl;
using IGreenTool = ViDi2.Runtime.IGreenTool;
using IStream = ViDi2.Runtime.IStream;
using IWorkspace = ViDi2.Runtime.IWorkspace;
using Point = System.Drawing.Point;

namespace SKON_TabWelldingInspection
{
    [Serializable]
    public class cls_DeepLearning
    {

        private IControl _control;
        private IWorkspace _workspace;

        public IWorkspace Workspace
        {
            get { return _workspace; }
            set
            {
                _workspace = value;
            }
        }

        public string WorkspaceName
        {
            
            get { return _control.Workspaces.Names[0]; }
          
        }

        public void Dispose()
        {
            _workspace.Close();
            _control.Dispose();
        }

        public bool Init(string strWorkSpacePath)
        {
            try
            {  
                _control = new ViDi2.Runtime.Local.Control(GpuMode.Deferred);
                _control.InitializeComputeDevices(GpuMode.SingleDevicePerTool, new List<int>() { });
                _workspace = _control.Workspaces.Add(Path.GetFileNameWithoutExtension(strWorkSpacePath), strWorkSpacePath);

                return true;
            }
            catch (ViDi2.Exception)
            {
                return false;
            }
        }

        private IStream GetStream(ViDi2.Runtime.IWorkspace workspace, string streamName)
        {
            IStream stream;

            try
            {
                stream = workspace.Streams[streamName];
                return stream;
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Stream not found error: {ex.Message}");
                return null;
            }
        }

        public Bitmap RunLocate(Bitmap image2, cls_RunResultJson runResultJson)
        {
            try
            {
                IStream stream = GetStream(_workspace, "0");

                using (FormsImage image = new FormsImage((Bitmap)image2.Clone()))
                {
                    ITool blueTool = stream.Tools["Locate"];

                    using (ISample sample = blueTool.Process(image))
                    {
                        //sample.Process(blueTool);
                        IBlueMarking blueMarking = sample.Markings[blueTool.Name] as IBlueMarking;

                        double x1 = 0, y1 = 0, x2 = 0, y2 = 0;

                        // 기준점 좌표
                        double x1_origin = 354;
                        double x2_origin = 2094;

                        foreach (IBlueView view in blueMarking.Views)
                        {
                            Dictionary<double, double> featureXY = new Dictionary<double, double>();
                            // 좌표 특정이 2개가 아닌 경우 잘못된것으로 보고 pass
                            if (view.Features.Count != 2)
                                continue;

                            // Score가 낮을 경우 Pass
                            foreach (var view_feature in view.Features)
                            {
                                if (view_feature.Score < 0.99)
                                    continue;
                            }

                            for (int i = 0; i < view.Features.Count; i++)
                            {
                                double x = view.Features[i].Position.X;
                                double y = view.Features[i].Position.Y;
                                featureXY.Add(x, y);
                            }

                            // 기준값과의 차이의 절대값을 계산해서 정렬
                            List<double> x1_diff = featureXY.Keys.OrderBy(x => Math.Abs(x - x1_origin)).ToList();
                            List<double> x2_diff = featureXY.Keys.OrderBy(x => Math.Abs(x - x2_origin)).ToList();

                            // 가장 가까운 값 1개 가져오기
                            x1 = x1_diff[0];
                            x2 = x2_diff[0];

                            // x1, x2에 해당하는 y 값 가져오기
                            y1 = featureXY[x1];
                            y2 = featureXY[x2];

                            runResultJson.BLUTOL_STRD1_X = x1;
                            runResultJson.BLUTOL_STRD1_Y = y1;
                            runResultJson.BLUTOL_STRD2_X = x2;
                            runResultJson.BLUTOL_STRD2_Y = y2;
                        }
                        return ProcessImage(x1, x2, y1, y2, image, runResultJson);
                    }
                }
            }
            catch { throw; }
        }

        private Bitmap ProcessImage(double x1, double x2, double y1, double y2, IImage image, cls_RunResultJson runResultJson)
        {
            double x1_origin = 354;
            double x2_origin = 2094;
            double y1_origin = 1270;
            double y2_origin = 1270;

            double maxMoveX = 150;  // x 의 원점이 124라서 120 이상은 이동하지 않도록 한다.
            double maxMoveY = 150;

            double distance_x1 = Math.Abs(x1 - x1_origin);
            double distance_x2 = Math.Abs(x2 - x2_origin);
            double distance_y1 = Math.Abs(y1 - y1_origin);
            double distance_y2 = Math.Abs(y2 - y2_origin);

            try
            {
                // 이동거리가 너무 크게 나오면 잘못된 값으로 위치 이동 하지 않는다.(원본 이미지 반환)
                if (distance_x1 > maxMoveX || distance_x2 > maxMoveX || distance_y1 > maxMoveY || distance_y2 > maxMoveY)
                    return new Bitmap(image.Bitmap);

                using (Bitmap image2 = new Bitmap((Bitmap)image.Bitmap.Clone()))
                {
                    Mat matImg = BitmapToMat(image2);

                    // 이미지 회전 각도 계산
                    double angle = Math.Atan2(y2 - y1, x2 - x1) * (180 / Math.PI);

                    runResultJson.BLUTOL_ROTAT_ANGL = angle;

                    // 이미지 이동 x1 기준으로 이동
                    float moveX = (float)(1224 - (x1 + (x2 - x1) / 2));
                    float moveY = (float)(y1 - y1_origin);

                    matImg = RotateImageOpenCV(matImg, angle);  // 이미지 회전
                    matImg = MoveImageOpenCV(matImg, moveX, -moveY);    // 이미지 이동

                    return MatToBitmap(matImg);
                }
            }
            catch (Exception)
            {
                return new Bitmap(image.Bitmap); // 오류 시 원본 반환
            }
        }

        // 이미지 이동 (Translation)
        private static Mat MoveImageOpenCV(Mat src, float shiftX, float shiftY)
        {
            Mat dst = new Mat();
            Mat translationMatrix = Cv2.GetRotationMatrix2D(new Point2f(0, 0), 0, 1);
            translationMatrix.Set<double>(0, 2, shiftX);
            translationMatrix.Set<double>(1, 2, shiftY);
            Cv2.WarpAffine(src, dst, translationMatrix, src.Size());
            return dst;
        }

        // 이미지 회전
        private static Mat RotateImageOpenCV(Mat src, double angle)
        {
            Mat dst = new Mat();
            Point2f center = new Point2f(src.Cols / 2, src.Rows / 2);
            Mat rotationMatrix = Cv2.GetRotationMatrix2D(center, angle, 1);
            Cv2.WarpAffine(src, dst, rotationMatrix, src.Size());
            return dst;
        }

        // Threshold 적용을 위한 Class 정의 2024-06-30
        public class ThresholdTag
        {
            public string Name { get; set; }
            public double Score { get; set; }
            public int Rank_Score { get; set; }
            public double Threshold { get; set; }
            public bool ThresholdPass { get; set; }

            // 생성자를 이용하여 Threshold 속성 초기화
            public ThresholdTag(string name, double score, double threshold)
            {
                Name = name;
                Score = score;
                Threshold = threshold;
                ThresholdPass = Score > Threshold; // 초기값 설정:threshold 통과 여부 확인
    
            }
        }



        public void RunVpdl(IImage image, cls_RunResultJson runResultJson)
        {
            IStream stream = GetStream(_workspace, runResultJson.CORNER_ID);

            try
            {
                ITool greenTool;

                greenTool = stream.Tools["Classify"];

                // Allocates a sample with the image
                //using (ISample sample = stream.CreateSample(image))
                using (ISample sample = greenTool.Process(image))
                {
                    //ITool greenTool;
                    //
                    //greenTool = stream.Tools["Classify"];
                    IManualRegionOfInterest greenRoi = greenTool.RegionOfInterest as ViDi2.IManualRegionOfInterest;

                    runResultJson.ROI_W = greenRoi.Parameters.Size.Width;
                    runResultJson.ROI_HT = greenRoi.Parameters.Size.Height;
                    runResultJson.ROI_X = greenRoi.Parameters.Offset.X;
                    runResultJson.ROI_Y = greenRoi.Parameters.Offset.Y;

                    //sample.Process(greenTool);
                    IGreenMarking greenMarking;
                    greenMarking = sample.Markings[greenTool.Name] as IGreenMarking;

                    if (greenMarking.Views.Count >= 3)
                        separateROI(greenMarking, runResultJson);
                    else if (greenMarking.Views.Count == 1)
                        uniROI(greenMarking, runResultJson);

                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"System error: {ex.Message}");
            }
        }

        private void uniROI(IGreenMarking greenMarking, cls_RunResultJson runResultJson)
        {
            try
            {
                IGreenView view = greenMarking.Views[0];

                double threshold = 0.0;

                if (runResultJson.CORNER_NAME.Equals("Cathode"))
                    runResultJson.INFRCE2_OK_THRHLD = runResultJson.CA_ROI_1_THRHLD;
                else
                    runResultJson.INFRCE2_OK_THRHLD = runResultJson.AN_ROI_1_THRHLD;

                // Threshold 값 정의
                threshold = runResultJson.INFRCE2_OK_THRHLD;

                // ThresholdTag 객체들을 초기화하는 리스트 생성
                // Threshold를 OK만 사용하는것으로 바뀌면서 NG Threshold는 0으로 고정 (NG는 모두 ThresholdPass : True) 2024-11-13
                // Binary or MultiClass 모두 적용되도록 한다.
                List<ThresholdTag> thresholdTags = new List<ThresholdTag>();
                if (view.Tags.Count == 2)
                {
                    thresholdTags.Add(new ThresholdTag("OK", view.Tags.FirstOrDefault(tag => tag.Name.Contains("00") || tag.Name.Contains("OK"))?.Score ?? 0, threshold));
                    thresholdTags.Add(new ThresholdTag("NG", view.Tags.FirstOrDefault(tag => tag.Name.Contains("87") || tag.Name.Contains("NG"))?.Score ?? 0, 0));
                }
                else
                {
                    thresholdTags.Add(new ThresholdTag("00_OK", view.Tags.FirstOrDefault(tag => tag.Name.Contains("00"))?.Score ?? 0, threshold));
                    thresholdTags.Add(new ThresholdTag("81_NG", view.Tags.FirstOrDefault(tag => tag.Name.Contains("81"))?.Score ?? 0, 0));
                    thresholdTags.Add(new ThresholdTag("86_NG", view.Tags.FirstOrDefault(tag => tag.Name.Contains("86"))?.Score ?? 0, 0));
                    thresholdTags.Add(new ThresholdTag("87_NG", view.Tags.FirstOrDefault(tag => tag.Name.Contains("87"))?.Score ?? 0, 0));
                }

                // thresholdTags 리스트에서 score에 따라 정렬하고 rank 할당
                var sortedTags = thresholdTags.OrderByDescending(tag => tag.Score).ToList();

                // ThresholdPass가 true인 ThresholdTag 중에서 가장 높은 Score를 가진 객체 추출
                var bestTag = sortedTags.Where(tag => tag.ThresholdPass)
                                        .OrderByDescending(tag => tag.Score)
                                        .FirstOrDefault();

                if (bestTag != null)
                {
                    if (bestTag.Name.Contains("OK"))
                    {
                        runResultJson.CORNER_JUDGE_STR = "OK";
                        runResultJson.CORNER_JUDGE_CD = "00"; // OK일 때는 00으로
                        runResultJson.CORNER_JUDGE_PLC_CD = 1;
                    }
                    else if (bestTag.Name.Contains("NG"))
                    {
                        runResultJson.CORNER_JUDGE_STR = "NG";

                        // Name에서 숫자 추출
                        string numericPart = new string(bestTag.Name.Where(char.IsDigit).ToArray());

                        // 숫자가 있으면 사용, 없으면 기본값 "01"
                        if (!string.IsNullOrEmpty(numericPart))
                            runResultJson.CORNER_JUDGE_CD = numericPart;
                        else
                            runResultJson.CORNER_JUDGE_CD = "87";   // 수정될 대상 (정해지지 않음)

                        runResultJson.CORNER_JUDGE_PLC_CD = (short)Convert.ToInt32(runResultJson.CORNER_JUDGE_CD);

                        // NG일 경우 Heatmap 저장
                        runResultJson.COMB_HEATMAP_IMAGE = greenMarking.Views[0].HeatMap != null ? greenMarking.Views[0].HeatMap.Bitmap : null;
                    }
                }
                else
                    return;

                if (view.Tags.Count == 2)
                {
                    foreach (var tag in view.Tags)
                    {
                        if (tag.Name.Contains("00")) { runResultJson.AI_INSP_OK_SCOR = tag.Score; }
                    }
                }
                else
                {
                    foreach (var tag in view.Tags)
                    {
                        if (tag.Name.Contains("00")) { runResultJson.AI_INSP_OK_SCOR = tag.Score; }
                        else if (tag.Name.Contains("81")) { runResultJson.AI_INSP_NG1_SCOR = tag.Score; }
                        else if (tag.Name.Contains("86")) { runResultJson.AI_INSP_NG2_SCOR = tag.Score; }
                        else if (tag.Name.Contains("87")) { runResultJson.AI_INSP_NG3_SCOR = tag.Score; }
                        else { runResultJson.AI_INSP_NG4_SCOR = tag.Score; }
                    }
                }

                runResultJson.CORNER_JUDGE = "CORNER_JUDGE_CD_" + runResultJson.CORNER_JUDGE_CD;
                runResultJson.AI_INSP_RSLT_CD = runResultJson.CORNER_JUDGE;
                runResultJson.AI_INSP_DTM = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }
            catch { throw; }
        }

        private void separateROI(IGreenMarking greenMarking, cls_RunResultJson runResultJson)
        {
            try
            {
                int i = 0;
                int NGcnt = 0;
                int NGcut = 0;
                string NGstr = "";


                foreach (IGreenView view in greenMarking.Views)
                {
                    string roiJudge;
                    string roiJudgeCode;

                    double threshold = 0.0;

                    // Cathode, Anode + view Index에 따라 Threshold 값 정의
                    if (runResultJson.CORNER_NAME.Equals("Cathode"))
                    {
                        if (i == 0) threshold = runResultJson.CA_ROI_1_THRHLD;
                        if (i == 1) threshold = runResultJson.CA_ROI_2_THRHLD;
                        if (i == 2) threshold = runResultJson.CA_ROI_3_THRHLD;
                    }
                    else
                    {
                        if (i == 0) threshold = runResultJson.AN_ROI_1_THRHLD;
                        if (i == 1) threshold = runResultJson.AN_ROI_2_THRHLD;
                        if (i == 2) threshold = runResultJson.AN_ROI_3_THRHLD;
                    }

                    // 여기서 부터 threshold 적용을 위한 새로운 코드 2024-06-30
                    // ThresholdTag 객체들을 초기화하는 리스트 생성
                    // Threshold를 OK만 사용하는것으로 바뀌면서 NG Threshold는 0으로 고정 (NG는 모두 ThresholdPass : True) 2024-11-13
                    List<ThresholdTag> thresholdTags = new List<ThresholdTag>
                        {
                            new ThresholdTag("00", view.Tags.FirstOrDefault(tag => tag.Name.Contains("00"))?.Score ?? 0, threshold),
                            new ThresholdTag("81", view.Tags.FirstOrDefault(tag => tag.Name.Contains("81"))?.Score ?? 0, 0),
                            new ThresholdTag("86", view.Tags.FirstOrDefault(tag => tag.Name.Contains("86"))?.Score ?? 0, 0),
                            new ThresholdTag("87", view.Tags.FirstOrDefault(tag => tag.Name.Contains("87"))?.Score ?? 0, 0)
                        };

                    // thresholdTags 리스트에서 score에 따라 정렬하고 rank 할당
                    var sortedTags = thresholdTags.OrderByDescending(tag => tag.Score).ToList();

                    //int rank = 1;
                    //foreach (var tag in sortedTags)
                    //{
                    //    tag.Rank_Score = rank;
                    //    rank++;
                    //}

                    // ThresholdPass가 true인 ThresholdTag 중에서 가장 높은 Score를 가진 객체 추출
                    var bestTag = sortedTags.Where(tag => tag.ThresholdPass)
                                           .OrderByDescending(tag => tag.Score)
                                           .FirstOrDefault();

                    string bestTagName;

                    if (bestTag != null)
                    {
                        bestTagName = bestTag.Name;
                    }
                    else
                    {
                        // Threshold를 모두 통과 못할 경우, 그래도 뭐라도 줘야해서 87로 함. (방어로직이지만 해당되는 사항 없을 것임)
                        bestTagName = "87";
                    }

                    if (bestTagName == "00")
                    {
                        roiJudge = "OK";
                        roiJudgeCode = "00";
                    }
                    else if (bestTagName == "81")
                    {
                        roiJudge = "NG";
                        roiJudgeCode = "81";
                        NGcut++;
                        NGcnt++;
                        NGstr += roiJudgeCode;
                    }
                    else if (bestTagName == "86")
                    {
                        roiJudge = "NG";
                        roiJudgeCode = "86";
                        NGcnt++;
                        NGstr += roiJudgeCode;
                    }
                    else if (bestTagName == "87")
                    {
                        roiJudge = "NG";
                        roiJudgeCode = "87";
                        NGcnt++;
                        NGstr += roiJudgeCode;
                    }
                    else
                    {
                        roiJudge = "NG";
                        roiJudgeCode = "99";
                        NGcnt++;
                        NGstr += roiJudgeCode;
                    }

                    if (i == 0)
                    {
                        runResultJson.ROI1_INFRCE2_RST_STR = roiJudge;
                        runResultJson.ROI1_INFRCE2_RST_CD = roiJudgeCode;
                        runResultJson.ROI1_INFRCE2_RST = "CORNER_JUDGE_CD_" + roiJudgeCode;   // ROI1_추론2_결과
                        runResultJson.ROI1_HEATMAP_IMAGE = greenMarking.Views[0].HeatMap.Bitmap;

                        // 모든 class 의 스코어값 저장
                        foreach (var tag in view.Tags)
                        {
                            if (tag.Name.Contains("01")) { runResultJson.ROI1_INFRCE2_OK_SCOR = tag.Score; }
                            else if (tag.Name.Contains("81")) { runResultJson.ROI1_INFRCE2_NG1_SCOR = tag.Score; }
                            else if (tag.Name.Contains("86")) { runResultJson.ROI1_INFRCE2_NG2_SCOR = tag.Score; }
                            else if (tag.Name.Contains("87")) { runResultJson.ROI1_INFRCE2_NG3_SCOR = tag.Score; }
                            else { runResultJson.ROI1_INFRCE2_NG4_SCOR = tag.Score; }
                        }
                    }
                    else if (i == 1)
                    {
                        runResultJson.ROI2_INFRCE2_RST_STR = roiJudge;
                        runResultJson.ROI2_INFRCE2_RST_CD = roiJudgeCode;
                        runResultJson.ROI2_INFRCE2_RST = "CORNER_JUDGE_CD_" + roiJudgeCode;   // ROI1_추론2_결과
                        runResultJson.ROI2_HEATMAP_IMAGE = greenMarking.Views[1].HeatMap.Bitmap;

                        // 모든 class 의 스코어값 저장
                        foreach (var tag in view.Tags)
                        {
                            if (tag.Name.Contains("01")) { runResultJson.ROI2_INFRCE2_OK_SCOR = tag.Score; }
                            else if (tag.Name.Contains("81")) { runResultJson.ROI2_INFRCE2_NG1_SCOR = tag.Score; }
                            else if (tag.Name.Contains("86")) { runResultJson.ROI2_INFRCE2_NG2_SCOR = tag.Score; }
                            else if (tag.Name.Contains("87")) { runResultJson.ROI2_INFRCE2_NG3_SCOR = tag.Score; }
                            else { runResultJson.ROI2_INFRCE2_NG4_SCOR = tag.Score; }
                        }
                    }
                    else if (i == 2)
                    {
                        runResultJson.ROI3_INFRCE2_RST_STR = roiJudge;
                        runResultJson.ROI3_INFRCE2_RST_CD = roiJudgeCode;
                        runResultJson.ROI3_INFRCE2_RST = "CORNER_JUDGE_CD_" + roiJudgeCode;   // ROI1_추론2_결과
                        runResultJson.ROI3_HEATMAP_IMAGE = greenMarking.Views[2].HeatMap.Bitmap;

                        // 모든 class 의 스코어값 저장
                        foreach (var tag in view.Tags)
                        {
                            if (tag.Name.Contains("01")) { runResultJson.ROI3_INFRCE2_OK_SCOR = tag.Score; }
                            else if (tag.Name.Contains("81")) { runResultJson.ROI3_INFRCE2_NG1_SCOR = tag.Score; }
                            else if (tag.Name.Contains("86")) { runResultJson.ROI3_INFRCE2_NG2_SCOR = tag.Score; }
                            else if (tag.Name.Contains("87")) { runResultJson.ROI3_INFRCE2_NG3_SCOR = tag.Score; }
                            else { runResultJson.ROI3_INFRCE2_NG4_SCOR = tag.Score; }
                        }
                    }
                    i++;
                }


                // NG가 1개 이상 포함되어 있으면 기본적으로 NG 처리
                if (NGcnt > 0)
                {
                    // 1:ok00 2:완전단선84 3:부분단선81 4:스크랩86 5:데미지87
                    // NG우선순위 : 86 84 87 81
                    if (NGstr.Contains("86"))
                    {
                        runResultJson.CORNER_JUDGE_STR = "NG";
                        runResultJson.CORNER_JUDGE_CD = "86";
                        runResultJson.CORNER_JUDGE_PLC_CD = 86;
                    }
                    else if (NGcut == 3)  // 단선이 3개면 완전 단선
                    {
                        runResultJson.CORNER_JUDGE_STR = "NG";
                        runResultJson.CORNER_JUDGE_CD = "84";
                        runResultJson.CORNER_JUDGE_PLC_CD = 84;
                    }
                    else if (NGstr.Contains("87"))
                    {
                        runResultJson.CORNER_JUDGE_STR = "NG";
                        runResultJson.CORNER_JUDGE_CD = "87";
                        runResultJson.CORNER_JUDGE_PLC_CD = 87;
                    }
                    else if (NGcut == 2)  // 단선이 2개면 부분 단선
                    {
                        runResultJson.CORNER_JUDGE_STR = "NG";
                        runResultJson.CORNER_JUDGE_CD = "81";
                        runResultJson.CORNER_JUDGE_PLC_CD = 81;
                    }
                    else if (NGcut == 1 && NGcnt == 1)  // 단선이 1개만 있으면 OK 처리함. ==> 단선 한개라도 NG로 보고하도록 요청에 의한 변경 250402
                    {
                        runResultJson.CORNER_JUDGE_STR = "NG";
                        runResultJson.CORNER_JUDGE_CD = "81";
                        runResultJson.CORNER_JUDGE_PLC_CD = 81;
                    }
                    else
                    {
                        runResultJson.CORNER_JUDGE_STR = "";
                        runResultJson.CORNER_JUDGE_CD = "";
                        runResultJson.CORNER_JUDGE_PLC_CD = 0;
                    }
                }
                else
                {
                    runResultJson.CORNER_JUDGE_STR = "OK";
                    runResultJson.CORNER_JUDGE_CD = "00";
                    runResultJson.CORNER_JUDGE_PLC_CD = 1;
                }

                runResultJson.CORNER_JUDGE = "CORNER_JUDGE_CD_" + runResultJson.CORNER_JUDGE_CD;
                runResultJson.AI_INSP_DTM = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

                //Heatmap 저장 기능 추가
                // NG가 하나라도 존재한다면, 히트맵을 저장 (NG ROI만 저장됨)
                // ROI별 NG라면 히트맵 기록 OK라면 기록하지 않음.
                if (NGcnt > 0)
                {
                    runResultJson.COMB_HEATMAP_IMAGE = new Bitmap((int)runResultJson.ROI_W, (int)runResultJson.ROI_HT);
                    using (Graphics g = Graphics.FromImage(runResultJson.COMB_HEATMAP_IMAGE))
                    {
                        if (runResultJson.ROI1_HEATMAP_IMAGE != null && runResultJson.ROI1_INFRCE2_RST_STR != "OK") g.DrawImage(runResultJson.ROI1_HEATMAP_IMAGE, new Point(0, 0));
                        if (runResultJson.ROI2_HEATMAP_IMAGE != null && runResultJson.ROI2_INFRCE2_RST_STR != "OK") g.DrawImage(runResultJson.ROI2_HEATMAP_IMAGE, new Point((int)runResultJson.ROI_W / 3, 0));
                        if (runResultJson.ROI3_HEATMAP_IMAGE != null && runResultJson.ROI3_INFRCE2_RST_STR != "OK") g.DrawImage(runResultJson.ROI3_HEATMAP_IMAGE, new Point((int)runResultJson.ROI_W / 3 * 2, 0));
                    }
                }
            }
            catch { throw; }
        }

        public bool AddLoadModel(string strWorkSpacePath)
        {
            try
            {
                if (_workspace == null)
                {
                    // 워크스페이스가 제대로 로딩이 안되면 초기화 작업부터 다시 진행
                    Init(strWorkSpacePath);
                    // 그래도 로드가 안되면 실패 반환
                    if(_workspace == null)
                    {
                        return false;
                    }
                } 
                else
                {
                    _workspace = _control.Workspaces.Add(Path.GetFileNameWithoutExtension(strWorkSpacePath), strWorkSpacePath);  
                    
                }               

                return true;
            }
            catch (ViDi2.Exception)
            {
                return false;
            }
        }

        public bool UnloadModel(string workspaceName)
        {
            try
            {
                _control.Workspaces[workspaceName].Close();
                _control.Workspaces.Remove(workspaceName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        // Bitmap -> Mat 변환
        public static Mat BitmapToMat(Bitmap bitmap)
        {
            try
            {
                return BitmapConverter.ToMat(bitmap);
            }
            catch { throw; }
        }

        // Mat -> Bitmap 변환
        public static Bitmap MatToBitmap(Mat mat)
        {
            try
            {
                return BitmapConverter.ToBitmap(mat);
            }
            catch { throw; }
        }
    }
}