using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Magnifier.HotKeys;

namespace Magnifier
{
    public partial class Magnifier : Form
    {
        private Timer updateTimer;
        private Bitmap magnifiedBitmap;
        private Rectangle lastCaptureArea;
        private Color transparencyKey = Color.Black;
        private float zoomFactor = 2.0f;
        private int regionSize = 200;
        private int _fps = 60;
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;

        public Color TransparencyKeyColor
        {
            get => transparencyKey;
            set
            {
                transparencyKey = value;
                this.TransparencyKey = transparencyKey; // Apply the new transparency key
            }
        }

        public float ZoomFactor
        {
            get => zoomFactor;
            set
            {
                zoomFactor = value > 0 ? value : 1.0f; // Ensure zoom factor is positive
            }
        }

        public int FPS
        {
            get => _fps;
            set
            {
                _fps = value;
                if (updateTimer != null)
                {
                    updateTimer.Interval = 1000 / _fps; // Update the timer interval based on FPS
                }
            }
        }

        public Magnifier()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.BackColor = Color.Black;
            this.TransparencyKey = Color.Black;
            this.DoubleBuffered = true; // Enable double buffering

            // Initialize tray icon and menu
            InitializeTrayIcon();

            // Initialize timer for updates
            updateTimer = new Timer { Interval = 1000 / _fps }; // Default to 60 FPS
            updateTimer.Tick += UpdateMagnifier;
            updateTimer.Start();

            // Set the initial size of the magnifier
            this.Size = new Size(regionSize * 2, regionSize * 2);

            // Initialize bitmap for magnified area
            magnifiedBitmap = new Bitmap(regionSize, regionSize);
        }

        private void InitializeTrayIcon()
        {
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Toggle Magnifier", null, ToggleMagnifier);
            trayMenu.Items.Add("Settings", null, OpenSettings);
            trayMenu.Items.Add("Exit", null, ExitApplication);

            trayIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                ContextMenuStrip = trayMenu,
                Text = "Magnifier",
                Visible = true
            };
            trayIcon.DoubleClick += OpenSettings;
        }

        private void ToggleMagnifier(object sender, EventArgs e)
        {
            this.Visible = !this.Visible;
        }

        private void OpenSettings(object sender, EventArgs e)
        {
            using (var settingsForm = new SettingsForm(this))
            {
                settingsForm.ShowDialog();
            }
        }

        private void ExitApplication(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Application.Exit();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            HotKeyManager.UnregisterHotKey(this);
            trayIcon.Dispose();
            magnifiedBitmap?.Dispose();
            base.OnFormClosed(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            HotKeyManager.RegisterHotKey(this, Keys.Z, KeyModifiers.Control | KeyModifiers.Shift);
            HotKeyManager.HotKeyPressed += HotKeyManager_HotKeyPressed;
        }

        private void HotKeyManager_HotKeyPressed(object sender, HotKeyEventArgs e)
        {
            this.Visible = !this.Visible;
        }

        private void UpdateMagnifier(object sender, EventArgs e)
        {
            var cursorPos = Cursor.Position;

            // Adjust position
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

            // Capture region
            int captureX = Math.Max(0, cursorPos.X - (int)((regionSize / 2) / zoomFactor));
            int captureY = Math.Max(0, cursorPos.Y - (int)((regionSize / 2) / zoomFactor));
            int captureWidth = (int)(regionSize / zoomFactor);
            int captureHeight = (int)(regionSize / zoomFactor);

            // Ensure the bitmap matches the capture size
            if (magnifiedBitmap.Width != captureWidth || magnifiedBitmap.Height != captureHeight)
            {
                magnifiedBitmap?.Dispose(); // Dispose of the old bitmap
                magnifiedBitmap = new Bitmap(captureWidth, captureHeight); // Create a new one
            }

            var captureArea = new Rectangle(captureX, captureY, captureWidth, captureHeight);

            // Only update if capture area has changed
            if (captureArea != lastCaptureArea)
            {
                using (var g = Graphics.FromImage(magnifiedBitmap))
                {
                    g.CopyFromScreen(captureArea.Location, Point.Empty, captureArea.Size);
                }
                lastCaptureArea = captureArea;
                Invalidate(); // Trigger repaint
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Ensure the graphics context uses high-quality settings
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // Scale and draw the captured bitmap to fill the magnifier window
            e.Graphics.DrawImage(
                magnifiedBitmap,
                new Rectangle(0, 0, this.Width, this.Height), // Destination rectangle (entire window)
                new Rectangle(0, 0, magnifiedBitmap.Width, magnifiedBitmap.Height), // Source rectangle (entire bitmap)
                GraphicsUnit.Pixel
            );

            // Draw the glowing border
            DrawGlowingBorder(e.Graphics);
        }

        private void DrawGlowingBorder(Graphics g)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                int borderThickness = 12;
                int innerRadius = borderThickness;

                Rectangle outerCircle = new Rectangle(0, 0, this.Width, this.Height);
                Rectangle innerCircle = new Rectangle(
                    innerRadius,
                    innerRadius,
                    this.Width - 2 * innerRadius,
                    this.Height - 2 * innerRadius
                );

                path.AddEllipse(outerCircle);
                path.AddEllipse(innerCircle);

                using (PathGradientBrush brush = new PathGradientBrush(path))
                {
                    brush.CenterColor = Color.Transparent;
                    brush.SurroundColors = new[] { Color.Cyan };
                    g.FillPath(brush, path);
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            int diameter = Math.Min(this.Width, this.Height);
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(0, 0, diameter, diameter);
                this.Region = new Region(path);
            }
        }
    }
}
