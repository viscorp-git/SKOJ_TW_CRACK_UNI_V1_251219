using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace BINPICKING_SYSTEM
{
    public partial class StopWatchEx : UserControl
    {
        private bool bStart = false;
        private Stopwatch sw = new Stopwatch();
        private Thread swThread = null;

        private delegate void SetTextCallback(string text);
        //delegate string CalStopWatch();

        public StopWatchEx()
        {
            InitializeComponent();
        }

        public void Start()
        {
            if (!bStart)
            {
                //string curTimer = "";
                sw.Start();
                //Button_Reset.Enabled = false;
                bStart = true;
                //curTimer = System.DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                //ProgressTimer.Text = curTimer;
                this.swThread = new Thread(new ThreadStart(this.ThreadProcStopWatch));
                this.swThread.Start();
            }
        }

        public void Stop()
        {
            if (bStart)
            {
                sw.Stop();
                bStart = false;
                //Button_Reset.Enabled = true;
            }
        }

        private void ThreadProcStopWatch()
        {
            while (this.bStart)
                this.SetText(this.CalStopWatch());
        }

        private void SetText(string text)
        {
            try
            {
                // InvokeRequired required compares the thread ID of the
                // calling thread to the thread ID of the creating thread.
                // If these threads are different, it returns true.
                if (this.ProgressTimer.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(SetText);
                    this.Invoke(d, new object[] { text });
                }
                else
                {
                    this.ProgressTimer.Text = text;
                    
                }
            }
            catch
            {
                swThread_Abort();
            }
        }

        private string CalStopWatch()
        {
            TimeSpan ts = sw.Elapsed;

            string strTotalTimer = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            return strTotalTimer;//sw.ElapsedMilliseconds.ToString() + "ms";               
        }

        public string Get_Lap()
        {
            return ProgressTimer.Text;
        }

        public void Reset()
        {
            sw.Reset();
            //ProgressTimer.Text = "00:00:00.00";
            swThread_Abort();
            SetText("00:00:00.00");
            this.Refresh();
        }

        private void swThread_Abort()
        {
            if (swThread != null)
            {
                swThread.Abort();
                if (bStart)
                {
                    sw.Stop();
                    
                }
            }
            bStart = false;
        }
    }
}
