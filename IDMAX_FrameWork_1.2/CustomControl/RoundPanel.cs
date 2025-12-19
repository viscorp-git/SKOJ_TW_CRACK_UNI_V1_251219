using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Helper;

namespace IDMAX_FrameWork
{
    public class RoundPanel : ScrollableControl
    {
        public RoundPanel()
        {
            this.SetStyle
                (ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
            this.UpdateStyles();

            base.BackColor = Color.Transparent;
        }

        #region BackgroundColor
        [Browsable(false)]
        [ReadOnly(true)]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }

        private Color _backgroundColor = SystemColors.Control;
        public Color BackgroundColor
        {
            get { return this._backgroundColor; }
            set
            {
                if (this._backgroundColor == value) return;

                this._backgroundColor = value;
                this.Invalidate();
            }
        }
        #endregion

        #region Radius
        private int _radius = 10;
        public int Radius
        {
            get { return this._radius; }
            set
            {
                if (this._radius == value) return;
                
                this._radius = value;
                this.Invalidate();
            }
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
                this.Invalidate();
            }
        }
        #endregion

        #region Gradien
        private bool _gradien = false;
        public bool Gradien
        {
            get { return this._gradien; }
            set 
            {
                if (this._gradien == value) return;

                this._gradien = value;
                this.Invalidate();
            }
        }

        private Color _gradienStartColor = Color.Black;
        public Color GradienStartColor
        {
            get { return this._gradienStartColor; }
            set 
            {
                if (this._gradienStartColor == value) return;

                this._gradienStartColor = value;
                this.Invalidate();
            }
        }

        private Color _gradienEndColor = Color.Black;
        public Color GradienEndColor
        {
            get { return this._gradienEndColor; }
            set 
            {
                if (this._gradienEndColor == value) return;

                this._gradienEndColor = value;
                this.Invalidate();
            }
        }
        #endregion

        #region Border
        private bool _border = false;
        public bool Border
        {
            get { return this._border; }
            set
            {
                if (this._border == value) return;

                this._border = value;
                this.Invalidate();
            }
        }

        private Color _borderColor = Color.Black;
        public Color BorderColor
        {
            get { return this._borderColor; }
            set 
            {
                if (this._borderColor == value) return;

                this._borderColor = value;
                this.Invalidate();
            }
        }

        private int _borderWidth = 0;
        public int BorderWidth
        {
            get { return this._borderWidth; }
            set
            {
                if (this._borderWidth == value) return;

                this._borderWidth = value;
                this.Invalidate();
            }
        }
        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!GDIHelper.PaintRound(
                e.Graphics, this, this._radius, this._actualOpacity, this._backgroundColor,
                this._gradien, this._gradienStartColor, this._gradienEndColor,
                this._border, this._borderColor, this._borderWidth))
            {
                base.OnPaint(e);
            }
        }
    }
}
