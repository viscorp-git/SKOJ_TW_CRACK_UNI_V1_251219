using System.Windows.Forms;

namespace IDMAX_FrameWork
{
    partial class PingChecker
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
            this.Label1 = new CustomLabel();
            this.SuspendLayout();
            // 
            // Label1
            // 
            this.Label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label1.EnableMarquee = false;
            this.Label1.Location = new System.Drawing.Point(0, 0);
            this.Label1.Name = "Label1";
            this.Label1.Opacity = 100F;
            this.Label1.OutlineColor = System.Drawing.Color.Empty;
            this.Label1.ShadowColor = System.Drawing.Color.Empty;
            this.Label1.ShadowOffset = new System.Drawing.Point(0, 0);
            this.Label1.Size = new System.Drawing.Size(150, 150);
            this.Label1.StringDirection = CustomLabel.Direction.Left;
            this.Label1.TabIndex = 0;
            this.Label1.Text = "Label1";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Label1.UseClearType = false;
            this.Label1.UseOutline = false;
            this.Label1.UseShadow = false;
            // 
            // PingChecker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Label1);
            this.Name = "PingChecker";
            this.Load += new System.EventHandler(this.PingChecker_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CustomLabel Label1;
    }
}
