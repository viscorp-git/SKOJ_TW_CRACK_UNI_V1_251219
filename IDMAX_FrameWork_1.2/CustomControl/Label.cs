using System;
using System.Drawing;
using System.Windows.Forms;
using Helper;

namespace IDMAX_FrameWork
{
    public class CustomLabel : Label
    {
        public enum Direction
        {
            Left,
            Right,
        }

        #region Outline
        public bool UseOutline
        {
            get;
            set;
        }

        public Color OutlineColor
        {
            get;
            set;
        }
        #endregion

        #region Shadow
        public bool UseShadow
        {
            get;
            set;
        }

        public Point ShadowOffset
        {
            get;
            set;
        }

        public Color ShadowColor
        {
            get;
            set;
        }
        #endregion

        #region Opacity
        private int _actualOpacity = 255;
        private float _opacity = 100;
        public float Opacity
        {
            get { return this._opacity; }
            set
            {
                if (value > 100) return;
                if (value < 0) return;
                if (this._opacity == value) return;

                this._opacity = value;
                this._actualOpacity = (int)(255 * value) / 100;
            }
        }
        #endregion

        public bool UseClearType
        {
            get;
            set;
        }

        //public bool AutoFontSize
        //{
        //    get;
        //    set;
        //}

        //public float MinFontSize
        //{
        //    get;
        //    set;
        //}

        //public float MaxFontSize
        //{
        //    get;
        //    set;
        //}

        private Rectangle _currentRectangle = new Rectangle();
        private Timer _marqueeTimer = null;

        private Direction _stringDirection = Direction.Left;
        public Direction StringDirection
        {
            get { return this._stringDirection; }
            set
            {
                if (this._stringDirection == value) return;

                this._stringDirection = value;
            }
        }

        private bool _enableMarquee = false;
        public bool EnableMarquee
        {
            get { return this._enableMarquee; }
            set
            {
                if (this._enableMarquee == value) return;

                this._enableMarquee = value;

                if (value) this.textSizeChecker();
            }
        }

        public override string Text
        {
            get { return base.Text; }
            set
            {
                if (value == null) value = string.Empty;

                base.Text = (this.EnableMarquee) ? value.Replace(Environment.NewLine, string.Empty) : value;
            }
        }

        public CustomLabel()
        {
            this.DoubleBuffered = true;

            this.ResizeRedraw = true;
            this.UseClearType = true;
            this.Margin = new System.Windows.Forms.Padding(3);

            this._marqueeTimer = new Timer();
            this._marqueeTimer.Interval = 30;
            this._marqueeTimer.Tick += new EventHandler(_marqueeTimer_Tick);
        }

        private void _marqueeTimer_Tick(object sender, EventArgs e)
        {
            this.positionCalculation();

            this.Invalidate();
        }

        private void drawImage(Graphics g)
        {
            if (this.Image == null) return;

            Point pt = Point.Empty;

            switch (this.ImageAlign)
            {
                case ContentAlignment.TopLeft:
                    pt.X = this.ClientRectangle.Left;
                    pt.Y = this.ClientRectangle.Top;

                    break;

                case ContentAlignment.TopCenter:
                    pt.X = (this.Width - this.Image.Width) / 2;
                    pt.Y = this.ClientRectangle.Top;

                    break;

                case ContentAlignment.TopRight:
                    pt.X = this.ClientRectangle.Right - this.Image.Width;
                    pt.Y = this.ClientRectangle.Top;

                    break;

                case ContentAlignment.MiddleLeft:
                    pt.X = this.ClientRectangle.Left;
                    pt.Y = (this.Height - this.Image.Height) / 2;

                    break;

                case ContentAlignment.MiddleCenter:
                    pt.X = (Width - this.Image.Width) / 2;
                    pt.Y = (Height - this.Image.Height) / 2;

                    break;

                case ContentAlignment.MiddleRight:
                    pt.X = this.ClientRectangle.Right - this.Image.Width;
                    pt.Y = (this.Height - this.Image.Height) / 2;

                    break;

                case ContentAlignment.BottomLeft:
                    pt.X = this.ClientRectangle.Left;
                    pt.Y = this.ClientRectangle.Bottom - this.Image.Height;

                    break;

                case ContentAlignment.BottomCenter:
                    pt.X = (this.Width - this.Image.Width) / 2;
                    pt.Y = this.ClientRectangle.Bottom - this.Image.Height;

                    break;

                case ContentAlignment.BottomRight:
                    pt.X = this.ClientRectangle.Right - this.Image.Width;
                    pt.Y = this.ClientRectangle.Bottom - this.Image.Height;

                    break;
            }

            g.DrawImage(this.Image, pt);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            this.drawImage(e.Graphics);

            if (this.UseClearType)
            {
                GDIHelper.DrawString(
                    e.Graphics, this.Text, this.ForeColor, this._currentRectangle, this.Font, this.TextAlign,
                    this.UseShadow, this.ShadowOffset, this._actualOpacity, this.ShadowColor,
                    this.UseClearType,
                    this.UseOutline, this.OutlineColor);
            }
            else base.OnPaint(e);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            this.textSizeChecker();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            this.textSizeChecker();
        }

        private void positionCalculation()
        {
            if (this.StringDirection == Direction.Left)
            {
                if (this._currentRectangle.X < -this._currentRectangle.Width)
                    this._currentRectangle.X = this.Width;
                else
                    this._currentRectangle.X -= 1;
            }
            else if (this.StringDirection == Direction.Right)
            {
                if (this._currentRectangle.X > this.Width)
                    this._currentRectangle.X = -this._currentRectangle.Width;
                else
                    this._currentRectangle.X += 1;
            }
        }

        private void textSizeChecker()
        {
            if (!this._enableMarquee)
            {
                this._currentRectangle = this.ClientRectangle;
            }
            else
            {
                int stringWidth = TextRenderer.MeasureText(this.Text, this.Font).Width;

                if (stringWidth >= this.Width)
                {
                    this._currentRectangle.Width = stringWidth + (int)(stringWidth * 0.1);

                    this.positionCalculation();
                }
                else
                {
                    this._currentRectangle = this.ClientRectangle;
                }

                if (TextRenderer.MeasureText(this.Text, this.Font).Width >= this.Width)
                {
                    if (!this._marqueeTimer.Enabled) this._marqueeTimer.Start();
                }
                else
                {
                    if (this._marqueeTimer.Enabled) this._marqueeTimer.Stop();

                    this.Invalidate();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._marqueeTimer != null)
                    this._marqueeTimer.Dispose();
            }
            this._marqueeTimer = null;

            base.Dispose(disposing);
        }
    }
}
