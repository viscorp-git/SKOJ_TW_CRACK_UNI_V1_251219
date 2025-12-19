using SKON_TabWelldingInspection.Class;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SKON_TabWelldingInspection
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool bNew = true;
            using (Mutex mutex = new Mutex(true, "SKON_TabWelldingInspection", out bNew))
            {
                if (!bNew)
                {
                    //MessageBox.Show("Program is Alreay Running.");

                    mutex.ReleaseMutex();
                    mutex.Close();
                    mutex.Dispose();
                    Application.Exit();
                }
                else
                {
                    mutex.ReleaseMutex();

                    // 모든 스레드에 대해 문화권 정보를 영어로 설정
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    frm_Main mainForm = new frm_Main();

                    // RestAPI PipeServer
                    cls_PipeServer pipeServer = new cls_PipeServer(mainForm, "pipeRestAPI", 3);
                    //mainForm.LoadingIni();
                    //pipeServer.PerformAction("update_model|{\"PATH\":\"D:\\EXPORT\\IMAGE\\\", \"MODEL_NM\":\"SKOH_TESTMODEL_240706.vrws\"}");  // RestApi test용 코드
                    Application.Run(mainForm);
                }
            }
        }
    }
}
