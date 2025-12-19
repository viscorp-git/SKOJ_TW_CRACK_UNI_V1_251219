using System;
using System.Windows.Forms;


namespace IDMAX_FrameWork
{
    public partial class KeyPad : UserControl
    {
        public delegate void KeyPressedEventHandler(string PressedKey);
        public event KeyPressedEventHandler KeypadKeyPressed;

        public KeyPad()
        {
            InitializeComponent();
        }

        private void btnKeypad_Click(object sender, EventArgs e)
        {
            try
            {
                Button btnTemp = (Button)sender;
                string strSendText = btnTemp.Text;

                if (KeypadKeyPressed != null)
                {
                    this.KeypadKeyPressed(strSendText);
                }
            }
            catch (Exception)
            {
            }
        }

        private void btnBackspace_Click(object sender, EventArgs e)
        {
            try
            {
                if (KeypadKeyPressed != null)
                {
                    this.KeypadKeyPressed("Backspace");
                }
            }
            catch (Exception)
            {
            }
        }

        //private void btnEnter_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (KeypadKeyPressed != null)
        //        {
        //            this.KeypadKeyPressed("Enter");
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}
    }
}
