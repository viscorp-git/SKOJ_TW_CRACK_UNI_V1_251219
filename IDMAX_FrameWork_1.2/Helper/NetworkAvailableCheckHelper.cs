using System;

namespace IDMAX_FrameWork
{
    public class NetworkAvailableCheckHelper
    {
        private static System.Timers.Timer _checkTimer = new System.Timers.Timer();

        public delegate void AvailableCheckDelegate(bool isAvailable);
        
        public static event AvailableCheckDelegate NetworkAvailable_Changed;
        public static event AvailableCheckDelegate InternetAvailable_Changed;
        
        private static bool _isNetworkAvailable = false;
        public static bool IsNetworkAvailable
        {
            get { return _isNetworkAvailable; }
            private set
            {
                if (_isNetworkAvailable == value) return;

                _isNetworkAvailable = value;

                if (NetworkAvailable_Changed != null) NetworkAvailable_Changed(value);
            }
        }

        private static bool _isInternetAvailable = false;
        public static bool IsInternetAvailable
        {
            get { return _isInternetAvailable; }
            private set
            {
                if (_isInternetAvailable == value) return;

                _isInternetAvailable = value;

                if (InternetAvailable_Changed != null) InternetAvailable_Changed(value);
            }
        }

        public static void Start(int interval)
        {
            if (_checkTimer == null)
            {
                _checkTimer = new System.Timers.Timer();
                _checkTimer.Elapsed += new System.Timers.ElapsedEventHandler(_checkTimer_Elapsed);
            }

            _checkTimer.AutoReset = false;
            _checkTimer.Interval = interval;
            _checkTimer.Start();
        }

        public static void Stop()
        {
            if (_checkTimer == null) return;

            _checkTimer.Stop();
        }

        private static void _checkTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                IsNetworkAvailable = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();

                IsInternetAvailable = Win32Helper.GetConnectedToInternet();
            }
            catch (Exception)
            {
                IsNetworkAvailable = false;
                IsInternetAvailable = false;
            }
            finally
            {
                _checkTimer.Start();
            }
        }
    }
}
