using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

namespace SKON_TabWelldingInspection
{
    public partial class frm_Model : IDMAX_FrameWork.MaterialForm
    {
        public class ListViewSorter : IComparer
        {
            public int Compare(object x, object y)
            {
                ListViewItem listViewItemX = (ListViewItem)x;
                ListViewItem listViewItemY = (ListViewItem)y;

                // 예를 들어, 리스트 뷰에서 첫 번째 열의 텍스트를 기준으로 정렬합니다.
                return string.Compare(listViewItemX.SubItems[0].Text, listViewItemY.SubItems[0].Text);
            }
        }

        private cls_Model mModel;

        public frm_Model()
        {
            InitializeComponent();
            lv_ModelList.ListViewItemSorter = new ListViewSorter();
        }

        private void frm_Model_Load(object sender, EventArgs e)
        {
            RecipeReflash();
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            string modelName = txt_ModelName.Text.Trim();
            int modelNumber = (int)num_ModelNumber.Value;
            string full_Path = cls_GlobalValue.ModelPath + "\\" + modelNumber.ToString() + "_" + modelName + ".mpp";

            for (int i = 0; i < lv_ModelList.Items.Count; i++)
            {
                int tempNo = int.Parse(lv_ModelList.Items[i].SubItems[0].Text);

                if (modelNumber == tempNo)
                {
                    MessageBox.Show("The same model Number exists");
                    return;
                }
            }

            if (modelName == "")
            {
                MessageBox.Show("Please enter a model name");
                return;
            }


            cls_Model model = new cls_Model();
            model.ModelName = modelName;
            model.ModelNumber = modelNumber;
            if (model.Model_Save(full_Path))
            {
                ListViewItem item = new ListViewItem(model.ModelNumber.ToString());
                item.SubItems.Add(model.ModelName);
                lv_ModelList.Items.Add(item);
            }
            else
            {
                MessageBox.Show("Model Add Fail");
            }

            txt_ModelName.Text = "";
            num_ModelNumber.Value = 0;
            lv_ModelList.Sort();

        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            if (lp_ModelNumberLoad.Enable)
            {
                string modelName = lp_ModelNameLoad.LampOnDescription.Trim();
                int modelNumber = Convert.ToInt16(lp_ModelNumberLoad.LampOnDescription.Trim());
                string full_Path = cls_GlobalValue.ModelPath + "\\" + modelNumber.ToString() + "_" + modelName + ".mpp";

                if (MessageBox.Show(modelName + " : delete this file?", "DELETE", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    if (cls_File.FileDelete(full_Path))
                    {
                        if (lv_ModelList.Items.Count > 0)
                        {
                            for (int i = 0; i < lv_ModelList.Items.Count; i++)
                            {
                                ListViewItem item = lv_ModelList.Items[i];
                                if (item.SubItems[0].Text == modelNumber.ToString())
                                {
                                    lv_ModelList.Items.RemoveAt(i);
                                    cls_GlobalValue.LastModelPath = "";
                                    cls_GlobalValue.WriteIniValue("VPRO", "LastModelSavePath", cls_GlobalValue.LastModelPath);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("The loaded model has already been deleted from the model list.");
                        }

                        lp_ModelNumberLoad.Off();
                        lp_ModelNameLoad.Off();
                    }
                    else
                    {
                        MessageBox.Show("delete Fail.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please load an item.");
            }
        }
        private void btn_Edit_Click(object sender, EventArgs e)
        {
            if (cls_GlobalValue.LastModelPath != string.Empty)
            {
                frm_Model_Copy frm = new frm_Model_Copy(mModel, "Edit");
                frm.ShowDialog();

                RecipeReflash();
            }
            else
            {
                MessageBox.Show("Please Load Model");
            }
        }

        private void btn_Copy_Click(object sender, EventArgs e)
        {
            if (cls_GlobalValue.LastModelPath != string.Empty)
            {
                frm_Model_Copy frm = new frm_Model_Copy(mModel, "Copy");
                frm.ShowDialog();

                RecipeReflash();
            }
            else
            {
                MessageBox.Show("Please Load Model");
            }
        }

        private void lv_ModelList_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (lv_ModelList.SelectedItems.Count > 0)
                {
                    string full_Path = cls_GlobalValue.ModelPath + "\\" + lv_ModelList.SelectedItems[0].SubItems[0].Text + "_" + lv_ModelList.SelectedItems[0].SubItems[1].Text + ".mpp";
                    RecipeLoad(full_Path);

                    MessageBox.Show("Model Change Complete");
                }
            }
            catch (Exception ex) { MessageBox.Show($"Model Change Error : {ex.Message}"); }
        }

        private void RecipeLoad(string _fullPath)
        {
            this.Cursor = Cursors.WaitCursor;
            mModel = new cls_Model();

            if (mModel.Model_Load(_fullPath))
            {
                lp_ModelNumberLoad.LampOnDescription = mModel.ModelNumber.ToString();
                lp_ModelNumberLoad.On();
                lp_ModelNameLoad.LampOnDescription = mModel.ModelName;
                lp_ModelNameLoad.On();

                cls_GlobalValue.LastModelPath = _fullPath;
                cls_GlobalValue.WriteIniValue("VPRO", "LastModelSavePath", cls_GlobalValue.LastModelPath);

            }
            else
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show("Model Load Fail.");
            }

            this.Cursor = Cursors.Default;
        }

        private void RecipeReflash()
        {
            DirectoryInfo di = new DirectoryInfo(cls_GlobalValue.ModelPath);
            if (!di.Exists)
            {
                di.Create();
            }
            FileInfo[] fi = di.GetFiles("*.mpp");

            if (fi.Length > 0)
            {
                Array.Sort<FileInfo>(fi, delegate (FileInfo x, FileInfo y) { return x.Name.CompareTo(y.Name); });
                lv_ModelList.Items.Clear();

                for (int i = 0; i < fi.Length; i++)
                {
                    if (fi[i].Name.Contains("_"))
                    {
                        string[] strSplit = fi[i].Name.Split('_');
                        string num = strSplit[0];
                        string name = fi[i].Name.Substring(num.Length + 1).Replace(".mpp", "");

                        ListViewItem item = new ListViewItem(num);
                        item.SubItems.Add(name);
                        lv_ModelList.Items.Add(item);
                    }
                }
            }

            if (File.Exists(cls_GlobalValue.LastModelPath))
            {
                RecipeLoad(cls_GlobalValue.LastModelPath);
            }
        }

    }
}