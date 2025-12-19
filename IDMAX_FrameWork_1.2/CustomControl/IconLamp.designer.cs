using System.Windows.Forms;

namespace IDMAX_FrameWork
{
    partial class IconLamp
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
            this.tlpMain = new TableLayoutPanel();
            this.pbIcon = new PictureBox();
            this.lblLamp = new CustomLabel();
            this.tlpMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.BackColor = System.Drawing.Color.Transparent;
            this.tlpMain.ColumnCount = 3;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tlpMain.Controls.Add(this.pbIcon, 1, 0);
            this.tlpMain.Controls.Add(this.lblLamp, 2, 0);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 1;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Size = new System.Drawing.Size(105, 23);
            this.tlpMain.TabIndex = 0;
            // 
            // pbIcon
            // 
            this.pbIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pbIcon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbIcon.Location = new System.Drawing.Point(5, 0);
            this.pbIcon.Margin = new System.Windows.Forms.Padding(0);
            this.pbIcon.Name = "pbIcon";
            this.pbIcon.Size = new System.Drawing.Size(32, 32);
            this.pbIcon.TabIndex = 0;
            this.pbIcon.TabStop = false;
            this.pbIcon.DoubleClick += new System.EventHandler(Control_DoubleClick);

            // 
            // lblLamp
            // 
            this.lblLamp.AutoSize = true;
            this.lblLamp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLamp.EnableMarquee = false;
            this.lblLamp.Location = new System.Drawing.Point(30, 0);
            this.lblLamp.Margin = new System.Windows.Forms.Padding(0);
            this.lblLamp.Name = "lblLamp";
            this.lblLamp.Opacity = 100F;
            this.lblLamp.OutlineColor = System.Drawing.Color.Empty;
            this.lblLamp.ShadowColor = System.Drawing.Color.Empty;
            this.lblLamp.ShadowOffset = new System.Drawing.Point(0, 0);
            this.lblLamp.Size = new System.Drawing.Size(60, 32);
            this.lblLamp.StringDirection = CustomLabel.Direction.Left;
            this.lblLamp.TabIndex = 1;
            this.lblLamp.Text = "Title";
            this.lblLamp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblLamp.UseClearType = true;
            this.lblLamp.UseOutline = false;
            this.lblLamp.UseShadow = false;
            this.lblLamp.DoubleClick += new System.EventHandler(Control_DoubleClick);
            // 
            // IconLamp
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.DimGray;
            this.Controls.Add(this.tlpMain);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "IconLamp";
            this.Size = new System.Drawing.Size(105, 32);
            this.tlpMain.ResumeLayout(false);
            this.tlpMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel tlpMain;
        private PictureBox pbIcon;
        private CustomLabel lblLamp;
    }
}
