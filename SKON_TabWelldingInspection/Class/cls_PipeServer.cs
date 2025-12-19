using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Imaging.Filters;
using Cognex.VisionPro.Implementation.Internal;
using System.Globalization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace SKON_TabWelldingInspection.Class
{
    public class cls_PipeServer
    {
        private frm_Main mainForm;
        private bool isProcessingPerformAction = false; // PerformAction이 처리중인지 나타내는 플래그 추가 2024-07-15

        public cls_PipeServer(frm_Main form, string apiName, int apiThreadNum)
        {
            mainForm = form;

            // pipServer은 1대 1로만 기능한다고 해서 3개를 실행하도록 수정함. 2024-07-04
            for (int i = 0; i < apiThreadNum; i++)
            {
                Task.Run(() => ListenForRequests(apiName, apiThreadNum));                
            }            
        }

        private async Task ListenForRequests(string apiName, int apiThreadNum)
        {
            while (true)
            {
                NamedPipeServerStream pipeServer = null;
                StreamReader reader = null;
                StreamWriter writer = null;
                try
                {
                    // pipeRestAPI 명은 받는 쪽과 동일해야 한다.
                    pipeServer = new NamedPipeServerStream(apiName, PipeDirection.InOut, apiThreadNum, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

                    await pipeServer.WaitForConnectionAsync();
                    reader = new StreamReader(pipeServer);
                    writer = new StreamWriter(pipeServer) { AutoFlush = true };

                    var message = await reader.ReadLineAsync();
                    if (message != null)
                    {
                        // PerformAction이 처리중인지 나타내는 플래그 추가 2024-07-15
                        if (isProcessingPerformAction)
                        {
                            if (pipeServer.IsConnected)
                            {
                                await writer.WriteLineAsync("13|System is busy");
                                await writer.FlushAsync();
                            }
                        }
                        else
                        {
                            isProcessingPerformAction = true; // PerformAction 시작
                            string result = PerformAction(message);
                            isProcessingPerformAction = false; // PerformAction 완료

                            if (pipeServer.IsConnected)
                            {
                                await writer.WriteLineAsync(result);
                                await writer.FlushAsync(); // Pipe 전송이 완전히 완료될때까지 대기
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    if (pipeServer != null && pipeServer.IsConnected)
                    {
                        await writer.WriteLineAsync($"11|{ex.Message}");
                    }
                }
                finally
                {
                    if (pipeServer != null)
                    {
                        try
                        {
                            pipeServer.Dispose();
                        }
                        catch (Exception ex)
                        {
                            if (pipeServer.IsConnected)
                            {
                                await writer.WriteLineAsync($"11|{ex.Message}");
                            }
                        }
                    }
                    if (reader != null)
                    {
                        try
                        {
                            reader.Dispose();
                        }
                        catch (Exception ex)
                        {
                            if (pipeServer != null && pipeServer.IsConnected)
                            {
                                await writer.WriteLineAsync($"11|{ex.Message}");
                            }
                        }
                    }
                    if (writer != null)
                    {
                        try
                        {
                            writer.Dispose();
                        }
                        catch (Exception ex)
                        {
                            if (pipeServer != null && pipeServer.IsConnected)
                            {
                                await writer.WriteLineAsync($"11|{ex.Message}");
                            }
                        }
                    }
                }
            }
        }

        public string PerformAction(string message)
        {
            try
            {
                string[] param = message.Split('|');

                if (param.Length < 2)
                {
                    return "12|Invalid Parameter ";
                }

                string paramAct = param[0];
                string paramMessage = param[1];

                if (paramAct == "update_threshold")
                {
                    return UpdateThreshold(paramMessage);
                }
                else if (paramAct == "status")
                {
                    return SelectStatus(paramMessage);
                }
                else if(paramAct == "update_model")
                {
                    return ChangeVpdlModel(paramMessage);
                }
                else
                {
                    return $"14|Invalid command";
                }                
            }
            catch (Exception ex)
            {
                return $"11|{ex.Message}";
            }
        }



        public string SelectStatus(string paramMessage)
        {
            //paramMessage = "{\"CORNER_ID\": \"1\"}";

            try
            {
                corner corner = null;

                // JSON 문자열을 TrainingData 객체로 변환
                try
                {
                    corner = JsonConvert.DeserializeObject<corner>(paramMessage);
                }
                catch (Exception ex)
                {
                    return $"42|The JSON structure is incorrect:{ex.Message}";
                }

                // ROI Value Check
                if (string.IsNullOrEmpty(corner.CORNER_ID))
                    return $"42|There are missing values in the data fields(CORNER_ID: {corner.CORNER_ID})";

                // Corner ID 유효성 (TOP 1,2 / BOTTOM 3,4)
                if (!IsValidCornerId(corner.CORNER_ID))
                    return $"42|Edge PC - [{mainForm.mPosition}] not available Corner {corner.CORNER_ID}";

                string type = string.Empty;
                if (corner.CORNER_ID == "1" || corner.CORNER_ID == "3")
                    type = "CA";
                else
                    type = "AN";

                statusReturn stsRtn = new statusReturn();

                stsRtn.MDL_FILE_NM = GetMainFormGlobalValue("MDL_FILE_NM");
                stsRtn.ROI1 = GetMainFormGlobalValue($"{type}_ROI1");
                stsRtn.ROI2 = GetMainFormGlobalValue($"{type}_ROI2");
                stsRtn.ROI3 = GetMainFormGlobalValue($"{type}_ROI3");
                
                // result json 결합
                string returnStr = $"0|{JsonConvert.SerializeObject(stsRtn)}";
                return returnStr;
            }
            catch (Exception ex)
            {
                return $"41|{ex.Message}";
            }
        }

        private string GetMainFormGlobalValue(string strGlobalValue)
        {
            switch (strGlobalValue)
            {
                case "MDL_FILE_NM":
                        return Path.GetFileName(mainForm.mWorkSpacePath);
                case "CA_ROI1":
                        // InvariantCulture는 항상 1234.56변환하는데, currentCultureString 옵션은 지역에따라 다르다. 유럽일 때, 1234,56으로 표시된다.
                        return string.Format(CultureInfo.InvariantCulture, "{0}", mainForm.mThreshold_CA_R1);
                case "CA_ROI2":
                        return string.Format(CultureInfo.InvariantCulture, "{0}", mainForm.mThreshold_CA_R2);
                case "CA_ROI3":
                        return string.Format(CultureInfo.InvariantCulture, "{0}", mainForm.mThreshold_CA_R3);
                case "AN_ROI1":
                        return string.Format(CultureInfo.InvariantCulture, "{0}", mainForm.mThreshold_AN_R1);
                case "AN_ROI2":
                        return string.Format(CultureInfo.InvariantCulture, "{0}", mainForm.mThreshold_AN_R2);
                case "AN_ROI3":
                        return string.Format(CultureInfo.InvariantCulture, "{0}", mainForm.mThreshold_AN_R3);
                default:
                    return "";
            }

        }

        // error code

        // 0|OK

        // 11|{ex.Message}  // PipeServer 관련 오류메세지
        // 12|System is busy  // 다른 작업이 완료전에 또다른 명령이 입력되었을 때
        // 13|Invalid Parameter // |로 잘랐을때 파라미터가 2개가 안될때
        // 14|Invalid command  // "update_threshold_trigger" 같은 지정된 명령어가 입력되지 않았을 때 

        // 21|{ex.Message}   // threshold update중 발생하는 오류 메세지
        // 22|Invalid Parameter  // { } 사이에 ,로 분리된 값이 한개라도 들어와야 함. 
        // 23|Invalid value : double 타입이 아닌 값이 입력될 경우
        // 24|Out of range // 0.0 < threshold < 1.0

        // 31|{ex.Message}   // Model Change중 발생하는 오류 메세지
        // 32|Invalid Parameter  // 입력 파라미터 오류
        // 33|File not found // 파일을 찾을 수 없음
        // 34|An unknown error has occurred  // 알 수 없는 오류 (오류메세지 리턴 없음)

        // 41|{ex.Message}   // Status에서 발생하는 오류 메세지
        // 42|Invalid Parameter  // 입력 파라미터 오류
        // 43|Invalid command  // 지정된 명령어가 입력되지 않았을 때 

        public class corner
        {
            public string CORNER_ID { get; set; }
        }

        public class statusReturn
        {
            public string MDL_FILE_NM { get; set; }
            public string ROI1 { get; set; }
            public string ROI2 { get; set; }
            public string ROI3 { get; set; }
        }

        public class thresholdData
        {
            public string CORNER_ID { get; set; }
            public string ROI1 { get; set; }
            public string ROI2 { get; set; }
            public string ROI3 { get; set; }
        }

        public class ThresholdSet
        {
            public List<thresholdData> THREASHOLD { get; set; }
        }

        private string UpdateThreshold(string paramMessage)
        {
            //paramMessage = @"update_threshold|"THRESHOLD": [{ "CORNER_ID": "1", "ROI1": "0.77", "ROI2": "0.53", "ROI3": "0.23"},{ "CORNER_ID": "2", "ROI1": "0.77", "ROI2": "0.53", "ROI3": "0.23"}]";
            // 파싱 후 메인 함수의 mThreshold 값을 수정함.

            try
            {
                ThresholdSet thresholdSet = null;
                List<thresholdData> thresholdList = null;

                // JSON 문자열을 TrainingData 객체로 변환
                try
                {
                    thresholdSet = JsonConvert.DeserializeObject<ThresholdSet>(paramMessage);
                    thresholdList = thresholdSet.THREASHOLD;
                }
                catch (Exception ex)
                {
                    return $"22|The JSON structure is incorrect:{ex.Message}";
                }

                if (thresholdSet == null || thresholdList == null)
                    return $"22|incorrect Parameter";
                
                if (thresholdSet?.THREASHOLD == null || thresholdSet.THREASHOLD.Count == 0)
                    return $"22|incorrect Parameter";

                // 유효값 체크
                foreach (var item in thresholdList)
                {
                    // ROI Value Check
                    if (string.IsNullOrEmpty(item.CORNER_ID))
                        return $"22|There are missing values in the data fields(CORNER_ID: {item.CORNER_ID})";

                    // Corner ID 유효성 (TOP 1,2 / BOTTOM 3,4)
                    if (!IsValidCornerId(item.CORNER_ID))
                        return $"22|Edge PC - [{mainForm.mPosition}] not available Corner {item.CORNER_ID}";

                    // 각 필드가 누락되었을 때 체크
                    bool missingAllROI = string.IsNullOrEmpty(item.ROI1) && string.IsNullOrEmpty(item.ROI2) && string.IsNullOrEmpty(item.ROI3);
                    
                    // 모든 ROI 값이 누락되었으면 리턴
                    if (missingAllROI)
                        return $"22|Missing all ROI values for CORNER_ID: {item.CORNER_ID}";

                    // ROI Double Check
                    bool roi1Valid = string.IsNullOrEmpty(item.ROI1) || double.TryParse(item.ROI1, out _);
                    bool roi2Valid = string.IsNullOrEmpty(item.ROI2) || double.TryParse(item.ROI2, out _);
                    bool roi3Valid = string.IsNullOrEmpty(item.ROI3) || double.TryParse(item.ROI3, out _);

                    if (!roi1Valid || !roi2Valid || !roi3Valid)
                        return $"22|The ROI value is not a numeric Value";

                    // ROI Min-Max Check
                    if (!IsValidROI(item.ROI1) || !IsValidROI(item.ROI2) || !IsValidROI(item.ROI3))
                        return $"24|Invalid ROI value: Out of range (under 0.0, over 1.0)";
                }

                foreach (var item in thresholdList)
                {
                    string type = string.Empty;
                    if (item.CORNER_ID == "1" || item.CORNER_ID == "3")
                        type = "CA";
                    else
                        type = "AN";

                    if (!string.IsNullOrEmpty(item.ROI1))
                        mainForm.SetThreshold($"{type}_ROI1", Convert.ToDouble(item.ROI1));
                    if (!string.IsNullOrEmpty(item.ROI2))
                        mainForm.SetThreshold($"{type}_ROI2", Convert.ToDouble(item.ROI2));
                    if (!string.IsNullOrEmpty(item.ROI3))
                        mainForm.SetThreshold($"{type}_ROI3", Convert.ToDouble(item.ROI3));
                }
                return "0|OK";
            }
            catch (Exception ex)
            {
                return $"21|{ex.Message}";
            }

        }

        private bool IsValidCornerId(string cornerId)
        {
            return mainForm.mPosition.Equals("TOP") ? (cornerId == "1" || cornerId == "2") : (cornerId == "3" || cornerId == "4");
        }

        private static bool IsValidROI(string roiValue)
        {
            if (string.IsNullOrEmpty(roiValue))
            {
                return true; // ROI 값이 없을때는 그냥 넘김.
            }

            if (double.TryParse(roiValue, out double roi))
            {
                return roi > 0.0 && roi < 1.0;
            }
            return false; // 유효값이 아님.
        }

        public string ChangeVpdlModel(string paramMessage)
        {
            try
            {
                //paramMessage = "{\"PATH\": \"D:/MODEL\",\"MODEL_NM\":\"tm_1128_01_0.1\"}";
                paramMessage = paramMessage.Replace("\\", "\\\\");
                JObject jsonStr = JObject.Parse(paramMessage);

                // PATH와 MODEL_NM 값 확인
                if (jsonStr["PATH"] == null || jsonStr["MODEL_NM"] == null)
                {
                    return "32|Invalid Parameter ";
                }

                string modelFileName = jsonStr["MODEL_NM"].ToString().Contains(".vrws") ? jsonStr["MODEL_NM"].ToString() : $"{jsonStr["MODEL_NM"].ToString()}.vrws";

                // 버전값 확인
                string[] splitData = Path.GetFileNameWithoutExtension(modelFileName).Split('_');
                if (splitData.Length < 1)
                {
                    return "32|Invalid Parameter : version Check";
                }
                
                // 맨마지막 자리가 버전 (버전이 Path 마지막에 추가되야함.)
                string version = splitData[splitData.Length - 1];

                string modelDir = $"{jsonStr["PATH"].ToString()}/{version}";

                string modelFullPath = Path.Combine(modelDir, modelFileName);

                // 파일 있는지 확인
                if (!File.Exists(modelFullPath))
                {
                    return $"33|File Not Found";
                }

                // 모델 교체 작업 시작 (메인폼에서 처리)
                var rst = mainForm.ModelChangeProcess(modelDir, modelFileName);
                if (rst.Item1)
                {
                    return "0|OK";
                }
                else
                {
                    string rtnString = string.IsNullOrEmpty(rst.Item2) ? "An unknown error has occurred." : rst.Item2;
                    return $"34|{rtnString}";
                }
            }
            catch (Exception ex)
            {
                return $"31|{ex.Message}";
            }
        }


    }
}