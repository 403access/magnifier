
using System.Windows.Forms;

namespace Magnifier
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.StartPosition = FormStartPosition.CenterScreen; // Center the window
            this.FormBorderStyle = FormBorderStyle.FixedDialog; // No resizing
            this.MaximizeBox = false; // Disable maximize button
            this.MinimizeBox = false; // Disable minimize button
            this.ControlBox = true; // Keep only the close button (X)

            this.Text = "Settings";
            this.Size = new System.Drawing.Size(300, 150);

            var fpsLabel = new Label
            {
                Text = "FPS:",
                Location = new System.Drawing.Point(20, 20),
                AutoSize = true
            };

            var fpsInput = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 120,
                Value = parentMagnifier.FPS,
                Location = new System.Drawing.Point(80, 18),
                Width = 60
            };

            // Zoom Factor Label and Input
            var zoomLabel = new Label
            {
                Text = "Zoom Factor:",
                Location = new System.Drawing.Point(20, 60),
                AutoSize = true
            };

            var zoomInput = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 10,
                DecimalPlaces = 1,
                Increment = 0.1M,
                Value = (decimal)parentMagnifier.ZoomFactor,
                Location = new System.Drawing.Point(120, 58),
                Width = 60
            };

            var saveButton = new Button
            {
                Text = "Save",
                Location = new System.Drawing.Point(100, 80),
                AutoSize = true
            };

            saveButton.Click += (s, e) =>
            {
                parentMagnifier.FPS = (int)fpsInput.Value;
                parentMagnifier.ZoomFactor = (float)zoomInput.Value;
                this.Close();
            };

            this.Controls.Add(fpsLabel);
            this.Controls.Add(fpsInput);
            this.Controls.Add(zoomLabel);
            this.Controls.Add(zoomInput);
            this.Controls.Add(saveButton);
        }

        #endregion
    }
}