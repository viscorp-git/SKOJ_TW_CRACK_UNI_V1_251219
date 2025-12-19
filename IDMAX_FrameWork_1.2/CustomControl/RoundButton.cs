using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;


namespace IDMAX_FrameWork
{
    [System.Flags]

    public enum Corners
    {
        None = 0,
        TopLeft = 1,
        TopRight = 2,
        BottomLeft = 4,
        BottomRight = 8,
        All = TopLeft | TopRight | BottomLeft | BottomRight
    }

    public enum CustomButtonState
    {
        Normal = 1,
        Hot,
        Pressed,
        Disabled,
        Focused
    }

    public class RoundButton : Control, IButtonControl
    {
        private bool m_IsDefault;
        private int m_CornerRadius = 8;

        private Corners m_RoundCorners;
        private CustomButtonState m_ButtonState = CustomButtonState.Normal;

        private ContentAlignment m_ImageAlign = ContentAlignment.MiddleCenter;
        private ContentAlignment m_TextAlign = ContentAlignment.MiddleCenter;
        private ImageList m_ImageList;
        private int m_ImageIndex = -1;

        private bool keyPressed;
        private Rectangle contentRect;

        public RoundButton() : base()
        {
            this.SetStyle(
                ControlStyles.Selectable | ControlStyles.StandardClick | ControlStyles.ResizeRedraw | 
                ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.UserPaint | 
                ControlStyles.SupportsTransparentBackColor, true);
        }

        private DialogResult m_DialogResult;
        public DialogResult DialogResult
        {
            get { return m_DialogResult; }
            set
            {
                if (Enum.IsDefined(typeof(DialogResult), value)) m_DialogResult = value;
            }
        }
        
        public void NotifyDefault(bool value)
        {
            if (m_IsDefault != value) m_IsDefault = value;
            
            this.Invalidate();
        }
        
        public void PerformClick()
        {
            if (this.CanSelect) base.OnClick(EventArgs.Empty);
        }

        //ButtonState

        [Browsable(false)]
        public CustomButtonState ButtonState
        {
            get { return m_ButtonState; }
        }
        
        [DefaultValue(8)]
        public int CornerRadius
        {
            get { return m_CornerRadius; }
            set
            {
                if (m_CornerRadius == value) return;
                
                m_CornerRadius = value;
                this.Invalidate();
            }
        }
        
        protected override System.Drawing.Size DefaultSize
        {
            get { return new Size(75, 23); }
        }
        
        [Browsable(false)]
        public bool IsDefault
        {
            get { return m_IsDefault; }
        }
        
        [DefaultValue(typeof(ImageList), null)]
        public ImageList ImageList
        {
            get { return m_ImageList; }
            set
            {
                m_ImageList = value;
                this.Invalidate();
            }
        }
        
        [DefaultValue(-1)]
        [TypeConverter(typeof(ImageIndexConverter))]
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
        public int ImageIndex
        {
            get { return m_ImageIndex; }
            set
            {
                m_ImageIndex = value;
                this.Invalidate();
            }
        }
        
        [DefaultValue(typeof(ContentAlignment), "MiddleCenter")]
        public ContentAlignment ImageAlign
        {
            get { return m_ImageAlign; }
            set
            {
                if (!Enum.IsDefined(typeof(ContentAlignment), value))
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(ContentAlignment));

                if (m_ImageAlign == value) return;

                m_ImageAlign = value;
                this.Invalidate();
            }
        }
        
        [DefaultValue(typeof(Corners), "None")]
        //[Editor(typeof(RoundCornersEditor), typeof(UITypeEditor))]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Corners RoundCorners
        {
            get { return m_RoundCorners; }
            set
            {
                if (m_RoundCorners == value) return;

                m_RoundCorners = value;
                this.Invalidate();
            }
        }
        
