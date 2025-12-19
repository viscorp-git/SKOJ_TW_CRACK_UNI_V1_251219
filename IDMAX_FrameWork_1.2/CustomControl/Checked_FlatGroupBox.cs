using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace IDMAX_FrameWork.CustomControl
{
    public enum CheckGroupBoxCheckAction
    {
        None, Enable, Disable,
    }
    public partial class Checked_FlatGroupBox : ContainerControl
    {
       
        private CheckBox m_checkBox;
        private bool m_contentsEnabled = true;
        private CheckGroupBoxCheckAction m_checkAction =
            CheckGroupBoxCheckAction.Enable;

        private int W;
        private int H;
        private bool _ShowText = true;

        [Category("Colors")]
        public Color BaseColor
        {
            get { return _BaseColor; }
            set { _BaseColor = value; }
        }

        public bool ShowText
        {
            get { return _ShowText; }
            set { _ShowText = value; }
        }

        [Category("Appearance")]
        [DefaultValue(true)]
        public bool Checked
        {
            get { return this.m_checkBox.Checked; }
            set { this.m_checkBox.Checked = value; }
        }

        [Category("Behavior")]
        [DefaultValue(true)]
        public bool ContentsEnabled
        {
            get { return this.m_contentsEnabled; }
            set
            {
                this.m_contentsEnabled = value;
                this.OnContentsEnabledChanged(EventArgs.Empty);
            }
        }

        private Color _BaseColor = Color.FromArgb(60, 70, 73);
        private Color _TextColor = Helpers.FlatColor;

        public Checked_FlatGroupBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            DoubleBuffered = true;
            BackColor = Color.Transparent;
            Size = new Size(240, 180);
            Font = new Font("Segoe ui", 10);

            this.m_checkBox = new CheckBox();
            this.m_checkBox.AutoSize = true;
            this.m_checkBox.Location = new Point(16,18);
            this.m_checkBox.Padding = new Padding(0, 0, 0, 0);
            this.m_checkBox.Checked = true;
            this.m_checkBox.TextAlign = ContentAlignment.MiddleLeft;
            this.m_checkBox.CheckedChanged += new EventHandler(CheckBox_CheckedChanged);
            this.Controls.Add(this.m_checkBox);
        }
        #region Event Handling
        /// <summary>
        /// CheckGroupBox.CheckBox CheckedChanged event.
        /// </summary>
        /// <param name=”e”></param>
        protected virtual void OnCheckedChanged(EventArgs e)
        {
            if (this.m_checkAction != CheckGroupBoxCheckAction.None)
            {
                // Toggle action depending on the value of checkAction.
                //   The ^ means a xor operation. The xor operation
                //   acts as a inversor, inverting the second operand
                //   whenever the first operand is true.

                this.ContentsEnabled =
                    (this.m_checkAction == CheckGroupBoxCheckAction.Disable)
                     ^ this.m_checkBox.Checked;
            }
        }

        /// <summary>
        /// ContentsEnabled Changed event.
        /// </summary>
        /// <param name=”e”></param>
        protected virtual void OnContentsEnabledChanged(EventArgs e)
        {
            this.SuspendLayout();
            foreach (Control control in this.Controls)
            {
                if (control != this.m_checkBox)
                {
                    // Set action for every control, except for
                    //  the CheckBox, which should remain intact.
                    control.Enabled = this.m_contentsEnabled;
                }
            }
            this.ResumeLayout(true);
        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.OnCheckedChanged(e);
        }
        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            this.UpdateColors();

            Bitmap B = new Bitmap(Width, Height);
            Graphics G = Graphics.FromImage(B);
            W = Width - 1;
            H = Height - 1;

            GraphicsPath GP = new GraphicsPath();
            GraphicsPath GP2 = new GraphicsPath();
            GraphicsPath GP3 = new GraphicsPath();
            Rectangle Base = new Rectangle(8, 8, W - 16, H - 16);

            var _with7 = G;
            _with7.SmoothingMode = SmoothingMode.HighQuality;
            _with7.PixelOffsetMode = PixelOffsetMode.HighQuality;
            _with7.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            _with7.Clear(BackColor);

            //-- Base
            GP = Helpers.RoundRec(Base, 8);
            _with7.FillPath(new SolidBrush(_BaseColor), GP);

            //-- Arrows
            //GP2 = Helpers.DrawArrow(28, 2, false);
            //_with7.FillPath(new SolidBrush(_BaseColor), GP2);
            //GP3 = Helpers.DrawArrow(28, 8, false);
            //_with7.FillPath(new SolidBrush(Color.FromArgb(60, 70, 73)), GP3);

            //-- if ShowText
            if (ShowText)
            {
                _with7.DrawString(Text, Font, new SolidBrush(_TextColor), new Rectangle(30, 16, W, H), Helpers.NearSF);
            }

            base.OnPaint(e);
            G.Dispose();
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.DrawImageUnscaled(B, 0, 0);
            B.Dispose();
        }

        private void UpdateColors()
        {
            FlatColors colors = Helpers.GetColors(this);

            _TextColor = colors.Flat;
        }
    }
}
