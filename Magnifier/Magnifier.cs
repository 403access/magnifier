using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Magnifier
{
    public partial class Magnifier : Form
    {
        private Timer updateTimer;
        private float zoomFactor = 2.0f;
        private int regionSize = 200;

        public Magnifier()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.BackColor = Color.Black;
            this.TransparencyKey = Color.Black;

            // Timer for updating the magnifier
            updateTimer = new Timer { Interval = 16 }; // ~60 FPS
            updateTimer.Tick += UpdateMagnifier;
            updateTimer.Start();

            // Set the initial size of the magnifier
            this.Size = new Size(regionSize * 2, regionSize * 2);
        }

        private void UpdateMagnifier(object sender, EventArgs e)
        {
            var cursorPos = Cursor.Position;

            // Adjust position dynamically
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            int offsetX = 20;

            if (cursorPos.X < screenWidth / 2)
                this.Left = cursorPos.X + offsetX;
            else
                this.Left = cursorPos.X - this.Width - offsetX;

            if (cursorPos.Y < screenHeight / 2)
                this.Top = cursorPos.Y + offsetX;
            else
                this.Top = cursorPos.Y - this.Height - offsetX;

            // Capture screen region
            int captureX = Math.Max(0, cursorPos.X - (regionSize / 2));
            int captureY = Math.Max(0, cursorPos.Y - (regionSize / 2));

            using (var bitmap = new Bitmap(regionSize, regionSize))
            {
                using (var g = Graphics.FromImage(bitmap))
                {
                    try
                    {
                        g.CopyFromScreen(captureX, captureY, 0, 0, new Size(regionSize, regionSize));
                    }
                    catch
                    {
                        return; // Ignore errors
                    }
                }

                using (var g = this.CreateGraphics())
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.Clear(Color.Transparent);
                    g.DrawImage(bitmap, new Rectangle(0, 0, this.Width, this.Height));
                    DrawGlowingBorder(g);
                }
            }
        }

        private void DrawGlowingBorder(Graphics g)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                int glowWidth = 8;
                path.AddEllipse(new Rectangle(glowWidth, glowWidth, this.Width - 2 * glowWidth, this.Height - 2 * glowWidth));

                using (PathGradientBrush brush = new PathGradientBrush(path))
                {
                    brush.CenterColor = Color.Cyan;
                    brush.SurroundColors = new[] { Color.FromArgb(0, Color.Cyan) }; // Transparent edge
                    g.FillEllipse(brush, new Rectangle(0, 0, this.Width, this.Height));
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Set window shape to circular
            int diameter = Math.Min(this.Width, this.Height);
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(0, 0, diameter, diameter);
                this.Region = new Region(path);
            }
        }
    }
}