        [DefaultValue(typeof(ContentAlignment), "MiddleCenter")]
        public ContentAlignment TextAlign
        {
            get { return m_TextAlign; }
            set
            {
                if (!Enum.IsDefined(typeof(ContentAlignment), value))
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(ContentAlignment));

                if (m_TextAlign == value) return;

                m_TextAlign = value;
                this.Invalidate();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            
            if (e.KeyCode == Keys.Space)
            {
                keyPressed = true;
                m_ButtonState = CustomButtonState.Pressed;
            }
            
            OnStateChange(EventArgs.Empty);
        }
        
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.KeyCode == Keys.Space)
            {
                if (this.ButtonState == CustomButtonState.Pressed) this.PerformClick();

                keyPressed = false;
                m_ButtonState = CustomButtonState.Focused;
            }
            
            OnStateChange(EventArgs.Empty);
        }
        
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (!keyPressed) m_ButtonState = CustomButtonState.Hot;

            OnStateChange(EventArgs.Empty);
        }
        
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (!keyPressed)
                if (this.IsDefault)  m_ButtonState = CustomButtonState.Focused;
                else m_ButtonState = CustomButtonState.Normal;

            OnStateChange(EventArgs.Empty);
        }
        
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            
            if (e.Button == MouseButtons.Left)
            {
                this.Focus();
                m_ButtonState = CustomButtonState.Pressed;
            }

            OnStateChange(EventArgs.Empty);
        }
        
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            m_ButtonState = CustomButtonState.Focused;
            OnStateChange(EventArgs.Empty);
        }
        
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove (e);
            
            if (new Rectangle(Point.Empty, this.Size).Contains(e.X, e.Y) && e.Button == MouseButtons.Left)
                m_ButtonState = CustomButtonState.Pressed;
            else
            {
                if(keyPressed) return;
                
                m_ButtonState = CustomButtonState.Hot;
            }
            
            OnStateChange(EventArgs.Empty);
        }
        
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            m_ButtonState = CustomButtonState.Focused;
            this.NotifyDefault(true);
        }
        
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            if (this.FindForm().Focused) this.NotifyDefault(false);
            
            m_ButtonState = CustomButtonState.Normal;
        }
        
        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            
            if (this.Enabled)
                m_ButtonState = CustomButtonState.Normal;
            else
                m_ButtonState = CustomButtonState.Disabled;

            OnStateChange(EventArgs.Empty);
        }
        
        protected override void OnClick(EventArgs e)
        {
            if (this.ButtonState == CustomButtonState.Pressed)
            {
                this.Focus();

                this.PerformClick();
            }
        }
        
        protected override void OnDoubleClick(EventArgs e)
        {
            if (this.ButtonState == CustomButtonState.Pressed)
            {
                this.Focus();

                this.PerformClick();
            }
        }
        
        protected override bool ProcessMnemonic(char charCode)
        {
            if (IsMnemonic(charCode, this.Text))
            {
                base.OnClick(EventArgs.Empty);

                return true;
            }
            
            return base.ProcessMnemonic(charCode);
        }
        
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            this.Invalidate();
        }
        
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            System.Drawing.Drawing2D.GraphicsContainer g = pevent.Graphics.BeginContainer();
            
            Rectangle translateRect = this.Bounds;

            pevent.Graphics.TranslateTransform(-this.Left, -this.Top);
            
            PaintEventArgs pe = new PaintEventArgs(pevent.Graphics, translateRect);
            
            this.InvokePaintBackground(this.Parent, pe);
            this.InvokePaint(this.Parent, pe);
            
            pevent.Graphics.ResetTransform();
            pevent.Graphics.EndContainer(g);
            pevent.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            Color shadeColor, fillColor;
            Color darkColor = DarkenColor(this.BackColor, 10);
            Color darkDarkColor = DarkenColor(this.BackColor, 25);
            Color lightColor = LightenColor(this.BackColor, 25);
            Color lightLightColor = LightenColor(this.BackColor, 60);
            
            if (this.ButtonState == CustomButtonState.Hot)
            {
                fillColor = lightColor;
                
                shadeColor = darkDarkColor;
            }
            else if (this.ButtonState == CustomButtonState.Pressed)
            {
                fillColor = this.BackColor;
                
                shadeColor = this.BackColor;
            }
            else
            {
                fillColor = this.BackColor;
                
                shadeColor = darkDarkColor;
            }
            
            Rectangle r = this.ClientRectangle;

            System.Drawing.Drawing2D.GraphicsPath path = RoundRectangle(r, this.CornerRadius, this.RoundCorners);
            System.Drawing.Drawing2D.LinearGradientBrush paintBrush = new System.Drawing.Drawing2D.LinearGradientBrush(r, fillColor, shadeColor, System.Drawing.Drawing2D.LinearGradientMode.Vertical);
            System.Drawing.Drawing2D.Blend b = new System.Drawing.Drawing2D.Blend();
            
            b.Positions = new float[] {0, 0.45F, 0.55F, 1};
            b.Factors = new float[] {0, 0, 1, 1};
            paintBrush.Blend = b;
            
            pevent.Graphics.FillPath(paintBrush, path);
            
            paintBrush.Dispose();
            
            Pen drawingPen = new Pen(darkDarkColor);
            pevent.Graphics.DrawPath(drawingPen, path);
            drawingPen.Dispose();
            
            bool inBounds = false;
            
            while (!inBounds && r.Width >= 1 && r.Height >= 1)
            {
                inBounds = path.IsVisible(r.Left, r.Top) &&
                path.IsVisible(r.Right, r.Top) &&
                path.IsVisible(r.Left, r.Bottom) &&
                path.IsVisible(r.Right, r.Bottom);
                
                r.Inflate(-1, -1);
            }
            
            contentRect = r;
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            DrawImage(e.Graphics);
            DrawText(e.Graphics);
            DrawFocus(e.Graphics);

            base.OnPaint(e);
        }
        
        protected override void OnParentBackColorChanged(EventArgs e)
        {
            base.OnParentBackColorChanged(e);

            this.Invalidate();
        }
        
        protected override void OnParentBackgroundImageChanged(EventArgs e)
        {
            base.OnParentBackgroundImageChanged(e);

            this.Invalidate();
        }

        private void DrawImage(Graphics g)
        {
            if (this.ImageList == null || this.ImageIndex == -1) return;
            
            if (this.ImageIndex < 0 || this.ImageIndex >= this.ImageList.Images.Count) return;
            
            Image _Image = this.ImageList.Images[this.ImageIndex];
            
            Point pt = Point.Empty;
            
            switch (this.ImageAlign)
            {
                case ContentAlignment.TopLeft:
                    pt.X = contentRect.Left;
                    pt.Y = contentRect.Top;
                    
                    break;
                
                case ContentAlignment.TopCenter:
                    pt.X = (Width - _Image.Width) / 2;
                    pt.Y = contentRect.Top;
                    
                    break;
                
                case ContentAlignment.TopRight:
                    pt.X = contentRect.Right - _Image.Width;
                    pt.Y = contentRect.Top;
                    
                    break;
                
                case ContentAlignment.MiddleLeft:
                    pt.X = contentRect.Left;
                    pt.Y = (Height - _Image.Height) / 2;
                    
                    break;
                
                case ContentAlignment.MiddleCenter:
                    pt.X = (Width - _Image.Width) / 2;
                    pt.Y = (Height - _Image.Height) / 2;
                    
                    break;
                
                case ContentAlignment.MiddleRight:
                    pt.X = contentRect.Right - _Image.Width;
                    pt.Y = (Height - _Image.Height) / 2;
                    
                    break;
                
                case ContentAlignment.BottomLeft:
                    pt.X = contentRect.Left;
                    pt.Y = contentRect.Bottom - _Image.Height;
                    
                    break;
                
                case ContentAlignment.BottomCenter:
                    pt.X = (Width - _Image.Width) / 2;
                    pt.Y = contentRect.Bottom - _Image.Height;

                    break;
                
                case ContentAlignment.BottomRight:
                    pt.X = contentRect.Right - _Image.Width;
                    pt.Y = contentRect.Bottom - _Image.Height;
                    
                    break;
            }
            
            if (this.ButtonState == CustomButtonState.Pressed) pt.Offset(1, 1);
            
            if (this.Enabled) this.ImageList.Draw(g, pt, this.ImageIndex);
            else ControlPaint.DrawImageDisabled(g, _Image, pt.X, pt.Y, this.BackColor);
        }
        
        private void DrawText(Graphics g)
        {
            SolidBrush TextBrush = new SolidBrush(this.ForeColor);
            
            RectangleF R = (RectangleF)contentRect;
            
            if (!this.Enabled) TextBrush.Color = SystemColors.GrayText;
            
            StringFormat sf = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.NoClip);
            
            if (ShowKeyboardCues) sf.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
            else sf.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Hide;
            
            switch (this.TextAlign)
            {
                case ContentAlignment.TopLeft:
                    sf.Alignment = StringAlignment.Near;
                    sf.LineAlignment = StringAlignment.Near;
                    
                    break;
                
                case ContentAlignment.TopCenter:
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Near;
                    
                    break;

                case ContentAlignment.TopRight:
                    sf.Alignment = StringAlignment.Far;
                    sf.LineAlignment = StringAlignment.Near;

                    break;

                case ContentAlignment.MiddleLeft:
                    sf.Alignment = StringAlignment.Near;
                    sf.LineAlignment = StringAlignment.Center;

                    break;

                case ContentAlignment.MiddleCenter:
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;

                    break;

                case ContentAlignment.MiddleRight:
                    sf.Alignment = StringAlignment.Far;
                    sf.LineAlignment = StringAlignment.Center;

                    break;

                case ContentAlignment.BottomLeft:
                    sf.Alignment = StringAlignment.Near;
                    sf.LineAlignment = StringAlignment.Far;

                    break;

                case ContentAlignment.BottomCenter:
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Far;

                    break;

                case ContentAlignment.BottomRight:
                    sf.Alignment = StringAlignment.Far;
                    sf.LineAlignment = StringAlignment.Far;

                    break;
            }

            if (this.ButtonState == CustomButtonState.Pressed) R.Offset(1, 1);

            if (this.Enabled) g.DrawString(this.Text, this.Font, TextBrush, R, sf);
            else ControlPaint.DrawStringDisabled(g, this.Text, this.Font, this.BackColor, R, sf);
        }

        private void DrawFocus(Graphics g)
        {
            Rectangle r = contentRect;
            r.Inflate(1, 1);

            if (this.Focused && this.ShowFocusCues && this.TabStop)
                ControlPaint.DrawFocusRectangle(g, r, this.ForeColor, this.BackColor);
        }
        
        private System.Drawing.Drawing2D.GraphicsPath RoundRectangle(Rectangle r, int radius, Corners corners)
        {
            r.Width -= 1;
            r.Height -= 1;

            if (radius > (r.Width))
                radius = r.Width;

            if (radius > (r.Height))
                radius = r.Height;

            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();

            if (radius <= 0 )
                path.AddRectangle(r);
            else
                if ((corners & Corners.TopLeft) == Corners.TopLeft)
                path.AddArc(r.Left, r.Top, radius, radius, 180, 90);
            else
                path.AddLine(r.Left, r.Top, r.Left, r.Top);

            if ((corners & Corners.TopRight) == Corners.TopRight)
                path.AddArc(r.Right - radius, r.Top, radius, radius, 270, 90);
            else
                path.AddLine(r.Right, r.Top, r.Right, r.Top);

            if ((corners & Corners.BottomRight) == Corners.BottomRight)
                path.AddArc(r.Right - radius, r.Bottom - radius, radius, radius, 0, 90);
            else
                path.AddLine(r.Right, r.Bottom, r.Right, r.Bottom);

            if ((corners & Corners.BottomLeft) == Corners.BottomLeft)
                path.AddArc(r.Left, r.Bottom - radius, radius, radius, 90, 90);
            else
                path.AddLine(r.Left, r.Bottom, r.Left, r.Bottom);

            path.CloseFigure();

            return path;
        }

        private Color DarkenColor(Color colorIn, int percent)
        {
            if (percent < 0 || percent > 100)
                throw new ArgumentOutOfRangeException("percent");

            int a, r, g, b;

            a = colorIn.A;
            r = colorIn.R - (int)((colorIn.R / 100f) * percent);
            g = colorIn.G - (int)((colorIn.G / 100f) * percent);
            b = colorIn.B - (int)((colorIn.B / 100f) * percent);

            return Color.FromArgb(a, r, g, b);
        }

        private Color LightenColor(Color colorIn, int percent)
        {
            if (percent < 0 || percent > 100)
                throw new ArgumentOutOfRangeException("percent");

            int a, r, g, b;

            a = colorIn.A;
            r = colorIn.R + (int)(((255f - colorIn.R) / 100f) * percent);
            g = colorIn.G + (int)(((255f - colorIn.G) / 100f) * percent);
            b = colorIn.B + (int)(((255f - colorIn.B) / 100f) * percent);

            return Color.FromArgb(a, r, g, b);
        }

        private CustomButtonState currentState;

        private void OnStateChange(EventArgs e)
        {
            if (this.ButtonState == currentState)
                return;

            currentState = this.ButtonState;

            this.Invalidate();
        }
        
        [PermissionSetAttribute(SecurityAction.LinkDemand, Unrestricted=true)]
        [PermissionSetAttribute(SecurityAction.InheritanceDemand, Unrestricted=true)]
        public class RoundCornersEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.DropDown;
            }

            public override Object EditValue(ITypeDescriptorContext context, IServiceProvider provider, Object value)
            {
                if (value.GetType() != typeof(Corners) || provider == null)
                    return value;

                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (edSvc != null)
                {
                    CheckedListBox lb = new CheckedListBox();
                    lb.BorderStyle = BorderStyle.None;
                    lb.CheckOnClick = true;

                    lb.Items.Add("TopLeft", (((RoundButton)context.Instance).RoundCorners & Corners.TopLeft) == Corners.TopLeft);
                    lb.Items.Add("TopRight", (((RoundButton)context.Instance).RoundCorners & Corners.TopRight) == Corners.TopRight);
                    lb.Items.Add("BottomLeft", (((RoundButton)context.Instance).RoundCorners & Corners.BottomLeft) == Corners.BottomLeft);
                    lb.Items.Add("BottomRight", (((RoundButton)context.Instance).RoundCorners & Corners.BottomRight) == Corners.BottomRight);

                    edSvc.DropDownControl(lb);
                    Corners cornerFlags = Corners.None;

                    foreach (object o in lb.CheckedItems)
                    {
                        cornerFlags = cornerFlags | (Corners)Enum.Parse(typeof(Corners), o.ToString());
                    }

                    lb.Dispose();

                    edSvc.CloseDropDown();

                    return cornerFlags;
                }

                return value;
            }
        }
    }
}
