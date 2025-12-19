using System.Windows.Forms;
namespace IDMAX_FrameWork
{
    partial class KeyPad
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
            this.tlpBackground = new TableLayoutPanel();
            this.btnZum = new Button();
            this.btn7 = new Button();
            this.btn8 = new Button();
            this.btn9 = new Button();
            this.btn4 = new Button();
            this.btn5 = new Button();
            this.btn6 = new Button();
            this.btn1 = new Button();
            this.btn2 = new Button();
            this.btn3 = new Button();
            this.btn0 = new Button();
            this.btnBackspace = new Button();
            this.tlpBackground.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpBackground
            // 
            this.tlpBackground.ColumnCount = 3;
            this.tlpBackground.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlpBackground.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlpBackground.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlpBackground.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpBackground.Controls.Add(this.btnZum, 0, 3);
            this.tlpBackground.Controls.Add(this.btn7, 0, 0);
            this.tlpBackground.Controls.Add(this.btn8, 1, 0);
            this.tlpBackground.Controls.Add(this.btn9, 2, 0);
            this.tlpBackground.Controls.Add(this.btn4, 0, 1);
            this.tlpBackground.Controls.Add(this.btn5, 1, 1);
            this.tlpBackground.Controls.Add(this.btn6, 2, 1);
            this.tlpBackground.Controls.Add(this.btn1, 0, 2);
            this.tlpBackground.Controls.Add(this.btn2, 1, 2);
            this.tlpBackground.Controls.Add(this.btn3, 2, 2);
            this.tlpBackground.Controls.Add(this.btn0, 0, 3);
            this.tlpBackground.Controls.Add(this.btnBackspace, 2, 3);
            this.tlpBackground.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpBackground.Location = new System.Drawing.Point(3, 3);
            this.tlpBackground.Name = "tlpBackground";
            this.tlpBackground.RowCount = 4;
            this.tlpBackground.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpBackground.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpBackground.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpBackground.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpBackground.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpBackground.Size = new System.Drawing.Size(309, 254);
            this.tlpBackground.TabIndex = 0;
            // 
            // btnZum
            // 
            this.btnZum.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnZum.Location = new System.Drawing.Point(108, 194);
            this.btnZum.Margin = new System.Windows.Forms.Padding(5);
            this.btnZum.Name = "btnZum";
            this.btnZum.Size = new System.Drawing.Size(93, 55);
            this.btnZum.TabIndex = 29;
            this.btnZum.Text = "Enter";
            this.btnZum.Click += new System.EventHandler(this.btnKeypad_Click);
            // 
            // btn7
            // 
            this.btn7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn7.Location = new System.Drawing.Point(5, 5);
            this.btn7.Margin = new System.Windows.Forms.Padding(5);
            this.btn7.Name = "btn7";
            this.btn7.Size = new System.Drawing.Size(93, 53);
            this.btn7.TabIndex = 18;
            this.btn7.Text = "7";
            this.btn7.Click += new System.EventHandler(this.btnKeypad_Click);
            // 
            // btn8
            // 
            this.btn8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn8.Location = new System.Drawing.Point(108, 5);
            this.btn8.Margin = new System.Windows.Forms.Padding(5);
            this.btn8.Name = "btn8";
            this.btn8.Size = new System.Drawing.Size(93, 53);
            this.btn8.TabIndex = 19;
            this.btn8.Text = "8";
            this.btn8.Click += new System.EventHandler(this.btnKeypad_Click);
            // 
            // btn9
            // 
            this.btn9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn9.Location = new System.Drawing.Point(211, 5);
            this.btn9.Margin = new System.Windows.Forms.Padding(5);
            this.btn9.Name = "btn9";
            this.btn9.Size = new System.Drawing.Size(93, 53);
            this.btn9.TabIndex = 20;
            this.btn9.Text = "9";
            this.btn9.Click += new System.EventHandler(this.btnKeypad_Click);
            // 
            // btn4
            // 
            this.btn4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn4.Location = new System.Drawing.Point(5, 68);
            this.btn4.Margin = new System.Windows.Forms.Padding(5);
            this.btn4.Name = "btn4";
            this.btn4.Size = new System.Drawing.Size(93, 53);
            this.btn4.TabIndex = 21;
            this.btn4.Text = "4";
            this.btn4.Click += new System.EventHandler(this.btnKeypad_Click);
            // 
            // btn5
            // 
            this.btn5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn5.Location = new System.Drawing.Point(108, 68);
            this.btn5.Margin = new System.Windows.Forms.Padding(5);
            this.btn5.Name = "btn5";
            this.btn5.Size = new System.Drawing.Size(93, 53);
            this.btn5.TabIndex = 22;
            this.btn5.Text = "5";
            this.btn5.Click += new System.EventHandler(this.btnKeypad_Click);
            // 
            // btn6
            // 
            this.btn6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn6.Location = new System.Drawing.Point(211, 68);
            this.btn6.Margin = new System.Windows.Forms.Padding(5);
            this.btn6.Name = "btn6";
            this.btn6.Size = new System.Drawing.Size(93, 53);
            this.btn6.TabIndex = 23;
            this.btn6.Text = "6";
            this.btn6.Click += new System.EventHandler(this.btnKeypad_Click);
            // 
            // btn1
            // 
            this.btn1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn1.Location = new System.Drawing.Point(5, 131);
            this.btn1.Margin = new System.Windows.Forms.Padding(5);
            this.btn1.Name = "btn1";
            this.btn1.Size = new System.Drawing.Size(93, 53);
            this.btn1.TabIndex = 24;
            this.btn1.Text = "1";
            this.btn1.Click += new System.EventHandler(this.btnKeypad_Click);
            // 
            // btn2
            // 
            this.btn2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn2.Location = new System.Drawing.Point(108, 131);
            this.btn2.Margin = new System.Windows.Forms.Padding(5);
            this.btn2.Name = "btn2";
            this.btn2.Size = new System.Drawing.Size(93, 53);
            this.btn2.TabIndex = 25;
            this.btn2.Text = "2";
            this.btn2.Click += new System.EventHandler(this.btnKeypad_Click);
            // 
            // btn3
            // 
            this.btn3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn3.Location = new System.Drawing.Point(211, 131);
            this.btn3.Margin = new System.Windows.Forms.Padding(5);
            this.btn3.Name = "btn3";
            this.btn3.Size = new System.Drawing.Size(93, 53);
            this.btn3.TabIndex = 26;
            this.btn3.Text = "3";
            this.btn3.Click += new System.EventHandler(this.btnKeypad_Click);
            // 
            // btn0
            // 
            this.btn0.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn0.Location = new System.Drawing.Point(5, 194);
            this.btn0.Margin = new System.Windows.Forms.Padding(5);
            this.btn0.Name = "btn0";
            this.btn0.Size = new System.Drawing.Size(93, 55);
            this.btn0.TabIndex = 28;
            this.btn0.Text = "0";
            this.btn0.Click += new System.EventHandler(this.btnKeypad_Click);
            // 
            // btnBackspace
            // 
            this.btnBackspace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnBackspace.Location = new System.Drawing.Point(211, 194);
            this.btnBackspace.Margin = new System.Windows.Forms.Padding(5);
            this.btnBackspace.Name = "btnBackspace";
            this.btnBackspace.Size = new System.Drawing.Size(93, 55);
            this.btnBackspace.TabIndex = 27;
            this.btnBackspace.Text = "←";
            this.btnBackspace.Click += new System.EventHandler(this.btnBackspace_Click);
            // 
            // KeyPad
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.tlpBackground);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "KeyPad";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(315, 260);
            this.tlpBackground.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel tlpBackground;
        private Button btn7;
        private Button btn3;
        private Button btn2;
        private Button btn8;
        private Button btn9;
        private Button btn4;
        private Button btn5;
        private Button btn6;
        private Button btn1;
        private Button btn0;
        private Button btnBackspace;
        private Button btnZum;
    }
}
