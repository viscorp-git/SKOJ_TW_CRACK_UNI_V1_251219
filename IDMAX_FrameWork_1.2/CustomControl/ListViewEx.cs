using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace IDMAX_FrameWork
{
    public class ListViewEx : ListView
    {
        [DllImport("uxtheme", CharSet = CharSet.Auto)]
        static extern Boolean SetWindowTheme(IntPtr hWindow, String subAppName, String subIDList);

        public ListViewEx()
        {
            SetStyle((ControlStyles)0x22010, true);
            this.SetStyle(ControlStyles.DoubleBuffer , true);
            this.UpdateStyles();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // 핸들이 생성된 후에 테마를 적용한다.
          //  SetWindowTheme(Handle, "explorer", null);
        }
    }
}
