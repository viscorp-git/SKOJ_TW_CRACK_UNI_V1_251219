using System;
using System.Windows.Forms;

namespace IDMAX_FrameWork
{
    public class Tooltip : ToolTip
    {
        public Tooltip()
        {
        }

        public enum Icon
        {
            Error,
            Info,
            None,
            Warning,
        }

        /// <summary>
        /// 툴팁을 표시합니다.
        /// </summary>
        /// <param name="control">툴팁을 표시할 컨트롤</param>
        /// <param name="title">툴팁 제목</param>
        /// <param name="caption">툴팁 내용</param>
        /// <param name="time">툴팁을 표시할 시간(초)</param>
        /// <param name="icon">툴팁 아이콘</param>
        /// <remarks>
        /// 2009/06/02          김지호          [L 00]
        /// </remarks>
        public static void ShowTooltip(Control control, string title, string caption, int time, Icon icon)
        {
            try
            {

                ToolTip toolTip = new ToolTip();
                toolTip.UseAnimation = true;
                toolTip.UseFading = true;

                toolTip.ToolTipTitle = title;
                time = time * 1000;

                switch (icon)
                {
                    case Icon.Error:
                        toolTip.ToolTipIcon = ToolTipIcon.Error;
                        break;

                    case Icon.Info:
                        toolTip.ToolTipIcon = ToolTipIcon.Info;
                        break;

                    case Icon.None:
                        toolTip.ToolTipIcon = ToolTipIcon.None;
                        break;

                    case Icon.Warning:
                        toolTip.ToolTipIcon = ToolTipIcon.Warning;
                        break;
                }

                toolTip.Hide(control);

                toolTip.Show(caption, control, time);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
