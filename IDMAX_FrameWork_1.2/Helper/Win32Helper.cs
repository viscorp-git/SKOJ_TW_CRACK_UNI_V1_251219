using System;
using System.Runtime.InteropServices;
using System.Text;

namespace IDMAX_FrameWork
{
    public class Win32
    {
        protected static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        protected static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        protected const UInt32 SWP_NOSIZE = 0x0001;
        protected const UInt32 SWP_NOMOVE = 0x0002;
        protected const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;
        protected const int WM_NCLBUTTONDOWN = 0xA1;
        protected const int WM_NCLBUTTONUP = 0xA2;
        protected const int HT_CAPTION = 0x2;
        protected const int SW_HIDE = 0;
        protected const int SW_SHOW = 1;
        protected const int SM_CXSCREEN = 0;
        protected const int SM_CYSCREEN = 1;
        protected const int WM_FONTCHANGE = 0x001D;
        protected const int HWND_BROADCAST = 0xffff;

        protected const int HWND_BOTTOM = 0x0001;
        protected const int HWND_TOP = 0x0000;

        protected const int SWP_NOZORDER = 0x0004;
        protected const int SWP_NOREDRAW = 0x0008;
        protected const int SWP_NOACTIVATE = 0x0010;
        protected const int SWP_FRAMECHANGED = 0x0020;
        protected const int SWP_SHOWWINDOW = 0x0040;
        protected const int SWP_HIDEWINDOW = 0x0080;
        protected const int SWP_NOCOPYBITS = 0x0100;
        protected const int SWP_NOOWNERZORDER = 0x0200;
        protected const int SWP_NOSENDCHANGING = 0x0400;

        protected const int GENERIC_WRITE = 0x40000000;
        protected const int FILE_ATTRIBUTE_NORMAL = 0x00000080;
        protected const int OPEN_EXISTING = 3;

        protected const uint GENERIC_READ = 0x80000000;

        protected static IntPtr TaskBarhwnd
        {
            get { return FindWindow("Shell_TrayWnd", ""); }
        }

        protected static IntPtr StartButtonhwnd
        {
            get { return FindWindowEx(IntPtr.Zero, IntPtr.Zero, (IntPtr)0xC017, null); }
        }

        protected struct LASTINPUTINFO
        {
            public uint cbSize;

            public uint dwTime;
        }

        [StructLayout(LayoutKind.Sequential)]
        protected class SystemTime
        {
            public ushort year;
            public ushort month;
            public ushort dayofweek;
            public ushort day;
            public ushort hour;
            public ushort minute;
            public ushort second;
            public ushort milliseconds;
        }

