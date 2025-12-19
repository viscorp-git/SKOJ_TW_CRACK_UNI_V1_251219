namespace SKON_TabWelldingInspection
{
    partial class frm_Loader
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_Loader));
            this.iTalk_ThemeContainer6 = new IDMAX_FrameWork.iTalk_ThemeContainer();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.lb_LoadingText = new IDMAX_FrameWork.iTalk_HeaderLabel();
            this.pgB_Loading = new System.Windows.Forms.ProgressBar();
            this.iTalk_ThemeContainer6.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // iTalk_ThemeContainer6
            // 
            this.iTalk_ThemeContainer6.BackColor = System.Drawing.Color.DimGray;
            this.iTalk_ThemeContainer6.Controls.Add(this.tableLayoutPanel4);
            this.iTalk_ThemeContainer6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.iTalk_ThemeContainer6.Font = new System.Drawing.Font("Tahoma", 8F);
            this.iTalk_ThemeContainer6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(142)))), ((int)(((byte)(142)))));
            this.iTalk_ThemeContainer6.Location = new System.Drawing.Point(0, 0);
            this.iTalk_ThemeContainer6.Margin = new System.Windows.Forms.Padding(0);
            this.iTalk_ThemeContainer6.MinimumSize = new System.Drawing.Size(126, 39);
            this.iTalk_ThemeContainer6.Name = "iTalk_ThemeContainer6";
            this.iTalk_ThemeContainer6.Padding = new System.Windows.Forms.Padding(1, 23, 1, 1);
            this.iTalk_ThemeContainer6.Sizable = false;
            this.iTalk_ThemeContainer6.Size = new System.Drawing.Size(529, 255);
            this.iTalk_ThemeContainer6.SmartBounds = false;
            this.iTalk_ThemeContainer6.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.iTalk_ThemeContainer6.TabIndex = 11;
            this.iTalk_ThemeContainer6.Text = "Loader";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 31.70732F));
            this.tableLayoutPanel4.Controls.Add(this.lb_LoadingText, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.pgB_Loading, 0, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(1, 23);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(527, 231);
            this.tableLayoutPanel4.TabIndex = 2;
            // 
            // lb_LoadingText
            // 
            this.lb_LoadingText.AutoSize = true;
            this.lb_LoadingText.BackColor = System.Drawing.Color.WhiteSmoke;
            this.lb_LoadingText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_LoadingText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lb_LoadingText.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_LoadingText.ForeColor = System.Drawing.Color.Black;
            this.lb_LoadingText.Location = new System.Drawing.Point(1, 1);
            this.lb_LoadingText.Margin = new System.Windows.Forms.Padding(0);
            this.lb_LoadingText.Name = "lb_LoadingText";
            this.lb_LoadingText.Size = new System.Drawing.Size(525, 179);
            this.lb_LoadingText.TabIndex = 2;
            this.lb_LoadingText.Text = "iTalk_HeaderLabel1";
            this.lb_LoadingText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pgB_Loading
            // 
            this.pgB_Loading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgB_Loading.Location = new System.Drawing.Point(1, 181);
            this.pgB_Loading.Margin = new System.Windows.Forms.Padding(0);
            this.pgB_Loading.Maximum = 8;
            this.pgB_Loading.Name = "pgB_Loading";
            this.pgB_Loading.Size = new System.Drawing.Size(525, 49);
            this.pgB_Loading.Step = 1;
            this.pgB_Loading.TabIndex = 0;
            // 
            // frm_Loader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(529, 255);
            this.ControlBox = false;
            this.Controls.Add(this.iTalk_ThemeContainer6);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(126, 39);
            this.Name = "frm_Loader";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MX INSPECTION LOADER";
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.iTalk_ThemeContainer6.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private IDMAX_FrameWork.iTalk_ThemeContainer iTalk_ThemeContainer6;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private IDMAX_FrameWork.iTalk_HeaderLabel lb_LoadingText;
        private System.Windows.Forms.ProgressBar pgB_Loading;
    }
}