namespace BINPICKING_SYSTEM
{
    partial class StopWatchEx
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ProgressTimer = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.ProgressTimer, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(227, 72);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // ProgressTimer
            // 
            this.ProgressTimer.AutoSize = true;
            this.ProgressTimer.BackColor = System.Drawing.Color.Black;
            this.ProgressTimer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProgressTimer.Font = new System.Drawing.Font("나눔바른고딕", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.ProgressTimer.ForeColor = System.Drawing.Color.White;
            this.ProgressTimer.Location = new System.Drawing.Point(0, 0);
            this.ProgressTimer.Margin = new System.Windows.Forms.Padding(0);
            this.ProgressTimer.Name = "ProgressTimer";
            this.ProgressTimer.Size = new System.Drawing.Size(227, 72);
            this.ProgressTimer.TabIndex = 0;
            this.ProgressTimer.Text = "00:00:00.00";
            this.ProgressTimer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // StopWatchEx
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "StopWatchEx";
            this.Size = new System.Drawing.Size(227, 72);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label ProgressTimer;
    }
}