        [StructLayout(LayoutKind.Sequential)]
        protected struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        [DllImport("kernel32", SetLastError = true)]
        protected static extern System.IntPtr CreateFile(string fileName, uint desiredAccess, uint shareMode, uint securityAttributes, uint creationDisposition,
            uint flagsAndAttributes, int hTemplateFile);
        [DllImport("kernel32", SetLastError = true)]
        protected static extern bool CloseHandle(IntPtr hObject);
        [DllImport("kernel32", SetLastError = true)]
        protected static extern bool WriteFile(IntPtr hFile, byte[] lpBuffer, int nNumberOfBytesToWrite, ref int lpNumberOfBytesWritten, int lpOverlapped);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        protected static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("kernel32")]
        protected static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);
        [DllImport("kernel32")]
        protected static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);
        [DllImport("User32.dll")]
        protected static extern bool LockWorkStation();
        [DllImport("User32.dll")]
        protected static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);
        [DllImport("Kernel32.dll")]
        protected static extern uint GetLastError();
        [DllImportAttribute("user32.dll")]
        protected static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        protected static extern bool SetCapture(IntPtr hWnd);
        [DllImportAttribute("user32.dll")]
        protected static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        protected static extern IntPtr FindWindow(string className, string windowText);
        [DllImport("user32.dll")]
        protected static extern int ShowWindow(IntPtr hwnd, int command);
        [DllImport("user32.dll")]
        protected static extern IntPtr FindWindowEx(IntPtr parentHwnd, IntPtr childAfterHwnd, IntPtr className, string windowText);
        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        protected static extern int GetSystemMetrics(int which);
        //[DllImport("user32.dll")]
        //protected static extern void SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int X, int Y, int width, int height, uint flags);
        [DllImport("Kernel32.dll")]
        protected static extern void SetSystemTime([In] SystemTime p_SetTimer);
        [DllImport("kernel32.dll", SetLastError = true)]
        protected static extern int WriteProfileString(string lpszSection, string lpszKeyName, string lpszString);
        [DllImport("user32.dll")]
        protected static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);
        [DllImport("gdi32", EntryPoint = "AddFontResource")]
        protected static extern int AddFontResource(string lpFileName);
        [DllImport("gdi32", EntryPoint = "RemoveFontResource")]
        protected static extern int RemoveFontResource(string lpFileName);
        [DllImport("Kernel32.dll")]
        protected static extern Boolean AllocConsole();
        [DllImport("wininet.dll")]
        protected extern static bool InternetGetConnectedState(out int Description, int ReservedValue);
    }

    public class Win32Helper : Win32
    {
        public enum ABE : uint
        {
            Left = 0,
            Top = 1,
            Right = 2,
            Bottom = 3
        }

        public static bool OSLock()
        {
            return LockWorkStation();
        }

        public static string GetIniValue(String strSection, String strKey, String strDefault, String strINIPath)
        {
            StringBuilder dstrResult = new StringBuilder(255);
            int i = 0;

            try
            {
                i = GetPrivateProfileString(strSection, strKey, strDefault, dstrResult, 255, strINIPath);   //"색션", "키", "Default", result, size, iniPath
            }
            catch
            {
            }

            return dstrResult.ToString();
        }

        public static void SetIniValue(String strSection, String strKey, String strValue, String strINIPath)
        {
            try
            {
                WritePrivateProfileString(strSection, strKey, strValue, strINIPath);                            //"색션", "키", "설정할값", iniPath
            }
            catch
            {
            }
        }

        public static uint GetIdleTime()
        {
            LASTINPUTINFO lastInPut = new LASTINPUTINFO();
            lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
            GetLastInputInfo(ref lastInPut);

            return GetTickCount() - lastInPut.dwTime;
        }

        public static uint GetTickCount()
        {
            return (uint)Environment.TickCount;
        }

        public static long GetLastInputTime()
        {
            LASTINPUTINFO lastInPut = new LASTINPUTINFO();
            lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
            if (!GetLastInputInfo(ref lastInPut))
            {
                throw new Exception(GetLastError().ToString());
            }

            return lastInPut.dwTime;
        }

        public static bool GetConnectedToInternet()
        {
            int desc;

            return InternetGetConnectedState(out desc, 0);
        }

        /// <summary>
        /// 특정 핸들값에 대한 컨트롤을 이용하여 폼을 움직입니다. 
        /// </summary>
        /// <param name="handle">폼 움직임에 이용할 핸들</param>
        /// <remarks>
        /// 2009/02/26          김지호          [L 00]
        /// </remarks>
        public static void ControlMove(IntPtr handle)
        {
            try
            {
                ReleaseCapture();
                SendMessage(handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
            catch
            {
                throw;
            }
        }

        public static void ShowTaskbar()
        {
            ShowWindow(TaskBarhwnd, SW_SHOW);
            ShowWindow(StartButtonhwnd, SW_SHOW);
        }

        public static void HideTaskbar()
        {
            ShowWindow(TaskBarhwnd, SW_HIDE);
            ShowWindow(StartButtonhwnd, SW_HIDE);
        }

        public static int ScreenX
        {
            get { return GetSystemMetrics(SM_CXSCREEN); }
        }

        public static int ScreenY
        {
            get { return GetSystemMetrics(SM_CYSCREEN); }
        }

        public static bool SetWinFullScreen(IntPtr hwnd)
        {
            return SetWindowPos(hwnd, (IntPtr)HWND_TOP, 0, 0, ScreenX, ScreenY, SWP_SHOWWINDOW);
        }

        public static bool SetWindowPosition(IntPtr hwnd, IntPtr hWndInsertAfter, int x, int y, int width, int heigth)
        {
            return SetWindowPos(hwnd, hWndInsertAfter, x, y, width, heigth, SWP_SHOWWINDOW | SWP_NOZORDER | SWP_NOSIZE);
        }

        public static bool SetAlwaysTop(IntPtr hwnd, bool enable)
        {
            return SetWindowPos(hwnd, (enable) ? HWND_TOPMOST : HWND_NOTOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
        }

        public static void SetDateTime(DateTime datetime, int gmt)
        {
            SystemTime st = new SystemTime();
            st.year = Convert.ToUInt16(datetime.ToString("yyyy"));
            st.month = Convert.ToUInt16(datetime.ToString("MM"));
            st.day = Convert.ToUInt16(datetime.ToString("dd"));
            st.hour = Convert.ToUInt16(datetime.ToString("HH"));
            st.minute = Convert.ToUInt16(datetime.ToString("mm"));
            st.second = Convert.ToUInt16(datetime.ToString("ss"));

            datetime = datetime.AddHours(-gmt);

            st.year = Convert.ToUInt16(datetime.Year);
            st.month = Convert.ToUInt16(datetime.Month);
            st.day = Convert.ToUInt16(datetime.Day);
            st.hour = Convert.ToUInt16(datetime.Hour);
            st.minute = Convert.ToUInt16(datetime.Minute);
            st.second = Convert.ToUInt16(datetime.Second);

            SetSystemTime(st);
        }

        public static bool AddFont(string fontPath)
        {
            int result = AddFontResource(fontPath);

            return (result != 0);
        }

        public static bool RemoveFont(string fontPath)
        {
            int result = RemoveFontResource(fontPath);

            return (result != 0);
        }

        public static bool ShowConsole()
        {
            return Win32.AllocConsole();
        }

        public static void WindowsTaskBarVisible(bool visible)
        {
            uint visibleHandle = (visible) ? (uint)SWP_SHOWWINDOW : (uint)SWP_HIDEWINDOW;
            IntPtr TaskBarHwnd;
            TaskBarHwnd = FindWindow("Shell_traywnd", "");
            SetWindowPos(TaskBarHwnd, IntPtr.Zero, 0, 0, 0, 0, visibleHandle);
        }
    }
}
