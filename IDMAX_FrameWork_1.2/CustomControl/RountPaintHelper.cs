using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Helper
{
    public class GDIHelper
    {
        public static GraphicsPath GetRoundCorner(Control control, int radius)
        {
            try
            {
                Rectangle rect = new Rectangle(0, 0, control.Width - 1, control.Height - 1);

                int diameter = 2 * radius;
                Rectangle arcRect =
                    new Rectangle(rect.Location, new Size(diameter, diameter));

                GraphicsPath path = new GraphicsPath();

                path.AddArc(arcRect, 180, 90);

                arcRect.X = rect.Right - diameter;
                path.AddArc(arcRect, 270, 90);

                arcRect.Y = rect.Bottom - diameter;
                path.AddArc(arcRect, 0, 90);

                arcRect.X = rect.Left;
                path.AddArc(arcRect, 90, 90);

                path.CloseFigure();

                return path;
            }
            catch (Exception)
            {
                return new GraphicsPath();
            }
        }

        //public static Font AppropriateFont(Graphics g, float minFontSize,
        //float maxFontSize, Size layoutSize, string s, Font f, out SizeF extent)
        //{
        //    if (maxFontSize == minFontSize)
        //        f = new Font(f.FontFamily, minFontSize, f.Style);

        //    extent = g.MeasureString(s, f);

        //    if (maxFontSize <= minFontSize)
        //        return f;

        //    float hRatio = layoutSize.Height / extent.Height;
        //    float wRatio = layoutSize.Width / extent.Width;
        //    float ratio = (hRatio < wRatio) ? hRatio : wRatio;

        //    float newSize = f.Size * ratio;

        //    if (newSize < minFontSize)
        //        newSize = minFontSize;
        //    else if (newSize > maxFontSize)
        //        newSize = maxFontSize;

        //    f = new Font(f.FontFamily, newSize, f.Style);
        //    extent = g.MeasureString(s, f);

        //    return f;
        //}

        public static GraphicsPath GetStringPath(string s, float dpi, RectangleF rect, Font font, StringFormat format)
        {
            GraphicsPath path = new GraphicsPath();
            // Convert font size into appropriate coordinates
            float emSize = dpi * font.SizeInPoints / 72;
            path.AddString(s, font.FontFamily, (int)font.Style, emSize, rect, format);
            return path;
        }

        public static RectangleF DrawString(Graphics g, string text, Color foreColor,
            Rectangle rectangle, Font font, ContentAlignment textAlign,
            bool useShadow, Point shadowOffset, int opacity, Color shadowColor,
            bool useClearType,
            bool useOutline, Color outlineColor)
        {
            StringFormat format = new StringFormat();//StringFormat.GenericTypographic;

            #region Align
            switch (textAlign)
            {
                case ContentAlignment.TopLeft:
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Near;

                    break;

                case ContentAlignment.TopCenter:
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Near;

                    break;

                case ContentAlignment.TopRight:
                    format.Alignment = StringAlignment.Far;
                    format.LineAlignment = StringAlignment.Near;

                    break;

                case ContentAlignment.MiddleLeft:
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Center;

                    break;

                case ContentAlignment.MiddleCenter:
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;

                    break;

                case ContentAlignment.MiddleRight:
                    format.Alignment = StringAlignment.Far;
                    format.LineAlignment = StringAlignment.Center;

                    break;

                case ContentAlignment.BottomLeft:
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Far;

                    break;

                case ContentAlignment.BottomCenter:
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Far;

                    break;

                case ContentAlignment.BottomRight:
                    format.Alignment = StringAlignment.Far;
                    format.LineAlignment = StringAlignment.Far;

                    break;
            }
            #endregion

            using (GraphicsPath path = GDIHelper.GetStringPath(text, g.DpiY, rectangle, font, format))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                if (useShadow)
                {
                    RectangleF off = rectangle;
                    off.Offset(shadowOffset);
                    using (GraphicsPath offPath = GDIHelper.GetStringPath(text, g.DpiY, off, font, format))
                    {
                        using (Brush b = new SolidBrush(Color.FromArgb(opacity, shadowColor)))
                        {
                            g.FillPath(b, offPath);
                        }
                    }
                }

                if (useClearType)
                {
                    using (Brush brush = new SolidBrush(foreColor))
                    {
                        g.FillPath(brush, path);
                    }
                }

                if (useOutline)
                {
                    using (Pen pen = new Pen(outlineColor))
                    {
                        g.DrawPath(pen, path);
                    }
                }

                return path.GetBounds();
            }
        }

        public static bool PaintRound(
            Graphics g, Control control, int radius, int opacity, Color backgroundColor,
            bool gradien, Color gradienStartColor, Color gradienEndColor,
            bool border, Color borderColor, int borderWidth)
        {
            try
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (GraphicsPath path = GetRoundCorner(control, radius))
                {
                    if (gradien)
                    {
                        using (Brush brush = new LinearGradientBrush(
                                                    new Rectangle(0, 0, control.ClientRectangle.Width, control.ClientRectangle.Height),
                                                    Color.FromArgb(opacity, gradienStartColor),
                                                    Color.FromArgb(opacity, gradienEndColor),
                                                    90.0f))
                        {
                            g.FillPath(brush, path);
                        }
                    }
                    else
                    {
                        using (Brush brush = new SolidBrush(backgroundColor))
                        {
                            g.FillPath(brush, path);
                        }
                    }

                    if (border)
                    {
                        using (Pen pen = new Pen(Color.FromArgb(opacity, borderColor), borderWidth))
                        {
                            pen.Alignment = PenAlignment.Inset;

                            g.DrawPath(pen, path);
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
