using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace SKON_TabWelldingInspection
{
    class cls_LoadingProvider
    {
        private Thread thread;
        private frm_Loader form;
        private EventWaitHandle loaded;

        public cls_LoadingProvider()
        {
            thread = new Thread(new ThreadStart(RunSplash));
            loaded = new EventWaitHandle(false, EventResetMode.ManualReset);
            thread.Start();
        }

        public void Open(int Max_Value)
        {
            loaded.WaitOne();

            form.Invoke(new InitProgressCallback(form.Init_Progress), new object[] { Max_Value });
        }

        public void Close()
        {
            loaded.WaitOne();
            Thread.Sleep(1000);
            form.Invoke(new CloseCallback(form.Close));
        }

        public void Join()
        {
            thread.Join();
        }

        public void UpdateProgress(string txt, int value)
        {
            loaded.WaitOne();
            Thread.Sleep(200);
            form.Invoke(new UpdateProgressCallback(form.UpdateProgress), new object[] { txt, value });
        }

        public void UpdateProgress(string txt, int value, int delay)
        {
            loaded.WaitOne();
            Thread.Sleep(delay);
            form.Invoke(new UpdateProgressCallback(form.UpdateProgress), new object[] { txt, value });
        }

        private void RunSplash()
        {
            form = new frm_Loader();
            form.Load += new EventHandler(OnLoad);
            form.ShowDialog();
        }

        void OnLoad(object sender, EventArgs e)
        {
            loaded.Set();
        }

        private delegate void CloseCallback();
        private delegate void UpdateProgressCallback(string txt, int vaue);
        private delegate void InitProgressCallback(int maxValue);

    }
}
