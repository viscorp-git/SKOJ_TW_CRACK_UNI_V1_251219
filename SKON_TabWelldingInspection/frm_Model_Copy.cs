using IDMAX_FrameWork;
using SKON_TabWelldingInspection.Class;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SKON_TabWelldingInspection
{
    public partial class frm_Model_Copy : BaseConfigForm
    {
        public cls_Model mModel;
        public string mModelModify;

        public frm_Model_Copy(cls_Model mModel, string mModelModify)
        {
            InitializeComponent();
            this.mModel = mModel;
            this.mModelModify = mModelModify;
        }
        private void frm_Model_Copy_Load(object sender, EventArgs e)
        {
            if (mModelModify == "Edit")
                lb_ModifyModel.Text = "Edit Model";
            else
                lb_ModifyModel.Text = "Copy Model";

            lb_ModelNumber_Value.Text = mModel.ModelNumber.ToString();
            lb_ModelName_Value.Text = mModel.ModelName;

            num_ModelNumber.Value = mModel.ModelNumber;
            txt_ModelName.Text = mModel.ModelName;

            // 변경 감지 이벤트 등록
            num_ModelNumber.ValueChanged += MarkDirty;
            txt_ModelName.TextChanged += MarkDirty;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            if (!ConfirmSave())
                return;

            string full_Path = cls_GlobalValue.ModelPath + "\\" + lb_ModelNumber_Value.Text + "_" + lb_ModelName_Value.Text + ".mpp";
            if (mModelModify == "Edit")
            {
                if (num_ModelNumber.Value.ToString() == lb_ModelNumber_Value.Text)
                {
                    ModelEdit(full_Path);
                }
                else
                {
                    if (IndexDuplicateCheck())
                    {
                        ModelEdit(full_Path);
                    }
                    else
                    {
                        MessageBox.Show("The model number is duplicated.");
                        return;
                    }
                }
            }
            else if (mModelModify == "Copy")
            {
                if (IndexDuplicateCheck())
                {
                    ModelCopy(full_Path);
                }
                else
                {
                    MessageBox.Show("The model number is duplicated.");
                    return;
                }
            }

            ResetDirty();   // 저장 완료 처리
            this.Close();
        }

        private void ModelCopy(string oldPath)
        {
            if (MessageBox.Show(lb_ModelName.Text + " : Copy this file?", "COPY", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                string newpath = cls_GlobalValue.ModelPath + "\\" + num_ModelNumber.Value.ToString() + "_" + txt_ModelName.Text.Trim() + ".mpp";

                if (cls_File.FileCopy(oldPath, newpath))
                {
                    MessageBox.Show("Copy OK.");
                }
                else
                {
                    MessageBox.Show("Copy Fail.");
                }
            }
        }

        private void ModelEdit(string oldPath)
        {
            if (MessageBox.Show(txt_ModelName.Text + " : Change this filename?", "RENAME", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                string newpath = cls_GlobalValue.ModelPath + "\\" + num_ModelNumber.Value.ToString() + "_" + txt_ModelName.Text.Trim() + ".mpp";

                if (cls_File.FileRename(oldPath, newpath))
                {
                    mModel.ModelName = txt_ModelName.Text.Trim();
                    mModel.ModelNumber = Convert.ToInt16(num_ModelNumber.Value);

                    if (mModel.Model_Save(newpath))
                    {
                        MessageBox.Show("Change OK.");
                    }
                }
                else
                {
                    MessageBox.Show("Change Fail.");
                }
            }
        }

        private bool IndexDuplicateCheck()
        {
            string strNumber = "";
            DirectoryInfo di = new DirectoryInfo(cls_GlobalValue.ModelPath);
            if (!di.Exists)
            {
                di.Create();
            }
            FileInfo[] fi = di.GetFiles("*.mpp");
            if (fi.Length > 0)
            {
                for (int i = 0; i < fi.Length; i++)
                {
                    strNumber = fi[i].Name.Split('_')[0];
                    if (strNumber == num_ModelNumber.Value.ToString())
                        return false;
                }
            }

            return true;
        }


    }
}