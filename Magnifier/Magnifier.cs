﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Magnifier.HotKeys;

namespace Magnifier
{
    public partial class Magnifier : Form
    {
        private Timer updateTimer;
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
            // Initialize Form properties
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.BackColor = Color.Black;
            this.TransparencyKey = Color.Black;

            // Initialize tray icon and menu
            InitializeTrayIcon();

            // Initialize timer for updates
            updateTimer = new Timer { Interval = 1000 / _fps }; // Default to 60 FPS
            updateTimer.Tick += UpdateMagnifier;
            updateTimer.Start();

            // Set the initial size of the magnifier
            this.Size = new Size(regionSize * 2, regionSize * 2);
        }

        private void InitializeTrayIcon()
        {
            // Create the tray menu
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Toggle Magnifier", null, ToggleMagnifier);
            trayMenu.Items.Add("Settings", null, OpenSettings);
            trayMenu.Items.Add("Exit", null, ExitApplication);

            // Create the tray icon
            trayIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application, // Replace with a custom icon if needed
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
            base.OnFormClosed(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Register global hotkey (CTRL + SHIFT + Z)
            HotKeyManager.RegisterHotKey(this, Keys.Z, KeyModifiers.Control | KeyModifiers.Shift);

            // Subscribe to the hotkey pressed event
            HotKeyManager.HotKeyPressed += HotKeyManager_HotKeyPressed;
        }

        private void HotKeyManager_HotKeyPressed(object sender, HotKeyEventArgs e)
        {
            this.Visible = !this.Visible; // Toggle visibility
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
            int captureX = Math.Max(0, cursorPos.X - (int)((regionSize / 2) / zoomFactor));
            int captureY = Math.Max(0, cursorPos.Y - (int)((regionSize / 2) / zoomFactor));
            int captureWidth = (int)(regionSize / zoomFactor);
            int captureHeight = (int)(regionSize / zoomFactor);

            using (var bitmap = new Bitmap(captureWidth, captureHeight))
            {
                using (var g = Graphics.FromImage(bitmap))
                {
                    try
                    {
                        g.CopyFromScreen(captureX, captureY, 0, 0, new Size(captureWidth, captureHeight));
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
                int borderThickness = 12; // Thickness of the border
                int innerRadius = borderThickness; // Inner glow radius

                // Outer circle (full size of the magnifier)
                Rectangle outerCircle = new Rectangle(0, 0, this.Width, this.Height);

                // Inner circle (creates the hollow center)
                Rectangle innerCircle = new Rectangle(
                    innerRadius,
                    innerRadius,
                    this.Width - 2 * innerRadius,
                    this.Height - 2 * innerRadius
                );

                // Add the outer and inner ellipses to the path
                path.AddEllipse(outerCircle);
                path.AddEllipse(innerCircle);

                // Create a PathGradientBrush for the glow effect
                using (PathGradientBrush brush = new PathGradientBrush(path))
                {
                    // Set the center color to transparent to avoid filling the middle
                    brush.CenterColor = Color.Transparent;

                    // Set the border color for the glow effect
                    brush.SurroundColors = new[] { Color.Cyan }; // Replace with desired color

                    // Draw the border glow
                    g.FillPath(brush, path);
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
