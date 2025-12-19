using System;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace IDMAX_FrameWork
{
    public partial class PingChecker : UserControl
    {
        private System.Timers.Timer _pingCheckTimer = null;

        private Ping _pingSender = null;

        private Object _controlLock = new Object();

        public string PingCheckAddress
        {
            get;
            set;
        }

        public PingChecker()
        {
            InitializeComponent();
        }

        private void PingChecker_Load(object sender, EventArgs e)
        {
            this.initializePingCheck();
        }

        private void initializePingCheck()
        {
            try
            {
                this._pingSender = new Ping();
                this._pingSender.PingCompleted += new PingCompletedEventHandler(_pingSender_PingCompleted);

                this._pingCheckTimer = new System.Timers.Timer();
                this._pingCheckTimer.AutoReset = false;
                this._pingCheckTimer.Interval = 100;
                this._pingCheckTimer.Elapsed += new System.Timers.ElapsedEventHandler(_pingCheckTimer_Elapsed);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Start()
        {
            try
            {
                if (this._pingCheckTimer != null) this._pingCheckTimer.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Stop()
        {
            try
            {
                if (this._pingCheckTimer != null) this._pingCheckTimer.Stop();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void displayReply(PingReply reply)
        {
            try
            {
                this.setControlText(this.Label1, reply.RoundtripTime.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void setControlText(Control control, string text)
        {
            try
            {
                lock (this._controlLock)
                {
                    control.Invoke((MethodInvoker)delegate
                    {
                        control.Text = text;
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void _pingSender_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled) ((AutoResetEvent)e.UserState).Set();
                if (e.Error != null) ((AutoResetEvent)e.UserState).Set();

                PingReply reply = e.Reply;

                this.displayReply(e.Reply);

                ((AutoResetEvent)e.UserState).Set();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void _pingCheckTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                AutoResetEvent waiter = new AutoResetEvent(false);

                int timeout = 10000;

                string data = "PingCheck";
                byte[] buffer = Encoding.ASCII.GetBytes(data);

                PingOptions options = new PingOptions(64, true);

                this._pingSender.SendAsync(this.PingCheckAddress, timeout, buffer, options, waiter);

                waiter.WaitOne();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this._pingCheckTimer.Start();
            }
        }
    }
}
