using System.Windows.Forms;

namespace IDMAX_FrameWork
{
    partial class TitleLamp
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.tlpMainLayout = new TableLayoutPanel();
            this.lblLamp = new Label();
            this.lblTitle = new Label();
            this.tlpMainLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpMainLayout
            // 
            this.tlpMainLayout.ColumnCount = 1;
            this.tlpMainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMainLayout.Controls.Add(this.lblLamp, 0, 1);
            this.tlpMainLayout.Controls.Add(this.lblTitle, 0, 0);
            this.tlpMainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMainLayout.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tlpMainLayout.Location = new System.Drawing.Point(0, 0);
            this.tlpMainLayout.Name = "tlpMainLayout";
            this.tlpMainLayout.RowCount = 2;
            this.tlpMainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35f));
            this.tlpMainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMainLayout.Size = new System.Drawing.Size(96, 96);
            this.tlpMainLayout.TabIndex = 0;
            // 
            // lblLamp
            // 
            this.lblLamp.AutoSize = true;
            this.lblLamp.BackColor = System.Drawing.Color.Tomato;
            this.lblLamp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLamp.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLamp.ForeColor = System.Drawing.Color.Black;
            this.lblLamp.Location = new System.Drawing.Point(0, 19);
            this.lblLamp.Margin = new System.Windows.Forms.Padding(0);
            this.lblLamp.Name = "lblLamp";
            this.lblLamp.Size = new System.Drawing.Size(96, 77);
            this.lblLamp.TabIndex = 1;
            this.lblLamp.Text = "Description";
            this.lblLamp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblLamp.DoubleClick += new System.EventHandler(Control_DoubleClick);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(96, 12);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Title";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Lamp
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.DimGray;
            this.Controls.Add(this.tlpMainLayout);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "Lamp";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.Size = new System.Drawing.Size(100, 100);
            this.tlpMainLayout.ResumeLayout(false);
            this.tlpMainLayout.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel tlpMainLayout;
        private Label lblLamp;
        private Label lblTitle;
    }
}
