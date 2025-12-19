using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace IDMAX_FrameWork
{
    public partial class Lamp : UserControl
    {
        public Lamp()
        {
            InitializeComponent();
        }

        public Font LampFont
        {
            get { return this.lblLamp.Font; }
            set { this.lblLamp.Font = value; }
        }

        public Color LampForeColor
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
                this.lblLamp.BackColor = value;
            }
        }

        private string _LampOnDescription = string.Empty;
        public string LampOnDescription
        {
            get { return this._LampOnDescription; }
            set { this._LampOnDescription = value; }
        }

        private string _LampOffDescription = string.Empty;
        public string LampOffDescription
        {
            get { return this._LampOffDescription; }
            set
            {
                this._LampOffDescription = value;
                this.lblLamp.Text = value;
            }
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

        private bool _Blink = false;
        public bool Blink
        {
            get { return this._Blink; }
            set
            {
                this._Blink = value;

                if (value) Blink_On();
                else Blink_Off();
            }
        }


        private delegate void OnControlEnable();

        public void Blink_On()
        {
            if (this.InvokeRequired)
            {
                OnControlEnable onControlEnable = null;
                onControlEnable = new OnControlEnable(Blink_On);
                this.Invoke(onControlEnable);
            }
            else
            {

                this._Blink = true;

                if (Enable)
                {
                    this.lblLamp.BackColor = Color.White;
                    this.lblLamp.ForeColor = this._LampOnColor;
                }
                else
                {
                    this.lblLamp.BackColor = Color.White;
                    this.lblLamp.ForeColor = this._LampOffColor;
                }

            }
        }

        public void Blink_Off()
        {
            if (this.InvokeRequired)
            {
                OnControlEnable onControlEnable = null;
                onControlEnable = new OnControlEnable(Blink_Off);
                this.Invoke(onControlEnable);
            }
            else
            {

                this._Blink = false;

                if (Enable)
                {
                    this.lblLamp.BackColor = this._LampOnColor;
                    this.lblLamp.ForeColor = Color.White;
                }
                else
                {
                    this.lblLamp.BackColor = this._LampOffColor;
                    this.lblLamp.ForeColor = Color.White;
                }

            }
        }

        public void On()
        {
            this._Enable = true;

            this.lblLamp.BackColor = this._LampOnColor;
            this.lblLamp.Text = this._LampOnDescription;
        }

        public void Off()
        {
            this._Enable = false;

            this.lblLamp.BackColor = this._LampOffColor;
            this.lblLamp.Text = this._LampOffDescription;
        }

        public int BorderWidth
        {
            get { return base.Padding.All; }
            set { base.Padding = new Padding(value); }
        }

        [Browsable(false)]
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
        private new Padding Padding
        {
            get { return base.Padding; }
            set { base.Padding = value; }
        }
    }
}
