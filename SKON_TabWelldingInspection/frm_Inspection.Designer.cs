namespace SKON_TabWelldingInspection
{
    partial class frm_Inspection
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_Close = new IDMAX_FrameWork.GlassButton();
            this.iTalk_TabControl1 = new IDMAX_FrameWork.iTalk_TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cogToolBlockEditV21_Cathode = new Cognex.VisionPro.ToolBlock.CogToolBlockEditV2();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.cogToolBlockEditV21_Anode = new Cognex.VisionPro.ToolBlock.CogToolBlockEditV2();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_ImageLoad_Cathode = new System.Windows.Forms.Button();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_ImageLoad_Anode = new System.Windows.Forms.Button();
            this.btn_Save = new IDMAX_FrameWork.GlassButton();
            this.ofd_LoadImage_Cathode = new System.Windows.Forms.OpenFileDialog();
            this.ofd_LoadImage_Anode = new System.Windows.Forms.OpenFileDialog();
            this.iTalk_TabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cogToolBlockEditV21_Cathode)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cogToolBlockEditV21_Anode)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_Close
            // 
            this.btn_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Close.BackColor = System.Drawing.Color.LightGray;
            this.btn_Close.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Close.Location = new System.Drawing.Point(1319, 23);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(120, 40);
            this.btn_Close.TabIndex = 4;
            this.btn_Close.Text = "CLOSE";
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // iTalk_TabControl1
            // 
            this.iTalk_TabControl1.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.iTalk_TabControl1.Controls.Add(this.tabPage1);
            this.iTalk_TabControl1.Controls.Add(this.tabPage2);
            this.iTalk_TabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.iTalk_TabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.iTalk_TabControl1.ItemSize = new System.Drawing.Size(55, 135);
            this.iTalk_TabControl1.Location = new System.Drawing.Point(0, 63);
            this.iTalk_TabControl1.Multiline = true;
            this.iTalk_TabControl1.Name = "iTalk_TabControl1";
            this.iTalk_TabControl1.SelectedIndex = 0;
            this.iTalk_TabControl1.Size = new System.Drawing.Size(1439, 797);
            this.iTalk_TabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.iTalk_TabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this.tabPage1.Controls.Add(this.tableLayoutPanel1);
            this.tabPage1.Location = new System.Drawing.Point(139, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1296, 789);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Cathode";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this.tabPage2.Controls.Add(this.tableLayoutPanel2);
            this.tabPage2.Location = new System.Drawing.Point(139, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1296, 789);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Anode";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.cogToolBlockEditV21_Cathode, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1290, 783);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // cogToolBlockEditV21_Cathode
            // 
            this.cogToolBlockEditV21_Cathode.AllowDrop = true;
            this.cogToolBlockEditV21_Cathode.ContextMenuCustomizer = null;
            this.cogToolBlockEditV21_Cathode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cogToolBlockEditV21_Cathode.Location = new System.Drawing.Point(3, 53);
            this.cogToolBlockEditV21_Cathode.MinimumSize = new System.Drawing.Size(489, 0);
            this.cogToolBlockEditV21_Cathode.Name = "cogToolBlockEditV21_Cathode";
            this.cogToolBlockEditV21_Cathode.ShowNodeToolTips = true;
            this.cogToolBlockEditV21_Cathode.Size = new System.Drawing.Size(1284, 727);
            this.cogToolBlockEditV21_Cathode.SuspendElectricRuns = false;
            this.cogToolBlockEditV21_Cathode.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.cogToolBlockEditV21_Anode, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel4, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1290, 783);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // cogToolBlockEditV21_Anode
            // 
            this.cogToolBlockEditV21_Anode.AllowDrop = true;
            this.cogToolBlockEditV21_Anode.ContextMenuCustomizer = null;
            this.cogToolBlockEditV21_Anode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cogToolBlockEditV21_Anode.Location = new System.Drawing.Point(3, 53);
            this.cogToolBlockEditV21_Anode.MinimumSize = new System.Drawing.Size(489, 0);
            this.cogToolBlockEditV21_Anode.Name = "cogToolBlockEditV21_Anode";
            this.cogToolBlockEditV21_Anode.ShowNodeToolTips = true;
            this.cogToolBlockEditV21_Anode.Size = new System.Drawing.Size(1284, 727);
            this.cogToolBlockEditV21_Anode.SuspendElectricRuns = false;
            this.cogToolBlockEditV21_Anode.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.btn_ImageLoad_Cathode, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1290, 50);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(1234, 50);
            this.label1.TabIndex = 2;
            this.label1.Text = "Cathode Rulebase Setup";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btn_ImageLoad_Cathode
            // 
            this.btn_ImageLoad_Cathode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_ImageLoad_Cathode.Image = global::SKON_TabWelldingInspection.Properties.Resources.OpenDoc_32x32;
            this.btn_ImageLoad_Cathode.Location = new System.Drawing.Point(1240, 0);
            this.btn_ImageLoad_Cathode.Margin = new System.Windows.Forms.Padding(0);
            this.btn_ImageLoad_Cathode.Name = "btn_ImageLoad_Cathode";
            this.btn_ImageLoad_Cathode.Size = new System.Drawing.Size(50, 50);
            this.btn_ImageLoad_Cathode.TabIndex = 3;
            this.btn_ImageLoad_Cathode.UseVisualStyleBackColor = true;
            this.btn_ImageLoad_Cathode.Click += new System.EventHandler(this.btn_ImageLoad_Cathode_Click);
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel4.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.btn_ImageLoad_Anode, 1, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(1290, 50);
            this.tableLayoutPanel4.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(1234, 50);
            this.label2.TabIndex = 2;
            this.label2.Text = "Anode Rulebase Setup";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btn_ImageLoad_Anode
            // 
            this.btn_ImageLoad_Anode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_ImageLoad_Anode.Image = global::SKON_TabWelldingInspection.Properties.Resources.OpenDoc_32x32;
            this.btn_ImageLoad_Anode.Location = new System.Drawing.Point(1240, 0);
            this.btn_ImageLoad_Anode.Margin = new System.Windows.Forms.Padding(0);
            this.btn_ImageLoad_Anode.Name = "btn_ImageLoad_Anode";
            this.btn_ImageLoad_Anode.Size = new System.Drawing.Size(50, 50);
            this.btn_ImageLoad_Anode.TabIndex = 3;
            this.btn_ImageLoad_Anode.UseVisualStyleBackColor = true;
            this.btn_ImageLoad_Anode.Click += new System.EventHandler(this.btn_ImageLoad_Anode_Click);
            // 
            // btn_Save
            // 
            this.btn_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Save.BackColor = System.Drawing.Color.LightGray;
            this.btn_Save.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Save.Location = new System.Drawing.Point(1193, 23);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(120, 40);
            this.btn_Save.TabIndex = 6;
            this.btn_Save.Text = "Save";
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // frm_Inspection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1439, 860);
            this.Controls.Add(this.iTalk_TabControl1);
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.btn_Save);
            this.Name = "frm_Inspection";
            this.Padding = new System.Windows.Forms.Padding(0, 63, 0, 0);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Insepction Setup";
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_Inspection_FormClosing);
            this.Load += new System.EventHandler(this.frm_Inspection_Load);
            this.iTalk_TabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cogToolBlockEditV21_Cathode)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cogToolBlockEditV21_Anode)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private IDMAX_FrameWork.GlassButton btn_Close;
        private IDMAX_FrameWork.iTalk_TabControl iTalk_TabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Cognex.VisionPro.ToolBlock.CogToolBlockEditV2 cogToolBlockEditV21_Cathode;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private Cognex.VisionPro.ToolBlock.CogToolBlockEditV2 cogToolBlockEditV21_Anode;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_ImageLoad_Cathode;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_ImageLoad_Anode;
        private IDMAX_FrameWork.GlassButton btn_Save;
        private System.Windows.Forms.OpenFileDialog ofd_LoadImage_Cathode;
        private System.Windows.Forms.OpenFileDialog ofd_LoadImage_Anode;
    }
}