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
    public partial class detectConfig : Form
    {
        frm_Main frmMain = new frm_Main();

        public detectConfig(frm_Main _frmMain)
        {
            InitializeComponent();
            frmMain = _frmMain;
            loadConfigData();
        }

        private void loadConfigData()
        {
            try
            {
                rdoBTrue.Checked = frmMain.mBright_Det_YN ? true : false;
                rdoBFalse.Checked = !frmMain.mBright_Det_YN ? true : false;

                rdoSTrue.Checked = frmMain.mSharp_Det_YN ? true : false;
                rdoSFalse.Checked = !frmMain.mSharp_Det_YN ? true : false;

                rdoLTrue.Checked = frmMain.mLocation_Det_YN ? true : false;
                rdoLFalse.Checked = !frmMain.mLocation_Det_YN ? true : false;

                rdoNGTrue.Checked = frmMain.mNG_Det_YN ? true : false;
                rdoNGFalse.Checked = !frmMain.mNG_Det_YN ? true : false;

                rdoNGITVTrue.Checked = frmMain.mNG_ITV_Det_YN ? true : false;
                rdoNGITVFalse.Checked = !frmMain.mNG_ITV_Det_YN ? true : false;

                rdoALTrue.Checked = frmMain.mLog_Det_YN ? true : false;
                rdoALFalse.Checked = !frmMain.mLog_Det_YN ? true : false;

                txtRankQty.Text = Convert.ToString(frmMain.mBright_Rank_Qty);
                txtTHRD_B_Cnt.Text = Convert.ToString(frmMain.mBright_Det_CNT);
                txtTHRD_S_Cnt.Text = Convert.ToString(frmMain.mSharp_Det_CNT);
                txtTHRD_L_Cnt.Text = Convert.ToString(frmMain.mLocation_Det_CNT);
                txtTHRD_NG_Cnt.Text = Convert.ToString(frmMain.mNG_Det_CNT);
                txtTHRD_NGITV_Cnt.Text = Convert.ToString(frmMain.mNG_ITV_Det_CNT);
                txtTHRD_NGITV_MAX_CNT.Text = Convert.ToString(frmMain.mNG_ITV_Det_MAX_CNT);

                txtB_CA_STD_VAL.Text = Convert.ToString(frmMain.mBright_CA_STD_VAL);
                txtB_CA_LOW_VAL.Text = Convert.ToString(frmMain.mBright_CA_LOW_VAL);
                txtB_CA_UPP_VAL.Text = Convert.ToString(frmMain.mBright_CA_UPP_VAL);
                txtS_CA_STD_VAL.Text = Convert.ToString(frmMain.mSharp_CA_STD_VAL);
                txtS_CA_LOW_VAL.Text = Convert.ToString(frmMain.mSharp_CA_LOW_VAL);

                txtB_AN_STD_VAL.Text = Convert.ToString(frmMain.mBright_AN_STD_VAL);
                txtB_AN_LOW_VAL.Text = Convert.ToString(frmMain.mBright_AN_LOW_VAL);
                txtB_AN_UPP_VAL.Text = Convert.ToString(frmMain.mBright_AN_UPP_VAL);
                txtS_AN_STD_VAL.Text = Convert.ToString(frmMain.mSharp_AN_STD_VAL);
                txtS_AN_LOW_VAL.Text = Convert.ToString(frmMain.mSharp_AN_LOW_VAL);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void btnConfigSave_Click(object sender, EventArgs e)
        {
            try
            {
                frmMain.mBright_Det_YN = rdoBTrue.Checked ? true : false;
                frmMain.mSharp_Det_YN = rdoSTrue.Checked ? true : false;
                frmMain.mLocation_Det_YN = rdoLTrue.Checked ? true : false;
                frmMain.mNG_Det_YN = rdoNGTrue.Checked ? true : false;
                frmMain.mNG_ITV_Det_YN = rdoNGITVTrue.Checked ? true : false;
                frmMain.mLog_Det_YN = rdoALTrue.Checked ? true : false;;

                frmMain.mBright_Rank_Qty = Convert.ToInt32(txtRankQty.Text);
                frmMain.mBright_Det_CNT = Convert.ToInt32(txtTHRD_B_Cnt.Text);
                frmMain.mSharp_Det_CNT = Convert.ToInt32(txtTHRD_S_Cnt.Text);
                frmMain.mLocation_Det_CNT = Convert.ToInt32(txtTHRD_L_Cnt.Text);
                frmMain.mNG_Det_CNT = Convert.ToInt32(txtTHRD_NG_Cnt.Text);
                frmMain.mNG_ITV_Det_CNT = Convert.ToInt32(txtTHRD_NGITV_Cnt.Text);
                frmMain.mNG_ITV_Det_MAX_CNT = Convert.ToInt32(txtTHRD_NGITV_MAX_CNT.Text);

                frmMain.mBright_CA_STD_VAL = Convert.ToDouble(txtB_CA_STD_VAL.Text);
                frmMain.mBright_CA_LOW_VAL = Convert.ToDouble(txtB_CA_LOW_VAL.Text);
                frmMain.mBright_CA_UPP_VAL = Convert.ToDouble(txtB_CA_UPP_VAL.Text);
                frmMain.mSharp_CA_STD_VAL = Convert.ToDouble(txtS_CA_STD_VAL.Text);
                frmMain.mSharp_CA_LOW_VAL = Convert.ToDouble(txtS_CA_LOW_VAL.Text);

                frmMain.mBright_AN_STD_VAL = Convert.ToDouble(txtB_AN_STD_VAL.Text);
                frmMain.mBright_AN_LOW_VAL = Convert.ToDouble(txtB_AN_LOW_VAL.Text);
                frmMain.mBright_AN_UPP_VAL = Convert.ToDouble(txtB_AN_UPP_VAL.Text);
                frmMain.mSharp_AN_STD_VAL = Convert.ToDouble(txtS_AN_STD_VAL.Text);
                frmMain.mSharp_AN_LOW_VAL = Convert.ToDouble(txtS_AN_LOW_VAL.Text);

                frmMain.ini.WriteIniValue("Detect", "Bright_Det_YN", Convert.ToString(frmMain.mBright_Det_YN));
                frmMain.ini.WriteIniValue("Detect", "Sharp_Det_YN", Convert.ToString(frmMain.mSharp_Det_YN));
                frmMain.ini.WriteIniValue("Detect", "Location_Det_YN", Convert.ToString(frmMain.mLocation_Det_YN));
                frmMain.ini.WriteIniValue("Detect", "NG_Det_YN", Convert.ToString(frmMain.mNG_Det_YN));
                frmMain.ini.WriteIniValue("Detect", "NG_ITV_Det_YN", Convert.ToString(frmMain.mNG_ITV_Det_YN));
                frmMain.ini.WriteIniValue("Detect", "Log_Det_YN", Convert.ToString(frmMain.mLog_Det_YN));

                frmMain.ini.WriteIniValue("Detect", "Bright_Det_CNT", Convert.ToString(frmMain.mBright_Det_CNT));
                frmMain.ini.WriteIniValue("Detect", "Sharp_Det_CNT", Convert.ToString(frmMain.mSharp_Det_CNT));
                frmMain.ini.WriteIniValue("Detect", "Location_Det_CNT", Convert.ToString(frmMain.mLocation_Det_CNT));
                frmMain.ini.WriteIniValue("Detect", "NG_Det_CNT", Convert.ToString(frmMain.mNG_Det_CNT));
                frmMain.ini.WriteIniValue("Detect", "NG_ITV_Det_CNT", Convert.ToString(frmMain.mNG_ITV_Det_CNT));
                frmMain.ini.WriteIniValue("Detect", "NG_ITV_Det_MAX_CNT", Convert.ToString(frmMain.mNG_ITV_Det_MAX_CNT));

                frmMain.ini.WriteIniValue("Detect", "Bright_Rank_Qty", Convert.ToString(frmMain.mBright_Rank_Qty));

                frmMain.ini.WriteIniValue("Detect", "Bright_CA_STD_VAL", Convert.ToString(frmMain.mBright_CA_STD_VAL));
                frmMain.ini.WriteIniValue("Detect", "Bright_CA_LOW_VAL", Convert.ToString(frmMain.mBright_CA_LOW_VAL));
                frmMain.ini.WriteIniValue("Detect", "Bright_CA_UPP_VAL", Convert.ToString(frmMain.mBright_CA_UPP_VAL));
                frmMain.ini.WriteIniValue("Detect", "Sharp_CA_STD_VAL", Convert.ToString(frmMain.mSharp_CA_STD_VAL));
                frmMain.ini.WriteIniValue("Detect", "Sharp_CA_LOW_VAL", Convert.ToString(frmMain.mSharp_CA_LOW_VAL));

                frmMain.ini.WriteIniValue("Detect", "Bright_AN_STD_VAL", Convert.ToString(frmMain.mBright_AN_STD_VAL));
                frmMain.ini.WriteIniValue("Detect", "Bright_AN_LOW_VAL", Convert.ToString(frmMain.mBright_AN_LOW_VAL));
                frmMain.ini.WriteIniValue("Detect", "Bright_AN_UPP_VAL", Convert.ToString(frmMain.mBright_AN_UPP_VAL));
                frmMain.ini.WriteIniValue("Detect", "Sharp_AN_STD_VAL", Convert.ToString(frmMain.mSharp_AN_STD_VAL));
                frmMain.ini.WriteIniValue("Detect", "Sharp_AN_LOW_VAL", Convert.ToString(frmMain.mSharp_AN_LOW_VAL));

                MessageBox.Show("저장되었습니다.", "Save");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}
