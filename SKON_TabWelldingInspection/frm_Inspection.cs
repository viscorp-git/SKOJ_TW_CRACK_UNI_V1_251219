using Cognex.VisionPro;
using IDMAX_FrameWork;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SKON_TabWelldingInspection
{
    public partial class frm_Inspection : BaseConfigForm
    {

        private cls_Model mModel = null;


        public frm_Inspection()
        {
            InitializeComponent();
        }

        private void frm_Inspection_Load(object sender, EventArgs e)
        {
            mModel = cls_GlobalValue.Model.DeepCopy();
            cogToolBlockEditV21_Cathode.Subject = mModel.CathodeToolBlock;
            cogToolBlockEditV21_Anode.Subject = mModel.AnodeToolBlock;

            // Toolblock 변경 감지 이벤트 등록
            cogToolBlockEditV21_Cathode.SubjectChanged += MarkDirty;
            cogToolBlockEditV21_Anode.SubjectChanged += MarkDirty;
        }

        private void frm_Inspection_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            if (!ConfirmSave())
                return;

            string full_Path = cls_GlobalValue.ModelPath + "\\" + mModel.ModelNumber.ToString() + "_" + mModel.ModelName + ".mpp";

            mModel.CathodeToolBlock = cogToolBlockEditV21_Cathode.Subject;
            mModel.AnodeToolBlock = cogToolBlockEditV21_Anode.Subject;

            if (mModel.Model_Save(full_Path))
            {
                cls_GlobalValue.Model = mModel.DeepCopy();
                cls_GlobalValue.ChangeToolblock_Anode = true;
                cls_GlobalValue.ChangeToolblock_Cathode = true;

                ResetDirty();   // 저장 완료 처리
                MessageBox.Show("Model Save Success");
            }
            else
            {
                MessageBox.Show("Model Save Fail");
            }
        }

        private void btn_ImageLoad_Anode_Click(object sender, EventArgs e)
        {
            ofd_LoadImage_Anode.DefaultExt = "jpg";
            ofd_LoadImage_Anode.Filter = "이미지 파일 (*.jpg, *.bmp, *.png, *.gif, *.tif)|*.jpg; *.bmp; *.png; *.gif; *.tif;|모든 파일 (*.*)|*.*";
            ofd_LoadImage_Anode.ShowDialog();

            if (ofd_LoadImage_Anode.FileName.Length > 0)
            {
                GC.Collect(GC.MaxGeneration - 1);
                Bitmap bmp = new Bitmap(ofd_LoadImage_Anode.FileName);
                ICogImage cogImg = new CogImage24PlanarColor((Bitmap)bmp.Clone());
                bmp.Dispose();

                if (cogToolBlockEditV21_Anode.Subject.Inputs.Contains("AnodeImage"))
                {
                    cogToolBlockEditV21_Anode.Subject.Inputs["AnodeImage"].Value = cogImg.CopyBase(CogImageCopyModeConstants.SharePixels);
                    cogToolBlockEditV21_Anode.Subject.Run();
                }
                else
                {
                    MessageBox.Show("Image Insert to Toolblock Error");
                }
                GC.Collect();
            }
        }

        private void btn_ImageLoad_Cathode_Click(object sender, EventArgs e)
        {
            ofd_LoadImage_Cathode.DefaultExt = "jpg";
            ofd_LoadImage_Cathode.Filter = "이미지 파일 (*.jpg, *.bmp, *.png, *.gif, *.tif)|*.jpg; *.bmp; *.png; *.gif; *.tif;|모든 파일 (*.*)|*.*";
            ofd_LoadImage_Cathode.ShowDialog();

            if (ofd_LoadImage_Cathode.FileName.Length > 0)
            {
                GC.Collect(GC.MaxGeneration - 1);
                Bitmap bmp = new Bitmap(ofd_LoadImage_Cathode.FileName);
                ICogImage cogImg = new CogImage24PlanarColor((Bitmap)bmp.Clone());
                bmp.Dispose();

                if (cogToolBlockEditV21_Cathode.Subject.Inputs.Contains("CathodeImage"))
                {
                    cogToolBlockEditV21_Cathode.Subject.Inputs["CathodeImage"].Value = cogImg.CopyBase(CogImageCopyModeConstants.SharePixels);
                    cogToolBlockEditV21_Cathode.Subject.Run();
                }
                else
                {
                    MessageBox.Show("Image Insert to Toolblock Error");
                }
                GC.Collect();
            }
        }
    }
}