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
    public partial class detectConfig : BaseConfigForm
    {
        //frm_Main frmMain = new frm_Main();
        private frm_Main frmMain;

        //변경 로그용 원본 값 저장 변수(일단 BRG만)
        private bool _orgBrightDetYN;
        private int _orgBrightDetCnt;
        
        public detectConfig(frm_Main main)
        {
            InitializeComponent();
            frmMain = main;
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

        // 저장된 값(frmMain 변수들) → 화면(UI)로 다시 세팅하는 메서드
        private void LoadConfigToUI()
        {
            // 원본 값 저장
            _orgBrightDetYN = frmMain.mBright_Det_YN;
            _orgBrightDetCnt = frmMain.mBright_Det_CNT;

            // RadioButton (bool)
            rdoBTrue.Checked = frmMain.mBright_Det_YN;
            rdoSTrue.Checked = frmMain.mSharp_Det_YN;
            rdoLTrue.Checked = frmMain.mLocation_Det_YN;
            rdoNGTrue.Checked = frmMain.mNG_Det_YN;
            rdoNGITVTrue.Checked = frmMain.mNG_ITV_Det_YN;
            rdoALTrue.Checked = frmMain.mLog_Det_YN;

            // TextBox (int/double)
            txtRankQty.Text = frmMain.mBright_Rank_Qty.ToString();

            txtTHRD_B_Cnt.Text = frmMain.mBright_Det_CNT.ToString();
            txtTHRD_S_Cnt.Text = frmMain.mSharp_Det_CNT.ToString();
            txtTHRD_L_Cnt.Text = frmMain.mLocation_Det_CNT.ToString();
            txtTHRD_NG_Cnt.Text = frmMain.mNG_Det_CNT.ToString();
            txtTHRD_NGITV_Cnt.Text = frmMain.mNG_ITV_Det_CNT.ToString();
            txtTHRD_NGITV_MAX_CNT.Text = frmMain.mNG_ITV_Det_MAX_CNT.ToString();

            txtB_CA_STD_VAL.Text = frmMain.mBright_CA_STD_VAL.ToString();
            txtB_CA_LOW_VAL.Text = frmMain.mBright_CA_LOW_VAL.ToString();
            txtB_CA_UPP_VAL.Text = frmMain.mBright_CA_UPP_VAL.ToString();
            txtS_CA_STD_VAL.Text = frmMain.mSharp_CA_STD_VAL.ToString();
            txtS_CA_LOW_VAL.Text = frmMain.mSharp_CA_LOW_VAL.ToString();

            txtB_AN_STD_VAL.Text = frmMain.mBright_AN_STD_VAL.ToString();
            txtB_AN_LOW_VAL.Text = frmMain.mBright_AN_LOW_VAL.ToString();
            txtB_AN_UPP_VAL.Text = frmMain.mBright_AN_UPP_VAL.ToString();
            txtS_AN_STD_VAL.Text = frmMain.mSharp_AN_STD_VAL.ToString();
            txtS_AN_LOW_VAL.Text = frmMain.mSharp_AN_LOW_VAL.ToString();
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void btnConfigCancel_Click(object sender, EventArgs e)
        {
            if (_isDirty)
            {
                if (MessageBox.Show(
                    "변경사항을 취소하시겠습니까?",
                    "Cancel",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
            }

            LoadConfigToUI(); // 원복
            ResetDirty();     // 변경 없음
        }

        private void btnConfigSave_Click(object sender, EventArgs e)
        {
            // 공통 Save Confirm 사용
            if (!ConfirmSave())
                return;

            try
            {
                // 로그 비교용 UI 값 임시 저장(일단 brg만)
                bool newBrightDetYN = rdoBTrue.Checked;
                int newBrightDetCnt = Convert.ToInt32(txtTHRD_B_Cnt.Text);

                // 어노말리 변경 로그
                if (_orgBrightDetYN != newBrightDetYN)
                {
                    frmMain.Log.WriteLog("TEST", $"[Detect Anomal] Brightness Bypass Info changed : {_orgBrightDetYN} -> {newBrightDetYN}");
                }
                if (_orgBrightDetCnt != newBrightDetCnt)
                {
                    frmMain.Log.WriteLog("TEST", $"[Detect Anomal] Brightness THRD Info changed : {_orgBrightDetCnt} -> {newBrightDetCnt}");
                }

                // 실제 메모리 값에 저장
                frmMain.mBright_Det_YN = newBrightDetYN;
                frmMain.mSharp_Det_YN = rdoSTrue.Checked ? true : false;
                frmMain.mLocation_Det_YN = rdoLTrue.Checked ? true : false;
                frmMain.mNG_Det_YN = rdoNGTrue.Checked ? true : false;
                frmMain.mNG_ITV_Det_YN = rdoNGITVTrue.Checked ? true : false;
                frmMain.mLog_Det_YN = rdoALTrue.Checked ? true : false;;

                frmMain.mBright_Rank_Qty = Convert.ToInt32(txtRankQty.Text);
                frmMain.mBright_Det_CNT = newBrightDetCnt;
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

                // INI 파일에 저장
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

                ResetDirty();
                MessageBox.Show("저장되었습니다.", "Save");
            }
            catch (Exception ex)
            {
                frmMain.Log.WriteLog("ERR", $"detectConfig Save Error : {ex.Message}");
                MessageBox.Show("저장 중 오류가 발생했습니다", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void detectConfig_Load(object sender, EventArgs e)
        {
            LoadConfigToUI();   // 저장된 값 → 화면(UI)로 세팅
            ResetDirty();      // 변경 없음 상태
        }
    }
}
