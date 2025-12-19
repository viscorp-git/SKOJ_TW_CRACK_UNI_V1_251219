using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SKON_TabWelldingInspection
{
    public partial class frm_Loader : Form
    {
        public frm_Loader()
        {
            InitializeComponent();
        }

        public void Init_Progress(int max_Value)
        {
            lb_LoadingText.Text = "Load.....";
            
        }

        public void UpdateProgress(string txt, int value)
        {
            lb_LoadingText.Text = txt;
           
            pgB_Loading.Value = value;

        }
    }
}
