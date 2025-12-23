using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SKON_TabWelldingInspection.Class
{
    public delegate void LogMsgHadler(string message);
    public class cls_Log
    {
        private static readonly object logLock = new object();
        public event LogMsgHadler OnLogMsg;
        private static ConcurrentQueue<Tuple<string, string, string>> logQueue = new ConcurrentQueue<Tuple<string, string, string>>();

        private bool _keepThread = false;
        public bool keepThread
        {
            get { return _keepThread; }
            set { _keepThread = value; }
        }

        public cls_Log()
        {
        }

        private void WriteLogFile(string path, string str, string camName = null)
        {

            try
            {
                DateTime datetime = DateTime.Now;
                cls_File.CreateFolder(path);
                string fullname = Path.Combine(path, $"{datetime.ToString("yyyy_MM_dd")}_{camName}.txt");

                // using 문으로 변경 
                using (StreamWriter sw = new StreamWriter(fullname, true, Encoding.UTF8)) // 인코딩을 UTF-8로 변경  Encoding.GetEncoding("ks_c_5601-1987")
                {
                    sw.WriteLine(str);
                }
            }
            catch (Exception) 
            {
                //
            }
        }

        public void WriteLog(string part, string msg, string camName = null)
        {
            string nowDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string message = "[" + nowDate + "] [" + part + "] " + msg;
            string path;

            if (part == "PLC") 
                path = @"D:\LOG\VISION_PLC_LOG";
            else if (part == "IO") 
                path = @"D:\LOG\VISION_IO_LOG";
            else if (part == "CAM") 
                path = @"D:\LOG\VISION_CAM_LOG";
            else if (part == "ANM")
                path = @"D:\LOG\VISION_ANM_LOG";
            else if (part == "ERR")
                path = @"D:\LOG\VISION_ERR_LOG";
            else if (part == "TEST")    //test용 디렉토리
                path = @"D:\LOG\VISION_TEST_LOG";
            else
                path = @"D:\LOG\VISION_SYS_LOG";

            //WriteLogFile(path, message);
            //OnLogMsg?.Invoke(message); 

            logQueue.Enqueue(new Tuple<string, string, string>(path, message, camName));
        }

        public void WriteLogDirect(string part, string msg)
        {
            string nowDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string message = "[" + nowDate + "] [" + part + "] " + msg;
            string path;

            if (part == "PLC")
                path = @"D:\LOG\VISION_PLC_LOG";
            else if (part == "IO")
                path = @"D:\LOG\VISION_IO_LOG";
            else if (part == "CAM")
                path = @"D:\LOG\VISION_CAM_LOG";
            else if (part == "ANM")
                path = @"D:\LOG\VISION_ANM_LOG";
            else if (part == "ERR")
                path = @"D:\LOG\VISION_ERR_LOG";
            else
                path = @"D:\LOG\VISION_SYS_LOG";

            WriteLogFile(path, message);
            OnLogMsg?.Invoke(message); 
        }

        public void WriteLogQueue()
        {
        loopPoint:
            try
            {
                while (keepThread)
                {
                    //Tuple<string, string> logData = null;

                    if (logQueue.Count == 0)
                    {
                        Task.Delay(10).Wait();
                        continue;
                    }

                    if (logQueue.TryDequeue(out Tuple<string, string, string> logData))
                    {
                        string _path = logData.Item1;
                        string _message = logData.Item2;
                        string _camName = logData.Item3;

                        // 파일 기록
                        WriteLogFile(_path, _message, _camName);

                        // UI 이벤트 호출 (비동기 처리)
                        Task.Run(() => OnLogMsg?.Invoke(_message));

                        Task.Delay(10).Wait();
                    }
                }
            }
            catch (ThreadAbortException e)
            {
                Thread.ResetAbort();
            }
            catch (Exception ex)
            {
                Task.Delay(100).Wait();

                // Loop Point Jump
                goto loopPoint;
            }
        }
    }
}