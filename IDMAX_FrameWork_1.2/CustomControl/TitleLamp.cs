using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace IDMAX_FrameWork
{
    public partial class TitleLamp : UserControl
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


        public TitleLamp()
        {
            InitializeComponent();
        }

        [DefaultValue("Title")]
        public string TitleText
        {
            get { return this.lblTitle.Text; }
            set { this.lblTitle.Text = value; }
        }

        public Font TitleFont
        {
            get { return this.lblTitle.Font; }
            set { this.lblTitle.Font = value; }
        }

        public Color TitleBackColor
        {
            get { return this.lblTitle.BackColor; }
            set { this.lblTitle.BackColor = value; }
        }

        public Color TitleForeColor
        {
            get { return this.lblTitle.ForeColor; }
            set { this.lblTitle.ForeColor = value; }
        }

        public Font DescriptionFont
        {
            get { return this.lblLamp.Font; }
            set { this.lblLamp.Font = value; }
        }

        public Color DescriptionForeColor
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


        private delegate void OnControlChange();
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
            if (this.InvokeRequired)
            {
                OnControlChange onControlChange = null;
                onControlChange = new OnControlChange(On);
                this.Invoke(onControlChange);
            }
            else
            {

                this._Enable = true;

                this.lblLamp.BackColor = this._LampOnColor;
                this.lblLamp.Text = this._LampOnDescription;
            }
        }



        public void Off()
        {
            if (this.InvokeRequired)
            {
                OnControlChange onControlChange = null;
                onControlChange = new OnControlChange(Off);
                this.Invoke(onControlChange);
            }
            else
            {

                this._Enable = false;

                this.lblLamp.BackColor = this._LampOffColor;
                this.lblLamp.Text = this._LampOffDescription;
            }
          
        }
    }
}
