using AForge;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Class.cls_Util;

namespace SKON_TabWelldingInspection.Class
{
    public class cls_detectAnomal
    {
        private static readonly object lockObj = new object();

        // 불량 연속 판정
        public bool NGCellCheckResult(string gubun, string judge, ref int ngCount, int Count_threshold)
        {
            bool isOk = judge.Equals("OK");

            // 이상범위라면 카운트 증가
            // 정상범위라면 카운트 초기화
            if (!isOk)
                ngCount += 1;
            else
                ngCount = 0;

            if (ngCount >= Count_threshold)
                return true;
            else
                return false;
        }

        public bool NGCellIntervalCheckResult(string gubun, string judge, ref Queue<bool> ngHistory, int maxCount, int threshold)
        {
            // 판정 결과 추가 (NG는 false, OK는 true로 기록)
            bool isOk = judge.Equals("OK");
            ngHistory.Enqueue(isOk);

            // 최대 카운트를 초과한 오래된 데이터 제거
            while (ngHistory.Count > maxCount)
            {
                ngHistory.Dequeue();
            }

            // 불량 갯수 계산
            int ngCount = ngHistory.Count(result => !result);

            // 불량 갯수가 threshold Count를 초과하면 true 반환
            return ngCount >= threshold;
        }

        // 불량 연속 판정 (특정 주기내에)
        public bool NGCellIntervalCheckResult(string gubun, string judge, ref Queue<DateTime> ngHistory, int Count_threshold, TimeSpan retentionPeriod)
        {
            int ngCount = 0;

            bool isOk = judge.Equals("OK") ? true : false;

            // 이상범위라면 카운트 증가
            if (!isOk)
                ngHistory.Enqueue(DateTime.Now);

            lock (lockObj)
            {
                // 오래된 데이터 제거
                DateTime cutoffTime = DateTime.Now - retentionPeriod;
                while (ngHistory.Count > 0 && ngHistory.Peek() < cutoffTime)
                {
                    ngHistory.Dequeue();
                }

                // NG 개수 판단
                ngCount = ngHistory.Count;
            }

            if (ngCount >= Count_threshold)
                return true;
            else
                return false;
        }

        // 위치 보정 여부 판정
        public bool locationCheckResult(bool isActiveLocate, double x1, double y1, double x2, double y2, double angle, ref int ngCount, int Count_threshold)
        {
            if (!isActiveLocate)
            {
                ngCount = 0;
                return false;
            }
                
            bool isOk = true;

            // 축들이 모두  0 + 각도도 0 이라면 보정 실패로 판단
            if (x1 == 0 && y1 == 0 && x2 == 0 && y2 == 0 && angle == 0)
                isOk = false;

            // 이상범위라면 카운트 증가
            // 정상범위라면 카운트 초기화
            if (!isOk)
                ngCount += 1;
            else
                ngCount = 0;

            if (ngCount >= Count_threshold)
                return true;
            else
                return false;
        }

        // 밝기 측정값 판정
        public bool brightCheckResult(string gubun, double value, double stdVal, double lowerBound, double upperBound, ref int ngCount, int Count_threshold)
        {

            // 밝기 측정값 판정
            bool isOk = value >= (stdVal - lowerBound) && value <= (stdVal + upperBound);

            // 이상범위라면 카운트 증가
            // 정상범위라면 카운트 초기화
            if (!isOk)
                ngCount += 1;
            else
                ngCount = 0;

            if (ngCount >= Count_threshold)
                return true;
            else
                return false;
        }

        // 선명도 측정값 판정
        public bool sharpnessCheckResult(string gubun, double value, double stdVal, double lowerBound, double upperBound, ref int ngCount, int Count_threshold)
        {
            bool isOk = value >= (stdVal - lowerBound);

            // 이상범위라면 카운트 증가
            // 정상범위라면 카운트 초기화
            if (!isOk)
                ngCount += 1;
            else
                ngCount = 0;

            if (ngCount >= Count_threshold)
                return true;
            else
                return false;
        }

        // 밝기 Pixel 평균값 계산
        public double CalculateAverage(int[] values, int rank_qty)
        {
            int[] sortedAverages = (int[])values.Clone();
            Array.Sort(sortedAverages);
            Array.Reverse(sortedAverages);

            int[] topValues = new int[rank_qty];
            Array.Copy(sortedAverages, topValues, rank_qty);

            List<int> topIndexes = new List<int>();
            for (int i = 0; i < values.Length; i++)
            {
                if (topValues.Contains(values[i]) && !topIndexes.Contains(i))
                {
                    topIndexes.Add(i);
                    if (topIndexes.Count == rank_qty) break;
                }
            }

            double topAverageIndex = Math.Round(topIndexes.Average());
            return topAverageIndex;
        }

        // 밝기 히스토그램 계산 함수
        public int[] CalculateHistogram(Mat roiMat)
        {
            Mat grayMat = new Mat();
            Cv2.CvtColor(roiMat, grayMat, ColorConversionCodes.BGR2GRAY);

            int[] histSize = { 256 };
            Rangef[] ranges = { new Rangef(0, 256) };

            Mat hist = new Mat();
            Cv2.CalcHist(new Mat[] { grayMat }, new int[] { 0 }, null, hist, 1, histSize, ranges);

            int[] histogram = new int[256];
            for (int i = 0; i < histogram.Length; i++)
            {
                histogram[i] = (int)hist.Get<float>(i);
            }

            return histogram;
        }

        // 선명도 계산 함수
        public double CalculateSharpness(Mat image)
        {
            // 이미지를 그레이스케일로 변환
            Mat grayImage = new Mat();
            Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);

            // 라플라시안(Laplacian) 필터 적용
            Mat laplacian = new Mat();
            Cv2.Laplacian(grayImage, laplacian, MatType.CV_64F);

            // 라플라시안 결과의 분산(Variance)을 계산하여 선명도 측정
            Scalar mean, stddev;
            Cv2.MeanStdDev(laplacian, out mean, out stddev);

            return stddev.Val0;
        }
    }
}
