using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace IDMAX_FrameWork
{
    public partial class IconLamp : UserControl
    {


        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            e.Control.DoubleClick += new EventHandler(Control_DoubleClick);
        }
        protected override void OnControlRemoved(ControlEventArgs e)
        {
            e.Control.DoubleClick -= new EventHandler(Control_DoubleClick);
            base.OnControlRemoved(e);
        }
        void Control_DoubleClick(object sender, EventArgs e)
        {
            this.OnDoubleClick(e);
        }


        public IconLamp()
        {
            InitializeComponent();
        }
    

        public string TitleText
        {
            get { return this.lblLamp.Text; }
            set { this.lblLamp.Text = value; }
        }

        public Font TitleFont
        {
            get { return this.lblLamp.Font; }
            set { this.lblLamp.Font = value; }
        }

        public Color TitleForeColor
        {
            get { return this.lblLamp.ForeColor; }
            set { this.lblLamp.ForeColor = value; }
        }

        private Color _LampOnColor = Color.Lime;
        public Color LampOnColor
        {
            get { return this._LampOnColor; }
            set { this._LampOnColor = value; }
        }

        private Color _LampOffColor = Color.Tomato;
        public Color LampOffColor
        {
            get { return this._LampOffColor; }
            set
            {
                this._LampOffColor = value;
                //this.tlpMain.BackColor = value;
            }
        }

        private Color _errorColor = Color.Yellow;
        public Color ErrorColor
        {
            get { return this._errorColor; }
            set
            {
                this._errorColor = value;
               // this.tlpMain.BackColor = value;
            }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get { return this._errorMessage; }
            set { this._errorMessage = value; }
        }

        public int BorderWidth
        {
            get { return base.Padding.All; }
            set { base.Padding = new Padding(value); }
        }

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

        private Color _borderColor = SystemColors.Control;
        public Color BorderColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        [Browsable(false)]
        [ReadOnly(true)]
        private new Padding Padding
        {
            get { return base.Padding; }
            set { base.Padding = value; }
        }

        private bool _Enable = false;
        public bool Enable
        {
            get { return this._Enable; }
            set
            {
                this._Enable = value;

                if (value) On();
                else Off();
            }
        }

        private bool _error = false;
        public bool Error
        {
            get { return this._error; }
            set
            {
                this._error = value;

                if (value) this.ErrorOccur();
                else this.ErrorClear();
            }
        }

        private Image _OnImage = null;
        public Image OnImage
        {
            get { return this._OnImage; }
            set { this._OnImage = value; }
        }

        private Image _OffImage = null;
        public Image OffImage
        {
            get { return this._OffImage; }
            set 
            {
                this._OffImage = value;
                this.pbIcon.BackgroundImage = this._OffImage;
            }
        }

        private Image _errorImage = null;
        public Image ErrorImage
        {
            get { return this._errorImage; }
            set
            {
                this._errorImage = value;
                this.pbIcon.BackgroundImage = this._errorImage;
            }
        }

        public void ErrorOccur()
        {
            //this.tlpMain.BackColor = this._errorColor;
            this.lblLamp.ForeColor = this._errorColor;
            this.pbIcon.BackgroundImage = this._errorImage;
        }

        public void ErrorClear()
        {
            if (this._Enable) this.On();
            else this.Off();

            this._errorMessage = string.Empty;
        }

        public void On()
        {
            this._Enable = true;

            this.lblLamp.ForeColor = this._LampOnColor;
            this.pbIcon.BackgroundImage = this._OnImage;
        }

        public void Off()
        {
            this._Enable = false;

            this.lblLamp.ForeColor = this._LampOffColor;
            this.pbIcon.BackgroundImage = this._OffImage;
        }
    }
}
