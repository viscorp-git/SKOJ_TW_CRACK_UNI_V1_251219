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
        private bool _orgSharpDetYN;
        private int _orgSharpDetCnt;
        private bool _orgLocationDetYN;
        private int _orgLocationDetCnt;
        private bool _orgNGDetYN;
        private int _orgNGDetCnt;
        private bool _orgNGITVDetYN;
        private int _orgNGITVDetCnt;
        private int _orgNGITVMaxCnt;
        private bool _orgLogDetYN;
        private int _orgBrightRankQty;

        private double _orgBrightCaStd;
        private double _orgBrightCaLow;
        private double _orgBrightCaUpp;
        private double _orgSharpCaStd;
        private double _orgSharpCaLow;
        private double _orgBrightAnStd;
        private double _orgBrightAnLow;
        private double _orgBrightAnUpp;
        private double _orgSharpAnStd;
        private double _orgSharpAnLow;

        public detectConfig(frm_Main main)
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.ControlBox = true;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            
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
            _orgSharpDetYN = frmMain.mSharp_Det_YN;
            _orgSharpDetCnt = frmMain.mSharp_Det_CNT;
            _orgLocationDetYN = frmMain.mLocation_Det_YN;
            _orgLocationDetCnt = frmMain.mLocation_Det_CNT;
            _orgNGDetYN = frmMain.mNG_Det_YN;
            _orgNGDetCnt = frmMain.mNG_Det_CNT;
            _orgNGITVDetYN = frmMain.mNG_ITV_Det_YN;
            _orgNGITVDetCnt = frmMain.mNG_ITV_Det_CNT;
            _orgNGITVMaxCnt = frmMain.mNG_ITV_Det_MAX_CNT;
            _orgLogDetYN = frmMain.mLog_Det_YN;
            _orgBrightRankQty = frmMain.mBright_Rank_Qty;

            _orgBrightCaStd = frmMain.mBright_CA_STD_VAL;
            _orgBrightCaLow = frmMain.mBright_CA_LOW_VAL;
            _orgBrightCaUpp = frmMain.mBright_CA_UPP_VAL;
            _orgSharpCaStd = frmMain.mSharp_CA_STD_VAL;
            _orgSharpCaLow = frmMain.mSharp_CA_LOW_VAL;
            _orgBrightAnStd = frmMain.mBright_AN_STD_VAL;
            _orgBrightAnLow = frmMain.mBright_AN_LOW_VAL;
            _orgBrightAnUpp = frmMain.mBright_AN_UPP_VAL;
            _orgSharpAnStd = frmMain.mSharp_AN_STD_VAL;
            _orgSharpAnLow = frmMain.mSharp_AN_LOW_VAL;

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

        private void LogIfChanged<T>(string paramName, T originalValue, T newValue)
        {
            if (!EqualityComparer<T>.Default.Equals(originalValue, newValue))
            {
                frmMain.Log.WriteLog("TEST", $"[Detect Anomal] {paramName} changed : {originalValue} -> {newValue}");
            }
        }

        private void btnConfigCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                "Are you sure you want to discard the changes?",
                "Cancel",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            LoadConfigToUI(); // 원복
            ResetDirty();     // 변경 없음
            this.Close();
        }

        private void btnConfigSave_Click(object sender, EventArgs e)
        {
            // 공통 Save Confirm 사용
            if (!ConfirmSave())
                return;

            try
            {
                // 로그 비교용 UI 값 임시 저장
                bool newBrightDetYN = rdoBTrue.Checked;
                int newBrightDetCnt = Convert.ToInt32(txtTHRD_B_Cnt.Text);
                bool newSharpDetYN = rdoSTrue.Checked;
                int newSharpDetCnt = Convert.ToInt32(txtTHRD_S_Cnt.Text);
                bool newLocationDetYN = rdoLTrue.Checked;
                int newLocationDetCnt = Convert.ToInt32(txtTHRD_L_Cnt.Text);
                bool newNGDetYN = rdoNGTrue.Checked;
                int newNGDetCnt = Convert.ToInt32(txtTHRD_NG_Cnt.Text);
                bool newNGITVDetYN = rdoNGITVTrue.Checked;
                int newNGITVDetCnt = Convert.ToInt32(txtTHRD_NGITV_Cnt.Text);
                int newNGITVMaxCnt = Convert.ToInt32(txtTHRD_NGITV_MAX_CNT.Text);
                bool newLogDetYN = rdoALTrue.Checked;
                int newBrightRankQty = Convert.ToInt32(txtRankQty.Text);

                // 251224 변경 로그 기록
                LogIfChanged("Brightness Bypass Info", _orgBrightDetYN, newBrightDetYN);
                LogIfChanged("Sharpness Bypass Info", _orgSharpDetYN, newSharpDetYN);
                LogIfChanged("Location Bypass Info", _orgLocationDetYN, newLocationDetYN);
                LogIfChanged("NG Bypass Info", _orgNGDetYN, newNGDetYN);
                LogIfChanged("NG Interval Bypass Info", _orgNGITVDetYN, newNGITVDetYN);

                LogIfChanged("Brightness THRD Count Info", _orgBrightDetCnt, newBrightDetCnt);
                LogIfChanged("Sharpness THRD Count Info", _orgSharpDetCnt, newSharpDetCnt);
                LogIfChanged("Location THRD Count Info", _orgLocationDetCnt,newLocationDetCnt);
                LogIfChanged("NG THRD Count Info", _orgNGDetCnt, newNGDetCnt);
                LogIfChanged("NG Interval THRD Count Info", _orgNGDetCnt, newNGITVDetCnt);
                LogIfChanged("NG Interval THRD Coutn(Max) Info", _orgNGITVMaxCnt, newNGITVMaxCnt);
                
                LogIfChanged("Analze Log Use Info", _orgLogDetYN, newLogDetYN);
                LogIfChanged("Brightness Rank Qty Info", _orgBrightRankQty, newBrightRankQty);

                double newBrightCaStd = Convert.ToDouble(txtB_CA_STD_VAL.Text);
                double newBrightCaLow = Convert.ToDouble(txtB_CA_LOW_VAL.Text);
                double newBrightCaUpp = Convert.ToDouble(txtB_CA_UPP_VAL.Text);
                double newSharpCaStd = Convert.ToDouble(txtS_CA_STD_VAL.Text);
                double newSharpCaLow = Convert.ToDouble(txtS_CA_LOW_VAL.Text);
                double newBrightAnStd = Convert.ToDouble(txtB_AN_STD_VAL.Text);
                double newBrightAnLow = Convert.ToDouble(txtB_AN_LOW_VAL.Text);
                double newBrightAnUpp = Convert.ToDouble(txtB_AN_UPP_VAL.Text);
                double newSharpAnStd = Convert.ToDouble(txtS_AN_STD_VAL.Text);
                double newSharpAnLow = Convert.ToDouble(txtS_AN_LOW_VAL.Text);
                
                LogIfChanged("Brightness CA STD Value", (int)_orgBrightCaStd, (int)newBrightCaStd);
                LogIfChanged("Brightness CA LOW Value", (int)_orgBrightCaLow, (int)newBrightCaLow);
                LogIfChanged("Brightness CA UPP Value", (int)_orgBrightCaUpp, (int)newBrightCaUpp);
                LogIfChanged("Sharpness CA STD Value", (int)_orgSharpCaStd, (int)newSharpCaStd);
                LogIfChanged("Sharpness CA LOW Value", (int)_orgSharpCaLow, (int)newSharpCaLow);
                LogIfChanged("Brightness AN STD Value", (int)_orgBrightAnStd, (int)newBrightAnStd);
                LogIfChanged("Brightness AN LOW Value", (int)_orgBrightAnLow, (int)newBrightAnLow);
                LogIfChanged("Brightness AN UPP Value", (int)_orgBrightAnUpp, (int)newBrightAnUpp);
                LogIfChanged("Sharpness AN STD Value", (int)_orgSharpAnStd, (int)newSharpAnStd);
                LogIfChanged("Sharpness AN LOW Value", (int)_orgSharpAnLow, (int)newSharpAnLow);

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
                MessageBox.Show("Settings have been saved.", "Save");
                this.Close();
            }
            catch (Exception ex)
            {
                frmMain.Log.WriteLog("ERR", $"detectConfig Save Error : {ex.Message}");
                MessageBox.Show("Failed to save the settings.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
